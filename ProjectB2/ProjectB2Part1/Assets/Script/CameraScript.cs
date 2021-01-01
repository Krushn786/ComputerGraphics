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

        if (Input.GetKey("w"))
        {
            gameObject.transform.Translate(0, 0, speed * Time.deltaTime);//+= speed * Time.deltaTime;
        }
        if (Input.GetKey("s"))
        {
            gameObject.transform.Translate(0, 0, -speed * Time.deltaTime);//pos.z -= speed * Time.deltaTime;
        }
        if (Input.GetKey("a"))
        {
            gameObject.transform.Translate(-speed * Time.deltaTime, 0, 0);
        }
        if (Input.GetKey("d"))
        {
            gameObject.transform.Translate(speed * Time.deltaTime, 0, 0);
        }
        if (Input.GetKey(KeyCode.Space))
        {
            gameObject.transform.Translate(0, speed * Time.deltaTime, 0);
        }
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            gameObject.transform.Translate(0, -speed * Time.deltaTime, 0);
        }
        //transform.position = pos;
    }

}
