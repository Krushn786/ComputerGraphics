using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotation : MonoBehaviour
{
    public Camera cam;
    public Transform target;

    private Vector3 previousPosition;
   

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(2))
        {
            previousPosition = cam.ScreenToViewportPoint(Input.mousePosition);
        }
        if (Input.GetMouseButton(2))
        {
            Vector3 direction = previousPosition - cam.ScreenToViewportPoint(Input.mousePosition);
            cam.transform.position = target.position;//new Vector3();

            cam.transform.Rotate(new Vector3(1, 0, 0), direction.y * 180);
            cam.transform.Rotate(new Vector3(0, 1, 0), -direction.x * 180, Space.World);
            //cam.transform.Translate(new Vector3(0, 0, -10));

            previousPosition = cam.ScreenToViewportPoint(Input.mousePosition);
        }

    }
}
