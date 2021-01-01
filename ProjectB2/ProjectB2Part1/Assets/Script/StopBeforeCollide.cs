using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class StopBeforeCollide : MonoBehaviour
{
    private Movement[] allAgents;
    private List<Movement> greenAgent;
    private List<Movement> agentReached;
    private Movement leader;
    private bool playerGoodToAdd = false;
    private float radiusTostop = 1f;
    private int countToReach = 6;
    private int currentLevel = 1;
    private bool isStoppindDistanceSet = false;
    private float tempSpeed = 0f;


    
    // Start is called before the first frame update
    void Start()
    {
        leader = null;
        allAgents = FindObjectsOfType<Movement>();
        greenAgent = new List<Movement>();
        agentReached = new List<Movement>();
        
    }

    // Update is called once per frame
    void Update()
    {
        //print(allAgents.Length);
        //print(greenAgent.Count);
        

        collectGreenAgent();
        

        isAllAgentReached();
        
    }
    protected void collectGreenAgent() {
        foreach(var n in allAgents) {
            if (n.isSelected())
            {
                addAgent(n);
                
                //print(n.name);
            }
            else {
                removeAgent(n);
            }
        }
        
    }

  
    protected void addAgent(Movement agentAdd) {
        playerGoodToAdd = true;

        foreach (var n in greenAgent.ToArray()) {
            if (n.Equals(agentAdd)) {
                //print("can't add");
                playerGoodToAdd = false;
            }
            agentReached.Clear();
        }
        if (playerGoodToAdd)
        {
            //print("2-stp");
            greenAgent.Add(agentAdd);
        }
        if (greenAgent.Count == 0)
        {
            //print("3-stp");
            greenAgent.Add(agentAdd);
        }
    }
    protected void removeAgent(Movement removeAgent) {

        foreach (var n in greenAgent.ToArray()) {
            if (n.Equals(removeAgent)) {
                greenAgent.Remove(removeAgent);
                if (agentReached.Contains(n)) {
                    agentReached.Remove(removeAgent);
                }
            }
        }
    }
    protected void isAllAgentReached() {
        //print("iem");

        //Vector3 vector = new Vector3();
        float x;
        //float y;
        float z;
        
        foreach (var n in greenAgent) {

            //print(n.isMoving() + "------ismoving" + n.name);
            x = Mathf.Abs(n.rayPointHit().x) - Mathf.Abs(n.GetComponent<NavMeshAgent>().transform.position.x);
            
            z = Mathf.Abs(n.rayPointHit().z) - Mathf.Abs(n.GetComponent<NavMeshAgent>().transform.position.z);

            float xRay = Mathf.Abs(n.rayPointHit().x);
            float zRay = Mathf.Abs(n.rayPointHit().z);
            float currentX = Mathf.Abs(n.GetComponent<NavMeshAgent>().transform.position.x);
            float currentZ = Mathf.Abs(n.GetComponent<NavMeshAgent>().transform.position.z);
            float distAll = Vector3.Distance(n.rayPointHit(), n.GetComponent<NavMeshAgent>().transform.position);
 
            if (distAll <= 2)
            {
                n.setSpeed(2);

            }
            else if (distAll > 2 && distAll <= 4)
            {
                n.setSpeed(5);
                

            }
            else if (distAll > 4 && distAll <= 8)
            {
                n.anim.SetBool("Shift", false);
                n.setSpeed(6);

            }
            else
            {
                n.resetSpeed();
                n.GetComponent<NavMeshAgent>().stoppingDistance = 0f;
            }

            if (agentReached.Count <= 1)
            {
                if ((int)distAll <= 1)
                {

                    if (agentReached.Count == 0)
                    {
                        //print(n.name);
                        agentReached.Add(n);
                        n.GetComponent<NavMeshAgent>().stoppingDistance = 0f;
                        n.anim.SetBool("Movement", false);
                        //anim.SetFloat("WalkAndRun", Input.GetAxis("Horizontal"));
                        //n.GetComponent<NavMeshAgent>().isStopped = true;
                        //print("done");
                    }
                    if (agentReached.Count == 1) {
                        agentReached.Add(n);
                        n.GetComponent<NavMeshAgent>().stoppingDistance = 1f;
                        n.GetComponent<NavMeshAgent>().isStopped = true;
                    }
                }
                else
                {
                    if (agentReached.Count == 1)
                    {
                        print("remove");
                        n.GetComponent<NavMeshAgent>().isStopped = false;
                        agentReached.Remove(n);
                    }
                }
            }
            else if(agentReached.Count >1 && agentReached.Count <3) {
                if ((int)distAll <= 2)
                {
                    if (!agentReached.Contains(n))
                    {
                        n.GetComponent<NavMeshAgent>().stoppingDistance = 2f;
                        n.GetComponent<NavMeshAgent>().isStopped = true;
                        agentReached.Add(n);
                        //print("done");
                    }
                }
                else {
                    n.GetComponent<NavMeshAgent>().isStopped = false;
                    agentReached.Remove(n);
                }
            }
            else if (agentReached.Count >= 3 && agentReached.Count < 4)
            {
                if ((int)distAll <= 3)
                {
                    if (!agentReached.Contains(n))
                    {
                        n.GetComponent<NavMeshAgent>().isStopped = true;
                        n.GetComponent<NavMeshAgent>().stoppingDistance = 3f;
                        agentReached.Add(n);
                    }
                }
                else
                {
                    n.GetComponent<NavMeshAgent>().isStopped = false;
                    agentReached.Remove(n);
                }
            }
            else if (agentReached.Count >= 4 && agentReached.Count < 5)
            {
                if ((int)distAll <= 4)
                {
                    if (!agentReached.Contains(n))
                    {
                        n.GetComponent<NavMeshAgent>().isStopped = true;
                        n.GetComponent<NavMeshAgent>().stoppingDistance = 3f;
                        agentReached.Add(n);
                    }
                }
                else
                {
                    n.GetComponent<NavMeshAgent>().isStopped = false;
                    agentReached.Remove(n);
                }
            }
            else if (agentReached.Count >= 6 && agentReached.Count < 8)
            {
                if ((int)distAll <= 5)
                {
                    if (!agentReached.Contains(n))
                    {
                        n.GetComponent<NavMeshAgent>().isStopped = true;
                        n.GetComponent<NavMeshAgent>().stoppingDistance = 3f;
                        agentReached.Add(n);
                    }
                }
                else
                {
                    n.GetComponent<NavMeshAgent>().isStopped = false;
                    agentReached.Remove(n);
                }
            }
            //print(agentReached.Count +"  " +(int)distAll);
        }

        
        //print("return true");

    }
    private float value() {
        int x = agentReached.ToArray().Length;
        if (x > 12)
        {
            return 5.5f;
        }
        else if (x >= 1 && x <= 4)
        {
            return 2f;
        }
        else if (x > 4 && x < 8) {
            return 2.5f;
        }
        else if (x >= 8 && x < 12)
        {
            return 3.5f;
        }
        else if (x >= 12 && x < 16)
        {
            return 4.5f;
        }
        else
        {
            return 0f;
        }

    }

    private void agentReachedAdd() {
        if (agentReached.Count >= 1) {
            foreach (var n in greenAgent)
            {
                float distance = Vector3.Distance(n.rayPointHit(), n.locationAgent());
                if (!agentReached.Contains(n)) {
                    agentReached.Add(n);
                }
            }
        }
    }

    /* private void keepIT() {
     * 
     * if (agentReached.Count == greenAgent.Count)
            {
                agentReached.RemoveRange(0, agentReached.Count);
            }
            print("agentReached --------------------------------" + agentReached.Count);

            foreach (var m in agentReached) {
                
            }
     * 
     * if (agentReached.Count == 0 && greenAgent.ToArray().Length > 0)
            {
                checkNreached = ((x <= this.value() && z <= this.value()) ? true : false);
                if (checkNreached) {
                    print("insideif");
                    agentReached.Add(n);
                }

                for (int i = 0; i < greenAgent.ToArray().Length; i++) {
                    if (agentReached.Count != 0)
                    {
                        if (!(greenAgent.ToArray()[i].Equals(agentReached.ToArray()[0])))
                        {
                            greenAgent.ToArray()[i].GetComponent<NavMeshAgent>().stoppingDistance = 6f;
                        }
                    }
                }
            }
     * 
     * 
     * 
     * 
     * if (x <= this.value() && z <= this.value())
            
            {    
                foreach (var m in agentReached)
                {
                    if (m.Equals(n))
                    {
                        return;
                    }
                }
                n.GetComponent<NavMeshAgent>().isStopped = true;
                agentReached.Add(n);
                //print(n.name + "  " + "in" + n.GetComponent<NavMeshAgent>().speed);


            }
            if (x > this.value() && z > this.value())
            {
                foreach (var m in agentReached)
                {
                    if (m.Equals(n))
                    {
                        n.GetComponent<NavMeshAgent>().stoppingDistance = 0f;
                        n.GetComponent<NavMeshAgent>().isStopped = false;
                        //print(n.name + "  " + "out" + n.GetComponent<NavMeshAgent>().speed);

                        agentReached.Remove(n);

                        return;
                    }
                }
            }
     * 
     * 
     * 
     * 
         //if (distAll < 2)
         //{ 
         //print("dist " + distAll);
         //print(x + " - " + z + n.GetComponent<NavMeshAgent>().name);
         //print(n.name + "speedBefore " + n.speed);

         //n.setSpeed(2);
         //print(n.name + "speedAfter " + n.speed);
         //}
         // else
         //{
         //print("Insideelse");
         //n.resetSpeed();
         //print(n.name + "speedAfterelse " + n.speed);
         //}
         //print(this.value() +"value");

         if (x <= this.value() && z <= this.value())
         //if (distAll<this.value())
         {

             if (agentReached.Count == 0)
             {
                 agentReached.Add(n);
             }
             //yield return new WaitForSeconds(0.2f);
             //print("aftersecond");

             //int tempTrav = 0;

             //print("return false");
             for (int i = 0; i < agentReached.ToArray().Length; i++)
             {
                 //distAll = Vector3.Distance(n.transform.position, m.transform.position);
                 if (agentReached.ToArray()[i].Equals(n))
                 {
                     return;
                 }
             }
             n.GetComponent<NavMeshAgent>().stoppingDistance = this.value();
             n.GetComponent<NavMeshAgent>().isStopped = true;
             n.setSpeed(2);
             agentReached.Add(n);
             print(n.name + "  " + "in" + n.GetComponent<NavMeshAgent>().speed);
             //print(n.name + "  " + "add");


         }
         if (x > this.value() && z > this.value())
         {
             //n.GetComponent<NavMeshAgent>().isStopped = false;
             foreach (var m in agentReached)
             {
                 if (m.Equals(n))
                 {
                     n.GetComponent<NavMeshAgent>().stoppingDistance = 0f;
                     n.GetComponent<NavMeshAgent>().isStopped = false;
                     print(n.name + "  " + "out" + n.GetComponent<NavMeshAgent>().speed);

                     agentReached.Remove(n);

                     return;
                 }
             }
         }
     }*/
}

