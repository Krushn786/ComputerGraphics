using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BroadPhaseScript : PrismManager
{
    public PolygonScript[] polygons;
    // Start is called before the first frame update
    void Start()
    {
        polygons = GameObject.FindObjectsOfType<PolygonScript>();

        StartCoroutine(Run());
    }

    private IEnumerator Run(float updateRate = 0.2f) {
        while (true) {
            var sweepX = new List<KeyValuePair<float, PolygonScript>>();

            foreach (var polygon in polygons) {
                polygon.min = new Vector3(polygon.points.Min(p => p.x), 0, polygon.points.Min(p => p.z));
                polygon.max = new Vector3(polygon.points.Max(p => p.x), 0, polygon.points.Max(p => p.z));

                sweepX.Add(new KeyValuePair<float, PolygonScript>(polygon.min.x, polygon));
                sweepX.Add(new KeyValuePair<float, PolygonScript>(polygon.max.x, polygon));
            }

            sweepX = sweepX.OrderBy(x => x.Key).ToList();

            var active = new HashSet<PolygonScript>();
            foreach (var i in sweepX) {
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
                else {
                    active.Add(i.Value);
                }
            }
            yield return new WaitForSeconds(updateRate);
        }
        yield break;
    }

    private bool IntersectingZ(PolygonScript a, PolygonScript b) {
        if (a.max.z > b.min.z && a.min.z < b.max.z)
        {
            return true;
        }
        else
            return false;
    }
    
}
