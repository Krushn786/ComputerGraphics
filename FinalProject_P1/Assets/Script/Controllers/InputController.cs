using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Assign the InputControll to the GameManager
public class InputController : MonoBehaviour {

    public float Vertical;
    public float Horizontal;
    public Vector2 MouseInput;
    public bool Fire1; //for firing
    public bool Fire2; //for aiming
    public bool Reload;
    public bool Run;
    public bool Crouch;
    //public bool Sprint; 
    //in tutorial video there were 4 state for moving: croucing, walking,
    //running, sprinting but in here we dont have sprinting. We will 
    //discuss about if it is nessesery to add or not
    public bool MouseWheelUp; //for changing weapon
    public bool MouseWheelDown;

    void Update() {
        Vertical = Input.GetAxis("Vertical");
        Horizontal = Input.GetAxis("Horizontal");
        MouseInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        Fire1 = Input.GetButton("Fire1");
        Fire2 = Input.GetButton("Fire2");
        Reload = Input.GetKey(KeyCode.R);
        Run = Input.GetKey(KeyCode.LeftShift);
        Crouch = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.C);
        MouseWheelUp = Input.GetAxis("Mouse ScrollWheel") > 0;
        MouseWheelDown = Input.GetAxis("Mouse ScrollWheel") < 0;
    }
}
