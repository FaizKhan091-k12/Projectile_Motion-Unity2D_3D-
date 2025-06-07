using UnityEngine;
using System.Collections.Generic;

public class GLTrajectoryRenderer : MonoBehaviour
{
    private Material lineMaterial;
    private List<Vector3> points = new List<Vector3>();
    private int drawCount = 0;

    public void Initialize(Material material, List<Vector3> trajectoryPoints)
    {
        lineMaterial = material;
        points = new List<Vector3>(trajectoryPoints);
        drawCount = 0;
    }

    public void UpdateDrawCount(int count)
    {
        drawCount = Mathf.Clamp(count, 0, points.Count);
    }

    private void OnRenderObject()
    {
        if (lineMaterial == null || drawCount < 2)
            return;

        // Don’t filter by camera
        lineMaterial.SetPass(0);
        GL.PushMatrix();
        GL.Begin(GL.LINES);
        GL.Color(Color.cyan);

        for (int i = 0; i < drawCount - 1; i++)
        {
            GL.Vertex(points[i]);
            GL.Vertex(points[i + 1]);
        }

        GL.End();
        GL.PopMatrix();
    }
}