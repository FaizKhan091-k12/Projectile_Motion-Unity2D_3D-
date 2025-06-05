using UnityEngine;
using System.Collections.Generic;

public class GLTrajectoryRenderer : MonoBehaviour
{
    [Header("Line Settings")]
    public Color lineColor = Color.cyan;

    private Material lineMaterial;
    private List<Vector3> points = new List<Vector3>();
    private int drawCount = 0;
    private Camera cam;

    /// <summary>
    /// Initializes the GL line with given material and points.
    /// </summary>
    public void Initialize(Material material, List<Vector3> trajectoryPoints)
    {
        lineMaterial = material;
        points = new List<Vector3>(trajectoryPoints);
        drawCount = 0;
        cam = Camera.main;
    }

    /// <summary>
    /// Update the number of visible points to draw.
    /// </summary>
    public void UpdateDrawCount(int count)
    {
        drawCount = Mathf.Clamp(count, 0, points.Count);
    }

    private void OnRenderObject()
    {
        if (lineMaterial == null || drawCount < 2)
            return;

        // Only draw when rendering from the main camera (or fallback if null)
        if (Camera.current != cam && cam != null)
            return;

        lineMaterial.SetPass(0);

        GL.PushMatrix();
        GL.MultMatrix(transform.localToWorldMatrix);
        GL.Begin(GL.LINES);
        GL.Color(lineColor);

        for (int i = 0; i < drawCount - 1; i++)
        {
            GL.Vertex(points[i]);
            GL.Vertex(points[i + 1]);
        }

        GL.End();
        GL.PopMatrix();
    }
}