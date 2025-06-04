using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class ProjectileSimulator : MonoBehaviour
{
    [Header("References")]
    public Transform projectile;
    public LineRenderer trajectoryLine;
    public int resolution = 50; // Number of points
    public float pointDelay = 0.1f; // Delay between points in seconds
    public GameObject canonFace;

    private Coroutine drawCoroutine;
    private List<Vector3> trajectoryPoints = new List<Vector3>();

    [Header("Settings")]
    public bool useAirResistance = true;

    [Header("Inputs")]
    public float initialSpeed = 10f;
    public float launchAngle = 45f;     // in degrees
    public float gravity = 9.81f;
    public float mass = 1f;             // in kg
    public float altitude = 0f;         // affects air resistance
    public float diameter = 0.1f;       // in meters
    public float height = 0f;           // initial launch height

    [Header("Drag Settings")]
    public float dragCoefficient = 0.47f; // For a sphere

    [Header("UI References")]
    [SerializeField] Slider initialSpeedslider;
    [SerializeField] Slider launchAngleSlider;
    [SerializeField] Slider gravitySlider;
    [SerializeField] Slider massSlider;
    [SerializeField] Slider altitudeSlider;
    [SerializeField] Slider diameterSlider;
    [SerializeField] Slider heightSlider;
    [SerializeField] Toggle useAirResistanceToggle;

    private Vector3 velocity;
    private Vector3 startPos;
    private float startX;
    private bool isSimulating = false;
    private float timeElapsed = 0f;

    void Start()
    {
        InitializeValues();
        startPos = projectile.position;
        initialSpeedslider.onValueChanged.AddListener(delegate { initialSpeed = initialSpeedslider.value; });
        launchAngleSlider.onValueChanged.AddListener(delegate
        {
            launchAngle = launchAngleSlider.value;
            canonFace.transform.localRotation = Quaternion.Euler(launchAngle, 0, 0);
        });
        gravitySlider.onValueChanged.AddListener(delegate { gravity = gravitySlider.value; });
        massSlider.onValueChanged.AddListener(delegate { mass = massSlider.value; });
        altitudeSlider.onValueChanged.AddListener(delegate { altitude = altitudeSlider.value; });
        diameterSlider.onValueChanged.AddListener(delegate { diameter = diameterSlider.value; });
        heightSlider.onValueChanged.AddListener(delegate { height = heightSlider.value; });
        useAirResistanceToggle.onValueChanged.AddListener(delegate { useAirResistance = useAirResistanceToggle.isOn; });

    }

    public void InitializeValues()
    {
        initialSpeed = initialSpeedslider.value;
        launchAngle = launchAngleSlider.value;
        gravity = gravitySlider.value;
        mass = massSlider.value; 
        altitude = altitudeSlider.value;
        diameter = diameterSlider.value;
        height = heightSlider.value;
        useAirResistance = useAirResistanceToggle.isOn;
        canonFace.transform.localRotation = Quaternion.Euler(launchAngle, 0, 0);
    }

    public void StartDrawingTrajectory()
    {
        if (drawCoroutine != null)
            StopCoroutine(drawCoroutine);

        drawCoroutine = StartCoroutine(DrawTrajectoryOverTime());
    }

    private IEnumerator DrawTrajectoryOverTime()
    {
        trajectoryLine.positionCount = 0;
        float simStep = 0.1f;
        Vector3[] points = new Vector3[resolution];

        float angleRad = launchAngle * Mathf.Deg2Rad;
        Vector3 velocity = new Vector3(
            initialSpeed * Mathf.Cos(angleRad),
            initialSpeed * Mathf.Sin(angleRad),
            0f
        );

        Vector3 position = transform.position + new Vector3(0, height, 0);

        for (int i = 0; i < resolution; i++)
        {
            points[i] = position;
            trajectoryLine.positionCount = i + 1;
            trajectoryLine.SetPosition(i, position);

            Vector3 acceleration = new Vector3(0f, -gravity, 0f);

            if (useAirResistance)
            {
                float airDensity = 1.225f * Mathf.Pow((1 - 0.0000225577f * altitude), 5.25588f);
                float radius = diameter / 2f;
                float area = Mathf.PI * radius * radius;
                float speed = velocity.magnitude;
                Vector3 dragForce = 0.5f * airDensity * speed * speed * dragCoefficient * area * -velocity.normalized;
                acceleration += dragForce / mass;
            }

            velocity += acceleration * simStep;
            position += velocity * simStep;

            yield return new WaitForSeconds(pointDelay);
        }
    }

    public void Launch()
    {
        StopAllCoroutines();
        timeElapsed = 0f;
        isSimulating = true;
        trajectoryPoints.Clear();

        Vector3 launchPos = startPos;
        launchPos.y += height;
        projectile.position = launchPos;
        startX = launchPos.x;

        float angleRad = launchAngle * Mathf.Deg2Rad;
        velocity = new Vector3(
            initialSpeed * Mathf.Cos(angleRad),
            initialSpeed * Mathf.Sin(angleRad),
            0f
        );

        StartCoroutine(SimulateProjectileMotion());
    }

    private IEnumerator SimulateProjectileMotion()
    {
        float simStep = 0.005f;

        while (isSimulating)
        {
            timeElapsed += simStep;

            Vector3 acceleration = new Vector3(0f, -gravity, 0f);

            if (useAirResistance)
            {
                float airDensity = 1.225f * Mathf.Pow((1 - 0.0000225577f * altitude), 5.25588f);
                float radius = diameter / 2f;
                float area = Mathf.PI * radius * radius;

                float speed = velocity.magnitude;
                Vector3 dragForce = 0.5f * airDensity * speed * speed * dragCoefficient * area * -velocity.normalized;

                acceleration += dragForce / mass;
            }

            velocity += acceleration * simStep;
            projectile.position += velocity * simStep;
            trajectoryPoints.Add(projectile.position);

            if (projectile.position.y <= startPos.y)
            {
                isSimulating = false;
                projectile.position = new Vector3(projectile.position.x, startPos.y, projectile.position.z);

                float distanceTraveled = projectile.position.x - startX;
                Debug.Log($"🎯 Distance Traveled: {distanceTraveled:F3} m");
                Debug.Log($"🕒 Time of Flight: {timeElapsed:F3} s");

                trajectoryLine.positionCount = trajectoryPoints.Count;
                trajectoryLine.SetPositions(trajectoryPoints.ToArray());
            }

            yield return null;
        }
    }

    public void DrawTrajectory()
    {
        trajectoryLine.positionCount = resolution;

        Vector3[] points = new Vector3[resolution];

        float angleRad = launchAngle * Mathf.Deg2Rad;
        Vector3 startVelocity = new Vector3(
            initialSpeed * Mathf.Cos(angleRad),
            initialSpeed * Mathf.Sin(angleRad),
            0f
        );

        Vector3 startPos = transform.position + new Vector3(0, height, 0);

        float simStep = 0.1f;
        Vector3 velocity = startVelocity;
        Vector3 position = startPos;

        for (int i = 0; i < resolution; i++)
        {
            points[i] = position;

            Vector3 acceleration = new Vector3(0f, -gravity, 0f);
            if (useAirResistance)
            {
                float airDensity = 1.225f * Mathf.Pow((1 - 0.0000225577f * altitude), 5.25588f);
                float radius = diameter / 2f;
                float area = Mathf.PI * radius * radius;
                float speed = velocity.magnitude;
                Vector3 dragForce = 0.5f * airDensity * speed * speed * dragCoefficient * area * -velocity.normalized;
                acceleration += dragForce / mass;
            }

            velocity += acceleration * simStep;
            position += velocity * simStep;
        }

        trajectoryLine.SetPositions(points);
    }
}
