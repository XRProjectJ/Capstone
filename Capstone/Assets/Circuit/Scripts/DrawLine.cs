using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawLine : MonoBehaviour
{
    [SerializeField] private GameObject[] cp = new GameObject[2];
    private Vector3 midCp;
    public LineRenderer lineRenderer;
    private int pointCount = 100;
    // Start is called before the first frame update
    void Start()
    {
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
        lineRenderer.widthMultiplier = 0.05f;

    }
    // Update is called once per frame
    void Update()
    {
        Draw();
    }
    private void Draw()
    {
        Vector3[] pointPos = new Vector3[pointCount];
        midCp = (cp[0].transform.position + cp[1].transform.position) / 2 + new Vector3(0, 0.03f, 0);
        lineRenderer.positionCount = pointCount;
        for (int i = 0; i < pointCount; i++)
        {
            float t = i / (float)pointCount;
            pointPos[i] = (1 - t) * (1 - t) * cp[0].transform.position + 2 * t * (1 - t) * midCp + t * t * cp[1].transform.position;

        }
        lineRenderer.SetPositions(pointPos);

    }
}
