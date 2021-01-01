using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class CarveObject : MonoBehaviour
{
    //ListOfwaypoint
    public GameObject[] waypoints;

    //Current way point
    int current = 0;

    //speed to move them
    public float speed = 5;

    //Making a radius of 1 float point to stop object at this point
    private float extraRadius = 1f;

    void Update()
    {
        if (Vector3.Distance(waypoints[current].transform.position, transform.position) < extraRadius)
        {
            current++;
            if (current >= waypoints.Length)
            {
                current = 0;
            }
        }
        transform.position = Vector3.MoveTowards(transform.position, waypoints[current].transform.position, Time.deltaTime * speed);

    }
}
