using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class ProjectileSimulator : MonoBehaviour
{
    [Header("Target Position")]
    [SerializeField] bool isFollowingTarget = false;
    [SerializeField] Transform target;

    [Header("References")]
    [SerializeField] private Material lineMaterial;
    public Transform projectile;
    public GLTrajectoryRenderer glRenderer;
    public GameObject canonFace;

    private List<Vector3> trajectoryPoints = new List<Vector3>();

    [Header("Settings")]
    public bool useAirResistance = true;

    [Header("Inputs")]
    public float initialSpeed = 10f;
    public float launchAngle = 45f;
    public float gravity = 9.81f;
    public float mass = 1f;
    public float altitude = 0f;
    public float diameter = 0.1f;
    public float height = 0f;

    [Header("Drag Settings")]
    public float dragCoefficient = 0.47f;

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
    private float timeElapsed = 0f;
    private bool isSimulating = false;

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

    public void Launch()
    {
        StopAllCoroutines();
        timeElapsed = 0f;
        isSimulating = false;
        trajectoryPoints.Clear();

        Vector3 launchPos = startPos;
        launchPos.y += height;
        startX = launchPos.x;

        float angleRad = launchAngle * Mathf.Deg2Rad;
        Vector3 simVelocity = new Vector3(
            initialSpeed * Mathf.Cos(angleRad),
            initialSpeed * Mathf.Sin(angleRad),
            0f
        );

        Vector3 position = launchPos;
        float simStep = 0.005f;
        float localTime = 0f;

        while (position.y >= startPos.y)
        {
            trajectoryPoints.Add(position);

            Vector3 acceleration = new Vector3(0f, -gravity, 0f);
            if (useAirResistance)
            {
                float airDensity = 1.225f * Mathf.Pow((1 - 0.0000225577f * altitude), 5.25588f);
                float radius = diameter / 2f;
                float area = Mathf.PI * radius * radius;
                float speed = simVelocity.magnitude;
                Vector3 dragForce = 0.5f * airDensity * speed * speed * dragCoefficient * area * -simVelocity.normalized;
                acceleration += dragForce / mass;
            }

            simVelocity += acceleration * simStep;
            position += simVelocity * simStep;
            localTime += simStep;
        }

        timeElapsed = localTime;
        Debug.Log($"🎯 Distance Traveled: {position.x - startX:F3} m");
        Debug.Log($"🕒 Time of Flight: {timeElapsed:F3} s");

        if (glRenderer != null)
        {
            glRenderer.Initialize(lineMaterial, trajectoryPoints);
            glRenderer.UpdateDrawCount(0);

        }

        StartCoroutine(AnimateProjectilePath());
    }

    private IEnumerator AnimateProjectilePath()
    {
        isSimulating = true;

        float elapsed = 0f;
        int index = 0;

        projectile.position = trajectoryPoints[0];

        while (index < trajectoryPoints.Count - 1)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / timeElapsed;
            t = Mathf.Clamp01(t);

            int newIndex = Mathf.FloorToInt(t * (trajectoryPoints.Count - 1));
            index = newIndex;

            float tIndex = t * (trajectoryPoints.Count - 1);
            int i0 = Mathf.FloorToInt(tIndex);
            int i1 = Mathf.Min(i0 + 1, trajectoryPoints.Count - 1);
            float lerpT = tIndex - i0;

            projectile.position = Vector3.Lerp(trajectoryPoints[i0], trajectoryPoints[i1], lerpT);

            if (glRenderer != null)
                glRenderer.UpdateDrawCount(index + 1);


            if (isFollowingTarget && target != null)
                target.position = projectile.position;

            yield return null;
        }

        projectile.position = trajectoryPoints[^1];
        if (glRenderer != null)
            glRenderer.UpdateDrawCount(trajectoryPoints.Count);


        isSimulating = false;
    }
}
