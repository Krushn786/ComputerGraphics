using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PrismManager : MonoBehaviour
{
    public int prismCount = 10;
    public float prismRegionRadiusXZ = 5;
    public float prismRegionRadiusY = 5;
    public float maxPrismScaleXZ = 5;
    public float maxPrismScaleY = 5;
    public GameObject regularPrismPrefab;
    public GameObject irregularPrismPrefab;

    private List<Prism> prisms = new List<Prism>();
    private List<GameObject> prismObjects = new List<GameObject>();
    private GameObject prismParent;
    private Dictionary<Prism,bool> prismColliding = new Dictionary<Prism, bool>();

    private const float UPDATE_RATE = 0.5f;

    #region Unity Functions

    void Start()
    {
        Random.InitState(0);    //10 for no collision

        prismParent = GameObject.Find("Prisms");
        for (int i = 0; i < prismCount; i++)
        {
            var randPointCount = Mathf.RoundToInt(3 + Random.value * 7);
            var randYRot = Random.value * 360;
            var randScale = new Vector3((Random.value - 0.5f) * 2 * maxPrismScaleXZ, (Random.value - 0.5f) * 2 * maxPrismScaleY, (Random.value - 0.5f) * 2 * maxPrismScaleXZ);
            var randPos = new Vector3((Random.value - 0.5f) * 2 * prismRegionRadiusXZ, (Random.value - 0.5f) * 2 * prismRegionRadiusY, (Random.value - 0.5f) * 2 * prismRegionRadiusXZ);

            GameObject prism = null;
            Prism prismScript = null;
            if (Random.value < 0.5f)
            {
                prism = Instantiate(regularPrismPrefab, randPos, Quaternion.Euler(0, randYRot, 0));
                prismScript = prism.GetComponent<RegularPrism>();
            }
            else
            {
                prism = Instantiate(irregularPrismPrefab, randPos, Quaternion.Euler(0, randYRot, 0));
                prismScript = prism.GetComponent<IrregularPrism>();
            }
            prism.name = "Prism " + i;
            prism.transform.localScale = randScale;
            prism.transform.parent = prismParent.transform;
            prismScript.pointCount = randPointCount;
            prismScript.prismObject = prism;

            prisms.Add(prismScript);
            prismObjects.Add(prism);
            prismColliding.Add(prismScript, false);
        }

        StartCoroutine(Run());
    }
    
    void Update()
    {
        #region Visualization

        DrawPrismRegion();
        DrawPrismWireFrames();

#if UNITY_EDITOR
        if (Application.isFocused)
        {
            UnityEditor.SceneView.FocusWindowIfItsOpen(typeof(UnityEditor.SceneView));
        }
#endif

        #endregion
    }

    IEnumerator Run()
    {
        yield return null;

        while (true)
        {
            foreach (var prism in prisms)
            {
                prismColliding[prism] = false;
            }

            foreach (var collision in PotentialCollisions())
            {
                if (CheckCollision(collision))
                {
                    prismColliding[collision.a] = true;
                    prismColliding[collision.b] = true;

                    ResolveCollision(collision);
                }
            }

            yield return new WaitForSeconds(UPDATE_RATE);
        }
    }

    #endregion

    #region Incomplete Functions
    //(Broad Check)This Checks for any Collison that might be taking place between object i and other object on list!
    private IEnumerable<PrismCollision> PotentialCollisions()
    {
        float updateRate = 0.2f;
        while (true)
        {
            var sweepX = new List<KeyValuePair<float, Prism>>();

            foreach (var prism in prisms)
            {
                prism.min = new Vector3(prism.points.Min(p => p.x), 0, prism.points.Min(p => p.z));
                prism.max = new Vector3(prism.points.Max(p => p.x), 0, prism.points.Max(p => p.z));

                sweepX.Add(new KeyValuePair<float, Prism>(prism.min.x, prism));
                sweepX.Add(new KeyValuePair<float, Prism>(prism.max.x, prism));
            }

            sweepX = sweepX.OrderBy(x => x.Key).ToList();

            var active = new HashSet<Prism>();
            foreach (var i in sweepX)
            {
                if (active.Contains(i.Value))
                {
                    active.Remove(i.Value);

                    foreach (var j in active)
                    {
                        if (IntersectingZ(i.Value, j))
                        {
                            Debug.DrawLine(i.Value.transform.position, j.transform.position, Color.red, updateRate);
                        }
                    }
                }
                else
                {
                    active.Add(i.Value);
                }
            }
            
        }
        yield break;

        /*for (int i = 0; i < prisms.Count; i++) {
            for (int j = i + 1; j < prisms.Count; j++) {
                var checkPrisms = new PrismCollision();
                checkPrisms.a = prisms[i];
                checkPrisms.b = prisms[j];
                
                yield return checkPrisms;
            }
        }

        yield break;
        */
    }
    private bool IntersectingZ(Prism a, Prism b)
    {
        if (a.max.z > b.min.z && a.min.z < b.max.z)
        {
            return true;
        }
        else
            return false;
    }

    //(Narrow Check)This Checks the if the objects from Broad Check List are actually collideing or not!
    private bool CheckCollision(PrismCollision collision)
    {
       
        float updateRate = 0.2f;
        var prismA = collision.a;
        var prismB = collision.b;
        List<Vector3> points;
        points = new List<Vector3>();

            #region Minkowski Computation 

            points.Clear();
            foreach (var point1 in prismA.points)
            {
                foreach (var point2 in prismB.points)
                {
                    var result = point1 - point2;
                            
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
                    if (simplex.Count == 3)
                    {
                        var simplexOrentation = Mathf.Sign(Vector3.Dot(Vector3.Cross(simplex[1] - simplex[0], Vector3.up), simplex[2] - simplex[0]));

                        var bounded = true;
                        Vector3? deletePoint = null;
                        for (int i = 0; i < 3; i++)
                        {
                            var a = simplex[i];
                            var b = simplex[(i + 1) % 3];

                            var temp = Mathf.Sign(Vector3.Dot(Vector3.Cross(b - a, Vector3.up), Vector3.zero - a));
                            if (temp != simplexOrentation)
                            {
                                bounded = false;

                                deletePoint = simplex[(i + 2) % 3];
                                //break;
                            }
                        }

                        print(bounded);
                        if (bounded)
                        {
                            intersecting = true;
                            //break;
                        }

                        if (deletePoint.HasValue)
                        {
                            simplex.Remove(deletePoint.Value);
                        }
                        //break;
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

                    return false;
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
                return true;
            }

            #endregion

            #region EPA Algorithm
            if (intersecting)
            {
                if (PointToLine(simplex[0], simplex[1], simplex[2]) > 0)
                {

                    var temp = simplex[0];
                    simplex[0] = simplex[1];
                    simplex[1] = temp;
                }
                var distToSimplexSegments = new List<float>();
                for (int s = 0; s < simplex.Count; s++)
                {
                    var a = simplex[s];
                    var b = simplex[(s + 1) % simplex.Count];
                    distToSimplexSegments.Add(Mathf.Abs(PointToLine(Vector3.zero, a, b)));
                }

                var minIndex = MinIndex(distToSimplexSegments);
                var minDist = distToSimplexSegments[minIndex];

                for (int i = 0; i < 10; i++)
                {
                    if (true)
                    {
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
                    else
                    {
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
                for (int s = 0; s < simplex.Count; s++)
                {
                    Debug.DrawLine(simplex[s], simplex[(s + 1) % simplex.Count], Color.magenta, updateRate);
                }
                Debug.DrawLine(Vector3.zero, (simplex[(minIndex + 1) % simplex.Count]) / 2, Color.white, updateRate);

                var tan = PointToLineTangent(Vector3.zero, simplex[minIndex], simplex[(minIndex + 1) % simplex.Count]);
                Debug.DrawLine(prismA.transform.position, prismA.transform.position - tan, Color.red, updateRate);
                Debug.DrawLine(prismB.transform.position, prismB.transform.position + tan, Color.red, updateRate);
                yield return true;
            }


            #endregion

            //new WaitForSeconds(updateRate);
        
           

        //collision.penetrationDepthVectorAB = Vector3.zero;

        //return true;
    }

    private float PointToLine(Vector3 p, Vector3 a, Vector3 b)
    {
        var newVec = p - a;
        var dir = b - a;
        var tangent = Vector3.Cross(dir, Vector3.up).normalized;
        var result = Vector3.Dot(newVec, tangent) / (newVec.magnitude) * (newVec.magnitude);

        return result;
    }

    private Vector3 PointToLineTangent(Vector3 p, Vector3 a, Vector3 b)
    {
        var newVec = p - a;
        var dir = b - a;
        var tangent = Vector3.Cross(dir, Vector3.up).normalized;

        var result = Vector3.Dot(newVec, tangent) / (newVec.magnitude) * (newVec.magnitude);

        return tangent * result;
    }

    private int MinIndex(List<float> a)
    {
        int trav = 0;
        float min = a[trav];

        for (int i = 1; i < a.Count; i++)
        {
            if (min > a[i])
            {
                min = a[i];
                trav = i;
            }
        }

        return trav;
    }

    private float ProjMag(Vector3 a, Vector3 b)
    {
        return Vector3.Dot(a, b) / (a.magnitude * b.magnitude) * a.magnitude;
    }

    #endregion

    #region Private Functions

    //This will help move the colliding objects to a postion on the space  where the objects will collide no more!
    private void ResolveCollision(PrismCollision collision)
    {
        var prismObjA = collision.a.prismObject;
        var prismObjB = collision.b.prismObject;

        var pushA = -collision.penetrationDepthVectorAB / 2;
        var pushB = collision.penetrationDepthVectorAB / 2;

        for (int i = 0; i < collision.a.pointCount; i++)
        {
            collision.a.points[i] += pushA;
        }
        for (int i = 0; i < collision.b.pointCount; i++)
        {
            collision.b.points[i] += pushB;
        }
        //prismObjA.transform.position += pushA;
        //prismObjB.transform.position += pushB;

        Debug.DrawLine(prismObjA.transform.position, prismObjA.transform.position + collision.penetrationDepthVectorAB, Color.cyan, UPDATE_RATE);
    }
    
    #endregion

    #region Visualization Functions

    //This method gives a visual reprensation to the assign prism region so that user can see what exactly is the actual field!
    private void DrawPrismRegion()
    {
        var points = new Vector3[] { new Vector3(1, 0, 1), new Vector3(1, 0, -1), new Vector3(-1, 0, -1), new Vector3(-1, 0, 1) }.Select(p => p * prismRegionRadiusXZ).ToArray();
        
        var yMin = -prismRegionRadiusY;
        var yMax = prismRegionRadiusY;

        var wireFrameColor = Color.yellow;

        foreach (var point in points)
        {
            Debug.DrawLine(point + Vector3.up * yMin, point + Vector3.up * yMax, wireFrameColor);
        }

        for (int i = 0; i < points.Length; i++)
        {
            Debug.DrawLine(points[i] + Vector3.up * yMin, points[(i + 1) % points.Length] + Vector3.up * yMin, wireFrameColor);
            Debug.DrawLine(points[i] + Vector3.up * yMax, points[(i + 1) % points.Length] + Vector3.up * yMax, wireFrameColor);
        }
    }

    private void DrawPrismWireFrames()
    {
        for (int prismIndex = 0; prismIndex < prisms.Count; prismIndex++)
        {
            var prism = prisms[prismIndex];
            var prismTransform = prismObjects[prismIndex].transform;

            var yMin = prism.midY - prism.height / 2 * prismTransform.localScale.y;
            var yMax = prism.midY + prism.height / 2 * prismTransform.localScale.y;

            var wireFrameColor = prismColliding[prisms[prismIndex]] ? Color.red : Color.green;

            foreach (var point in prism.points)
            {
                Debug.DrawLine(point + Vector3.up * yMin, point + Vector3.up * yMax, wireFrameColor);
            }

            for (int i = 0; i < prism.pointCount; i++)
            {
                Debug.DrawLine(prism.points[i] + Vector3.up * yMin, prism.points[(i + 1) % prism.pointCount] + Vector3.up * yMin, wireFrameColor);
                Debug.DrawLine(prism.points[i] + Vector3.up * yMax, prism.points[(i + 1) % prism.pointCount] + Vector3.up * yMax, wireFrameColor);
            }
        }
    }

    #endregion

    #region Utility Classes

    private class PrismCollision
    {
        MenkowskiScript pairs = new MenkowskiScript();

        public Prism a;
        public Prism b;
        public Vector3 penetrationDepthVectorAB;

    }

    private class Tuple<K,V>
    {
        public K Item1;
        public V Item2;

        public Tuple(K k, V v) {
            Item1 = k;
            Item2 = v;
        }
    }

    #endregion
}
