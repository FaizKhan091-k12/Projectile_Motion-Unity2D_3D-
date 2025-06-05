using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(MeshRenderer))]
public class ProjectileLogic : MonoBehaviour
{
    public float timeToTravel = 2f;
    public Material lineMaterial;

    private float elapsed = 0f;
    private int currentIndex = 0;
    private GLTrajectoryRenderer glDrawer;
    private bool isAnimating = false;
    private List<Vector3> trajectoryPoints;

    public void Initialize(List<Vector3> points)
    {
        if (points == null || points.Count < 2)
        {
            Debug.LogError("ProjectileLogic: Trajectory not set or too short.");
            return;
        }

        trajectoryPoints = points;

        glDrawer = gameObject.AddComponent<GLTrajectoryRenderer>();
        glDrawer.Initialize(lineMaterial, trajectoryPoints);
        glDrawer.enabled = true;

        transform.position = trajectoryPoints[0];
        isAnimating = true;
    }

    void Update()
    {
        if (!isAnimating || trajectoryPoints.Count < 2) return;

        elapsed += Time.deltaTime;
        float t = Mathf.Clamp01(elapsed / timeToTravel);

        float tIndex = t * (trajectoryPoints.Count - 1);
        int i0 = Mathf.FloorToInt(tIndex);
        int i1 = Mathf.Min(i0 + 1, trajectoryPoints.Count - 1);
        float lerpT = tIndex - i0;

        transform.position = Vector3.Lerp(trajectoryPoints[i0], trajectoryPoints[i1], lerpT);
        glDrawer.UpdateDrawCount(Mathf.FloorToInt(t * trajectoryPoints.Count));

        if (t >= 1f)
        {
            isAnimating = false;
            transform.position = trajectoryPoints[^1];
            glDrawer.UpdateDrawCount(trajectoryPoints.Count);
        }
    }
}