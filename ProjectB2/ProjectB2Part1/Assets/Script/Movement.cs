using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Movement : MonoBehaviour
{
    //Camera for RayPoint
    public Camera cam;
    //Agent Not Selected Color
    public Material redStop;
    //Agent Selected Color
    public Material greenGo;



    //Capsule Mesh to integrate it
    //public SkinnedMeshRenderer capMesh;
    public MeshRenderer capMesh;

    //Speed of the Agent while movement
    public float speed = 14f;
    private float userSetSpeed;

    //NavMesh Agent
    public Rigidbody rb;
    public NavMeshAgent agent;
    public GameObject agentMan;

    private string agentName = "";

    protected bool selected;

    private bool moving;

    private bool collideWithPlayer;
    private RaycastHit hit;

    //private Rigidbody rb;
    public Animator anim;
    float idlePose = 0.0f;
    float SecondDone = 0.0f;

    public OffMeshLink links;
    private bool running = true;
  

    private bool reached = false;
    // Start is called before the first frame update
    void Start()
    {
        
        //rb = agentMan.GetComponent<Rigidbody>();
        //agent = agentMan.GetComponent<NavMeshAgent>();
        agentName = agent.name;
        moving = false;
        selected = false;
        capMesh.material = redStop;
        userSetSpeed = this.speed;
        //rb = GetComponent<Rigidbody>();
        //collAgentToGround = true;
        //anim = agentMan.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
        //foreach (var n in links)
        //{
            if (agent.isOnOffMeshLink)
            {
            //rb.AddForce(new Vector3(0, 100, 0),ForceMode.Impulse);
                //anim.SetBool("Shift", true);
                anim.SetBool("Jump", true);
            
            }
            else
            {
                anim.SetBool("Jump", false);
            //anim.SetBool("Shift", false);
        }
        //}
        stayIdle();
        //isMoving();
        isReached();
        //print(agent.pathStatus);
        //print(agent.isPathStale);
        //Will call ifClicked that will turn the agent either RED[Can't Move] or GREEN[Can Move]
        if (Input.GetMouseButtonDown(0))
        {
            ifClicked();
        }
        if (Input.GetKeyDown(KeyCode.R) && !running)
        {
            running = true;
            float distAll = Vector3.Distance(rayPointHit(), agent.GetComponent<NavMeshAgent>().transform.position);

            if (distAll > 4 && selected)
            {
                anim.SetBool("Shift", true);
            }

        }
        else if (Input.GetKeyDown(KeyCode.R) && running) {
            running = false;
            anim.SetBool("Shift", false);
        }

        //WILL set Destination of Agent if it's valid!
        if (Input.GetMouseButtonDown(1))
        {
            if (selected == true)
            {
                moveAgent();
            }
        }

        if (moving)
        {
            if (agent.hasPath || agent.remainingDistance > agent.stoppingDistance)
            {
                moving = true;
                reached = false;
                //movement.SetBool("move", true);
            }
            //!agent.hasPath && !agent.pathPending
            else if (!agent.hasPath && !agent.pathPending)
             
                    {
                moving = false;
                reached = true;
                // movement.SetBool("move", false);
            }
        }
    }
    //Get the RayCast and check if it's valid to Move the agent where the person clicked
    //If it is then SET the Destination to the clicked Vector on the World!
    private void moveAgent()
    {

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        

        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            agent.SetDestination(hit.point);
            anim.SetBool("Movement", true);
            anim.SetFloat("WalkAndRun", Input.GetAxis("Horizontal"));
            agent.isStopped = false;

            agent.GetComponent<NavMeshAgent>().speed = speed;
            moving = true;
            
        }
    }

    //Get the RayCast and check if the user clicked the AGENT?
    //If yes go clickedMe(To get it's color change from RED-> GREEN |OR| GREEN -> RED)
    private void ifClicked()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        //RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            if (hit.transform.CompareTag("Player"))//hit.collider.CompareTag("Player"))
            {
                ClickMe();
            }
        }
    }

    //Change the material of a selected item
    public void ClickMe()
    {

        if (!selected)
        {
            //myRend.material = greenGo;
            capMesh.material = greenGo;
            selected = true;
        }
        else
        {
            //myRend.material = redStop;
            capMesh.material = redStop;
            selected = false;
        }

    }

    public  bool isSelected()
    {
        return selected;
    }

    public bool isReached() {
        //print(agent.name + "--" + reached);
        return reached;
    }
    public bool isMoving() {
        return moving;
        
    }
    public Vector3 rayPointHit() {
        return hit.point;
    }
    /*public bool getCollideWithPlayer() {
        return collideWithPlayer;
    }
    public void setCollideWithPlayer(bool canMove) {
        this.collideWithPlayer = canMove;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<NavMeshAgent>().CompareTag("Player")) {
            setCollideWithPlayer(true);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<NavMeshAgent>().CompareTag("Player"))
        {
            setCollideWithPlayer(false);
        }
    }*/
    public void resetSpeed() {
        this.speed = userSetSpeed;
    }
    public void setSpeed(float x)
    {

        speed = x;
    }
    public Vector3 locationAgent() {
        return agent.transform.position;
    }
    private void stayIdle()
    {
        
        if (anim.GetFloat("Blend") <= 1f && SecondDone < 5.0f)
        {
            if (SecondDone < 1.66667f)
            {
                idlePose += (0.1f * Time.deltaTime);
            }
            anim.SetFloat("Blend", (idlePose));
            SecondDone += Time.deltaTime;
        }
        else if (SecondDone >= 2.0f)
        {
            SecondDone = 0.0f;
        }
        else if (idlePose >= 1f)
        {
            idlePose = 0.0f;
            anim.SetFloat("Blend", idlePose);
        }
    }

}
