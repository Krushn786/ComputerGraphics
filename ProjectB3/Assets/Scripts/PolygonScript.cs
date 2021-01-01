using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PolygonScript : MonoBehaviour
{
    public int pointCount = 3;
    public List<Vector3> points;
    public Vector3 min, max;
    //public float midY, height;
    public float radius = 1;
    //private int trav = 0;

    //public GameObject prismObject;

    void Start()
    {
        points = new List<Vector3>();
        //trav = 0;
        
    }
    void Update()
    {
        points.Clear();
        for (int i = 0; i < pointCount; i++)
        {
            var pointOffSet = new Vector3(Mathf.Sin((float)i / pointCount * 2 * Mathf.PI), 0, Mathf.Cos((float)i / pointCount * 2 * Mathf.PI)) * radius;
            points.Add(pointOffSet + transform.position);

        }
        //trav = 0;
        for (int i = 0; i < pointCount; i++)
        {
            Debug.DrawLine(points[i], points[(i + 1) % pointCount], Color.green);
        }
    }
}
