using UnityEngine;
using System.Collections.Generic;

public class GLTrajectoryRenderer : MonoBehaviour
{
    public Material lineMaterial;
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

        lineMaterial.SetPass(0);
        GL.PushMatrix();
        GL.Begin(GL.LINES);

        for (int i = 0; i < drawCount - 1; i++)
        {
            float t = (float)i / (drawCount - 1);

            // Gradient from Green → Yellow → Red
            Color color;
            if (t < 0.5f)
            {
                // Green → Yellow
                color = Color.Lerp(Color.green, Color.yellow, t * 2f);
            }
            else
            {
                // Yellow → Red
                color = Color.Lerp(Color.yellow, Color.red, (t - 0.5f) * 2f);
            }

            GL.Color(color);
            GL.Vertex(points[i]);
            GL.Vertex(points[i + 1]);
        }

        GL.End();
        GL.PopMatrix();
    }
}