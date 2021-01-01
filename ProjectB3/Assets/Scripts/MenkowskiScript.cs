using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ExecuteInEditMode]
public class MenkowskiScript : PrismManager
{
    public Prism polygon1, polygon2;
    public Operator minkowskiOperator;

    private List<Vector3> points;

    public enum Operator {
        SUM,
        DIFFERENCE
    }
    // Start is called before the first frame update
    void Start()
    {
        points = new List<Vector3>();
        StartCoroutine(Run());
    }

    private IEnumerator Run(float updateRate = 0.2f) {
        while (true) {

            #region Minkowski Computation 

            points.Clear();
            foreach (var point1 in polygon1.points)
            {
                foreach (var point2 in polygon2.points)
                {
                    var result = point1 + point2;

                    switch (minkowskiOperator)
                    {
                        case Operator.SUM:
                            result = point1 + point2;
                            break;
                        case Operator.DIFFERENCE:
                            result = point1 - point2;
                            break;
                        default:
                            break;
                    }
                    points.Add(result);
                }
            }

            /*for (int i = 0; i < points.Count; i++)
            {
                Debug.DrawLine(points[i], points[i] + Vector3.up * 3, Color.green, updateRate);
            }*/

            Debug.DrawLine(Vector3.zero, Vector3.up * 2, Color.red, updateRate);
            #endregion

            #region GJK Algorithm
            var simplex = new List<Vector3>();
            var intersecting = false;

            if (points.Count > 2)
            {
                simplex.Add(points.Aggregate((a, b) => a.x < b.x ? a : b));
                simplex.Add(points.Where(p => p != simplex[0]).Aggregate((a, b) => a.x > b.x ? a : b));

                do
                {
                    if (simplex.Count == 3) {
                        var simplexOrentation = Mathf.Sign(Vector3.Dot(Vector3.Cross(simplex[1] - simplex[0], Vector3.up), simplex[2] - simplex[0]));

                        var bounded = true;
                        Vector3? deletePoint = null;
                        for (int i = 0; i < 3; i++) {
                            var a = simplex[i];
                            var b = simplex[(i + 1) % 3];

                            var temp = Mathf.Sign(Vector3.Dot(Vector3.Cross(b - a, Vector3.up), Vector3.zero - a));
                            if (temp != simplexOrentation) {
                                bounded = false;

                                deletePoint = simplex[(i + 2) % 3];
                                break;
                            }
                        }

                        print(bounded);
                        if (bounded) {
                            intersecting = true;
                            break;
                        }

                        if (deletePoint.HasValue) {
                            simplex.Remove(deletePoint.Value);
                        }
                        break;
                        Debug.Break();
                    }
                    var dir = simplex[1] - simplex[0];
                    var tangent = Vector3.Cross(dir, Vector3.up);
                    var orientation = Mathf.Sign(Vector3.Dot(tangent, -simplex[0]));
                    var supportAxis = tangent * orientation;

                    //for (int i = 0; i < simplex.Count; i++) {
                    //Debug.DrawLine(simplex[0], simplex[(1) % simplex.Count], Color.cyan, updateRate);
                    //Debug.DrawLine((simplex[0] + simplex[1]) / 2f, (simplex[0] + simplex[1]) / 2f + supportAxis, Color.blue, updateRate);
                    //}

                    var supportPoint = points.Aggregate((a, b) => Vector3.Dot(a, supportAxis) > Vector3.Dot(b, supportAxis) ? a : b);

                    if (!simplex.Contains(supportPoint))
                    {
                        simplex.Add(supportPoint);
                    }

                    yield return null;
                    //Debug.DrawLine(supportPoint, supportPoint + Vector3.up * 3, Color.magenta, updateRate);
                    //print(orientation);
                } while (simplex.Count == 3);

                /*if (intersecting) {
                    for (int i = 0; i < 3; i++)
                    {
                        Debug.DrawLine(simplex[i], simplex[(i + 1) % 3], Color.magenta, updateRate);
                    }
                    yield return new WaitForSeconds(updateRate);
                }*/
            }

            #endregion

            #region EPA Algorithm
            if (intersecting)
            {
                if (PointToLine(simplex[0], simplex[1], simplex[2]) > 0){

                    var temp = simplex[0];
                    simplex[0] = simplex[1];
                    simplex[1] = temp;
                }
                var distToSimplexSegments = new List<float>();
                for (int s = 0; s < simplex.Count; s++) {
                    var a = simplex[s];
                    var b = simplex[(s + 1) % simplex.Count];
                    distToSimplexSegments.Add(Mathf.Abs(PointToLine(Vector3.zero, a, b)));
                }

                var minIndex = MinIndex(distToSimplexSegments);
                var minDist = distToSimplexSegments[minIndex];

                for (int i = 0; i < 10; i++) {
                    if (true) {
                        var a = simplex[minIndex];
                        var b = simplex[(minIndex + 1) % simplex.Count];

                        Debug.DrawLine(a, b, Color.cyan, updateRate);
                    }

                    var dir = simplex[(minIndex + 1) % simplex.Count] - simplex[minIndex];
                    var tangent = Vector3.Cross(dir, Vector3.up);
                    var orentation = -Mathf.Sign(Vector3.Dot(tangent, -simplex[minIndex]));
                    var supportAxis = tangent * orentation;
                    var supportPoint = points.Aggregate((a, b) => Vector3.Dot(a, supportAxis) > Vector3.Dot(b, supportAxis) ? a : b);

                    if (simplex.Contains(supportPoint))
                    {
                        break;
                    }
                    else {
                        var ind = (minIndex + 1) % simplex.Count;
                        simplex.Insert(ind, supportPoint);
                        distToSimplexSegments.Insert(ind, float.MaxValue);

                        minIndex = MinIndex(distToSimplexSegments);
                        for (int s = minIndex; s <= minIndex + 1; s++)
                        {
                            var a = simplex[(s) % simplex.Count];
                            var b = simplex[(s + 1) % simplex.Count];

                            distToSimplexSegments[(s) % simplex.Count] = Mathf.Abs(PointToLine(Vector3.zero, a, b));
                        }
                        minIndex = MinIndex(distToSimplexSegments);
                        minDist = distToSimplexSegments[minIndex];
                    }

                }
                for (int s = 0; s < simplex.Count; s++) {
                    Debug.DrawLine(simplex[s], simplex[(s + 1) % simplex.Count], Color.magenta, updateRate);
                }
                Debug.DrawLine(Vector3.zero, (simplex[(minIndex + 1) % simplex.Count]) / 2, Color.white, updateRate);

                var tan = PointToLineTangent(Vector3.zero, simplex[minIndex], simplex[(minIndex + 1) % simplex.Count]);
                Debug.DrawLine(polygon1.transform.position, polygon1.transform.position - tan, Color.red, updateRate);
                Debug.DrawLine(polygon2.transform.position, polygon2.transform.position + tan, Color.red, updateRate);
            }


            #endregion

            yield return new WaitForSeconds(updateRate);
        }
    }

    private float PointToLine(Vector3 p, Vector3 a, Vector3 b) {
        var newVec = p - a;
        var dir = b - a;
        var tangent = Vector3.Cross(dir, Vector3.up).normalized;
        var result = Vector3.Dot(newVec, tangent) / (newVec.magnitude) * (newVec.magnitude);

        return result;
  }

    private Vector3 PointToLineTangent(Vector3 p, Vector3 a, Vector3 b) {
        var newVec = p - a;
        var dir = b - a;
        var tangent = Vector3.Cross(dir, Vector3.up).normalized;

        var result = Vector3.Dot(newVec, tangent) / (newVec.magnitude) * (newVec.magnitude);

        return tangent * result;
    }

    private int MinIndex(List<float> a) {
        int trav = 0;
        float min = a[trav];

        for (int i = 1; i < a.Count; i++) {
            if(min > a[i])
            {
                min = a[i];
                trav = i;
            }
        }

        return trav;
    }

    private float ProjMag(Vector3 a, Vector3 b) {
        return Vector3.Dot(a, b) / (a.magnitude * b.magnitude) * a.magnitude;
    }


}
