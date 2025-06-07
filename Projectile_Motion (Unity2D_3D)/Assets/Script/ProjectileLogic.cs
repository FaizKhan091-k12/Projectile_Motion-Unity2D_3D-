using UnityEngine;
using System.Collections.Generic;

public class ProjectileLogic : MonoBehaviour
{
    private List<Vector3> trajectoryPoints;
    private float timeToTravel;
    private float elapsed = 0f;
    private int currentIndex = 0;

    private GLTrajectoryRenderer glDrawer;
    private bool isAnimating = false;

    public void Initialize(List<Vector3> points, float totalTime, Material lineMat, GameObject glDrawerObj)
    {
        if (points == null || points.Count < 2)
        {
            Debug.LogError("Invalid trajectory points");
            return;
        }

        trajectoryPoints = new List<Vector3>(points);
        timeToTravel = totalTime;
        glDrawer = glDrawerObj.GetComponent<GLTrajectoryRenderer>();

        if (glDrawer != null)
        {
            glDrawer.Initialize(lineMat, trajectoryPoints);
            glDrawer.UpdateDrawCount(0);
        }

        transform.position = trajectoryPoints[0];
        isAnimating = true;
    }

    void Update()
    {
        if (!isAnimating || trajectoryPoints == null || trajectoryPoints.Count < 2)
            return;

        elapsed += Time.deltaTime;
        float t = Mathf.Clamp01(elapsed / timeToTravel);

        float tIndex = t * (trajectoryPoints.Count - 1);
        int i0 = Mathf.FloorToInt(tIndex);
        int i1 = Mathf.Min(i0 + 1, trajectoryPoints.Count - 1);
        float lerpT = tIndex - i0;

        transform.position = Vector3.Lerp(trajectoryPoints[i0], trajectoryPoints[i1], lerpT);

        glDrawer?.UpdateDrawCount(i0 + 1);

        if (t >= 1f)
        {
            transform.position = trajectoryPoints[^1];
            glDrawer?.UpdateDrawCount(trajectoryPoints.Count);
            isAnimating = false;
        }
    }
}