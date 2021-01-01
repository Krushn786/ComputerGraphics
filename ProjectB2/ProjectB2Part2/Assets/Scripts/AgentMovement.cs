using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AgentMovement : MonoBehaviour
{
    public Camera cam;
    private string selectableTag = "Player";
    public Material redStop;
    public Material greenGo;
    public SkinnedMeshRenderer myRend;
    

    private NavMeshAgent agent;
    private Animator movement;
    public GameObject player;

    private bool selected;

    private bool moving;
    // Start is called before the first frame update
    void Start()
    {
        movement = GetComponent<Animator>();
        moving = false;
        agent = GetComponent<NavMeshAgent>();
        selected = false;
       myRend.material = redStop;


    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
        ifClicked();
        }   

        if (Input.GetMouseButtonDown(1)) {
            if (selected == true) {
                moveAgent();
            }
        }

        if (moving)
        {
            if (agent.hasPath || agent.remainingDistance > agent.stoppingDistance)
            {
                moving = true;
                movement.SetBool("move", true);
            }
            //!agent.hasPath && !agent.pathPending
            else if (!agent.hasPath && !agent.pathPending)
            {
                moving = false;
                movement.SetBool("move", false);
            }
        }
    }

    private void moveAgent() {
        
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            agent.SetDestination(hit.point);
            moving = true;
        }
    }

    private void ifClicked() {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            if (hit.transform.CompareTag(selectableTag))
            {
                ClickMe();
            }
        }
    }
    public void ClickMe()
    {

        if (!selected)
        {
            myRend.material = greenGo;
            selected = true;
        }
        else
        {
            myRend.material = redStop;
            selected = false;
        }

    }
}
    

    

