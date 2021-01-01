using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MoveObstacle : MonoBehaviour
{
    public Camera cam;
    protected bool selected;
    private MeshRenderer obsMesh;
    //Agent Not Selected Color
    public Material cannotMove;
    //Agent Selected Color
    public Material canMove;

    private NavMeshObstacle obstacle;
    public float speed=10f;
    // Start is called before the first frame update
    void Start()
    {
        obstacle = this.gameObject.GetComponent<NavMeshObstacle>();
        obsMesh = this.gameObject.GetComponent<MeshRenderer>();
        obsMesh.material = cannotMove;
        selected = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ifClicked();
        }

        if (selected) {
            control();
        }
    }

    private void control() {
        if (Input.GetKey(KeyCode.DownArrow))
        {
            gameObject.transform.Translate(Vector3.up * Time.deltaTime * speed);
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            gameObject.transform.Translate(Vector3.down * Time.deltaTime * speed);
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            gameObject.transform.Translate(Vector3.right * Time.deltaTime * speed);
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            gameObject.transform.Translate(Vector3.left * Time.deltaTime * speed);
        }
        if (Input.GetKey(KeyCode.N))
        {
            gameObject.transform.Translate(Vector3.forward * Time.deltaTime * speed);
        }
        if (Input.GetKey(KeyCode.M))
        {
            gameObject.transform.Translate(Vector3.back * Time.deltaTime * speed);
        }
    }

    private void ifClicked()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            if (hit.collider.name.Equals(obstacle.name))
            {
                ClickMe();
            }
        }
    }
    public void ClickMe()
    {

        if (!selected)
        {
            //myRend.material = greenGo;
            obsMesh.material = canMove;
            selected = true;
        }
        else
        {
            //myRend.material = redStop;
            obsMesh.material = cannotMove;
            selected = false;
        }

    }
}
