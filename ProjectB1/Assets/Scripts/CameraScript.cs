using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    float speed = 25.0f;
    private Vector3 currentMousePostion;
    private Vector3 pos;

    // Use this for initialization
    void Start()
    {
        pos = gameObject.transform.position;
        
    }

    void Update()
    {
        
        if (Input.GetKey("w")) {
            pos.z += speed * Time.deltaTime;
        }
        if (Input.GetKey("s"))
        {
            pos.z -= speed * Time.deltaTime;
        }
        if (Input.GetKey("a"))
        {
            pos.x -= speed * Time.deltaTime;
        }
        if (Input.GetKey("d"))
        {
            pos.x += speed * Time.deltaTime;
        }
        if (Input.GetKey("space")) {
            pos.y += speed * Time.deltaTime;
        }
        if (Input.GetKey("left shift") || Input.GetKey("right shift")) {
            pos.y -= speed * Time.deltaTime;
        }
        transform.position = pos;
    }

}
