using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailController : MonoBehaviour
{
    private LineRenderer lineRenderer;
    public int maxPoints = 50;
    public float pointSpacing = 0.1f;
    private List<Vector3> points = new List<Vector3>();
    private Vector3 lastPoint;

    private void Start()
    {
        if(lineRenderer == null)
        {
            lineRenderer = GetComponent<LineRenderer>();
        }
        
        lastPoint = transform.position;
    }

    private void Update()
    {
        if (Vector3.Distance(transform.position, lastPoint) > pointSpacing)
        {
            
            AddPoint(transform.position);
            lastPoint = transform.position;
        }
        
        UpdateLineRenderer();
        
    }

    private void AddPoint(Vector3 point)
    {
        points.Add(point);

        if (points.Count > maxPoints)
        {
            points.RemoveAt(0);
        }

    }

    private void UpdateLineRenderer()
    {
        lineRenderer.positionCount = points.Count;
        lineRenderer.SetPositions(points.ToArray());
    }

    public void ClearTrail()
    {
        points.Clear();
        lineRenderer.positionCount = 0;
    }


}
