using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathDrawer : MonoBehaviour
{

    public Transform bird;
    private LineRenderer lineRenderer; //birds path

    private List<Vector2> pathPoints = new List<Vector2>();


    private bool isDrawing = false;
    private bool startTracking = false;

    private float timeSinceLastPoint = 0f;
    private float recordInterval = 0.1f;
    private float fakeXPosition = 0f;


    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 0;
        lineRenderer.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!startTracking || isDrawing) return;

        timeSinceLastPoint += Time.deltaTime;
        if (timeSinceLastPoint > recordInterval)
        {
            TrackBirdPath();
            timeSinceLastPoint = 0f;
        }


    }

    void TrackBirdPath()
    {
        if (bird == null) return;

        float birdY = bird.position.y;
        fakeXPosition += 0.2f;

        Vector2 newPoint = new Vector2(fakeXPosition, birdY);
        pathPoints.Add(newPoint);

    }

    public void StartTrackingPath()
    {
        startTracking = true;
    }

    public void DrawPathGameOver()
    {

        if (pathPoints.Count == 0) return;

        isDrawing = true;
        lineRenderer.enabled = true;
        lineRenderer.positionCount = pathPoints.Count;

        for (int i = 0; i < pathPoints.Count; i++)
        {
            lineRenderer.SetPosition(i, new Vector3(pathPoints[i].x, pathPoints[i].y, 0));
        }

        Debug.Log("Path Drawn: " + pathPoints.Count + " points.");

    }


    public List<Vector2> GetPath()
    {
        return pathPoints;
    }

    public float GetCurrentFakeX()
    {
        return fakeXPosition;
    }

    public bool HasStartedTracking()
    {
        return startTracking;
    }

}
