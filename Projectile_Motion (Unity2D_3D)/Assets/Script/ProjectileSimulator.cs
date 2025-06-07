using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class ProjectileSimulator : MonoBehaviour
{
    [Header("Target & Launch Settings")]
    [SerializeField] private Transform launchPoint;
    [SerializeField] private bool isFollowingTarget = false;
    [SerializeField] private Transform target;

    [Header("References")]
    public GameObject projectilePrefab; // Drag sphere prefab here
    [SerializeField] private Material lineMaterial;
    public GameObject canonFace;

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
    [SerializeField] private Slider initialSpeedslider;
    [SerializeField] private Slider launchAngleSlider;
    [SerializeField] private Slider gravitySlider;
    [SerializeField] private Slider massSlider;
    [SerializeField] private Slider altitudeSlider;
    [SerializeField] private Slider diameterSlider;
    [SerializeField] private Slider heightSlider;
    [SerializeField] private Toggle useAirResistanceToggle;

    private float timeElapsed = 0f;
    private List<GameObject> activeProjectiles = new List<GameObject>();
    public int maxProjectiles = 5;

    void Start()
    {
        InitializeValues();

        initialSpeedslider.onValueChanged.AddListener(_ => initialSpeed = initialSpeedslider.value);
        launchAngleSlider.onValueChanged.AddListener(_ =>
        {
            launchAngle = launchAngleSlider.value;
            canonFace.transform.localRotation = Quaternion.Euler(launchAngle, 0, 0);
        });
        gravitySlider.onValueChanged.AddListener(_ => gravity = gravitySlider.value);
        massSlider.onValueChanged.AddListener(_ => mass = massSlider.value);
        altitudeSlider.onValueChanged.AddListener(_ => altitude = altitudeSlider.value);
        diameterSlider.onValueChanged.AddListener(_ => diameter = diameterSlider.value);
        heightSlider.onValueChanged.AddListener(_ => height = heightSlider.value);
        useAirResistanceToggle.onValueChanged.AddListener(_ => useAirResistance = useAirResistanceToggle.isOn);
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
        if (activeProjectiles.Count >= maxProjectiles)
        {
            Destroy(activeProjectiles[0]);
            activeProjectiles.RemoveAt(0);
        }

        List<Vector3> trajectoryPoints = CalculateTrajectory(out float totalTime);
        Vector3 launchPos = launchPoint.position + new Vector3(0, height, 0);

        GameObject projectileInstance = Instantiate(projectilePrefab, launchPos, Quaternion.identity);
        activeProjectiles.Add(projectileInstance);

        // 💬 Debug time and distance
        float distanceTraveled = trajectoryPoints[^1].x - launchPos.x;
        Debug.Log($"🕒 Time of Flight: {totalTime:F3} seconds" + $"📏 Distance Traveled: {distanceTraveled:F3} meters");
        //Debug.Log($"📏 Distance Traveled: {distanceTraveled:F3} meters");

        // 💡 Make the target follow this projectile if enabled
        if (isFollowingTarget && target != null)
        {
            target.SetParent(projectileInstance.transform);
            target.localPosition = Vector3.zero;
        }

        Transform glLine = projectileInstance.transform.Find("GLLine");

        if (glLine == null)
        {
            Debug.LogError("GLLine child object not found on the projectile prefab.");
            return;
        }

        ProjectileLogic logic = projectileInstance.GetComponent<ProjectileLogic>();
        if (logic != null)
        {
            logic.Initialize(trajectoryPoints, totalTime, lineMaterial, glLine.gameObject);
        }
        else
        {
            Debug.LogError("ProjectileLogic component not found on projectile prefab.");
        }
    }



    private List<Vector3> CalculateTrajectory(out float timeElapsed)
    {
        List<Vector3> points = new List<Vector3>();
        float angleRad = launchAngle * Mathf.Deg2Rad;

        Vector3 velocity = new Vector3(
            initialSpeed * Mathf.Cos(angleRad),
            initialSpeed * Mathf.Sin(angleRad),
            0f
        );

        Vector3 position = launchPoint.position + new Vector3(0, height, 0);
        float simStep = 0.005f;
        float time = 0f;

        while (position.y >= launchPoint.position.y)
        {
            points.Add(position);

            Vector3 acceleration = new Vector3(0, -gravity, 0);
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
            time += simStep;
        }

        timeElapsed = time;
        return points;
    }
}
