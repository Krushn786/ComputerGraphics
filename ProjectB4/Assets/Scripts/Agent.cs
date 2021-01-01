using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class Agent : MonoBehaviour
{
    public float radius;
    public float mass;
    public float perceptionRadius;

    private List<Vector3> path;
    private NavMeshAgent nma;
    private Rigidbody rb;

    private HashSet<GameObject> perceivedNeighbors = new HashSet<GameObject>();
    private HashSet<GameObject> adjacentWalls = new HashSet<GameObject>();
    public GameObject agentPrefab;


    void Start()
    {
        path = new List<Vector3>();
        nma = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();

        gameObject.transform.localScale = new Vector3(2 * radius, 1, 2 * radius);
        nma.radius = radius;
        rb.mass = mass;
        GetComponent<SphereCollider>().radius = perceptionRadius / 2;
    }

    private void Update()
    {
        if (path.Count > 1 && Vector3.Distance(transform.position, path[0]) < 1.1f)
        {
            path.RemoveAt(0);
        } else if (path.Count == 1 && Vector3.Distance(transform.position, path[0]) < 2f)
        {
            path.RemoveAt(0);

            if (path.Count == 0)
            {
                gameObject.SetActive(false);
                AgentManager.RemoveAgent(gameObject);
            }
        }

        #region Visualization

        if (true)
        {
            if (path.Count > 0)
            {
                Debug.DrawLine(transform.position, path[0], Color.green);
            }
            for (int i = 0; i < path.Count - 1; i++)
            {
                Debug.DrawLine(path[i], path[i + 1], Color.yellow);
            }
        }

        if (true)
        {
            foreach (var neighbor in perceivedNeighbors)
            {
                Debug.DrawLine(transform.position, neighbor.transform.position, Color.yellow);
            }
        }

        #endregion
    }

    #region Public Functions

    public void ComputePath(Vector3 destination)
    {
        nma.enabled = true;
        var nmPath = new NavMeshPath();
        nma.CalculatePath(destination, nmPath);
        path = nmPath.corners.Skip(1).ToList();
        //path = new List<Vector3>() { destination };
        //nma.SetDestination(destination);
        nma.enabled = false;
    }

    public Vector3 GetVelocity()
    {
        return rb.velocity;
    }

    #endregion

    #region Incomplete Functions

private Vector3 ComputeForce()
    {
        //SocialForce//
        //To use this comment (Spiral Force), (Wall Follower Force), (Leader Force) and (Crowd Force)//
        var force = CalculateGoalForce() + CalculateAgentForce() + CalculateWallForce();

        //SpiralForce//
        //To use this comment (Social Force), (Wall Follower Force), (Leader Force) and (Crowd Force)//
        /*  Make Sure to DESTORY ALL "Walls" game object in AgentManager */

        //var force = CalculateSpiralForce() + CalculateAgentForce() + CalculateWallForce();

        //Wall Follower Force//
        //To use this comment (Social Force), (Spiral Force), (Leader Force) and (Crowd Force)//

        //var force = CalculateWallFollowForce() + CalculateAgentForce();


        //Leader Force//
        //To use this comment (Social Force), (Spiral Force), (Wall Follower Force) and (Crowd Force)//

        //var force =  CalculateLeaderForce() + CalculateAgentForce() + CalculateWallForce();

        //Crowd Force//
        //To use this comment (Social Force), (Spiral Force), (Wall Follower Force) and (Leader Force)//

        //var force = CalculateCrowdFollowingForce();


        if (force != Vector3.zero)
{
    return force.normalized * Mathf.Min(force.magnitude, Parameters.maxSpeed);
}
else
{
    return Vector3.zero;
}
}

    #region Social Force


private Vector3 CalculateGoalForce()
{
if (path.Count == 0)
{
    return Vector3.zero;
}


var tempDistanceSelftoNeigbor = path[0] - transform.position;
var desiredVelocity = tempDistanceSelftoNeigbor.normalized * Parameters.DESIREDSPEED;
var actualVelocity = rb.velocity;
return mass * (desiredVelocity - actualVelocity) / Parameters.T;
}

private Vector3 CalculateAgentForce()
{
var agentForce = Vector3.zero;

foreach (var n in perceivedNeighbors)
{
    if (AgentManager.IsAgent(n))
    {

        var neigAgent = AgentManager.agentsObjs[n];
        var directionVector = (transform.position - neigAgent.transform.position).normalized;
        var RminusD = (radius + neigAgent.radius) - Vector3.Distance(transform.position, n.transform.position);
        var tangentialVector = Vector3.Cross(Vector3.up, directionVector);

        var psychologialForce = Parameters.A * Mathf.Exp(RminusD / Parameters.B) * directionVector;
        var nonPenetractionForce =  Parameters.k * (RminusD > 0f ? 1 : 0) * directionVector;
        var slidingForce = Parameters.Kappa * (RminusD > 0f ? RminusD : 0) * Vector3.Dot(rb.velocity - neigAgent.GetVelocity(), tangentialVector) * tangentialVector;

        agentForce = psychologialForce + nonPenetractionForce + slidingForce;
    }
}

return agentForce;
}

private Vector3 CalculateWallForce()
{
var calWalForcetoAgent = Vector3.zero;

foreach (var wall in adjacentWalls)
{
    var calCenterofWall = wall.transform.position;
    var getPosition = transform.position;

    #region Compute Normal

    var normalVector = getPosition - calCenterofWall;
            normalVector.y = 0;

    if (Mathf.Abs(normalVector.x) > Mathf.Abs(normalVector.z))
    {
        normalVector.z = 0;
    }
    else
    {
        normalVector.x = 0;
    }
    normalVector.Normalize();

    #endregion

    var calculateDirection = (getPosition - calCenterofWall);
            calculateDirection.y = 0;
    var agentToWallProj = Vector3.Project(calculateDirection, normalVector);
    var RminusD = (radius + 0.12f) - agentToWallProj.magnitude;

    calWalForcetoAgent += Parameters.WALL_A * Mathf.Exp(RminusD / Parameters.WALL_B) * normalVector;
    calWalForcetoAgent += Parameters.WALL_k * (RminusD > 0f ? 1 : 0) * calculateDirection;

    var tangentBetweenWallAgent = Vector3.Cross(Vector3.up, normalVector);
    calWalForcetoAgent += Parameters.WALL_Kappa * (RminusD > 0f ? RminusD : 0) * Vector3.Dot(rb.velocity, tangentBetweenWallAgent) * tangentBetweenWallAgent;
}

return calWalForcetoAgent;
}

    #endregion

public void ApplyForce()
{
var force = ComputeForce();
force.y = 0;

rb.AddForce(force / mass, ForceMode.Acceleration);
}

    #region Collision Check Work

public void OnTriggerEnter(Collider other)
{
if (AgentManager.IsAgent(other.gameObject))
{
    perceivedNeighbors.Add(other.gameObject);
}
if (WallManager.IsWall(other.gameObject))
{
    adjacentWalls.Add(other.gameObject);
}
}

public void OnTriggerExit(Collider other)
{
if (perceivedNeighbors.Contains(other.gameObject))
{
    perceivedNeighbors.Remove(other.gameObject);
}
if (adjacentWalls.Contains(other.gameObject))
{
    adjacentWalls.Remove(other.gameObject);
}
}

public void OnCollisionEnter(Collision collision)
{
        if (collision.gameObject.tag == "wall")
        {
            lastWallCol = collision.gameObject;
        }

        if (WallManager.IsWall(collision.gameObject))
        {
            //lastCollidedWall = collision.gameObject;
            CalculateWallForce();
            //print(WallForce);
        }
    }

public void OnCollisionExit(Collision collision)
{

}

    #endregion

    #region Single Agent Behavior
    private Vector3 CalculateSpiralForce()
{

Vector3 findCenter = Vector3.zero - new Vector3(transform.position.x, 0, transform.position.z);

Vector3 computeTangent = Vector3.Cross(findCenter, Vector3.up);

Vector3 newForce = computeTangent;
newForce += findCenter;

Debug.DrawLine(transform.position, transform.position + computeTangent, Color.magenta);
Debug.DrawLine(transform.position, transform.position + findCenter, Color.green);

return newForce * mass;
}

GameObject lastWallCol = null;

private Vector3 CalculateWallFollowForce()
{
        
        if (lastWallCol == null)
        {
            return Vector3.zero;
        }

        Vector3 sepAgentfromWall = transform.position - lastWallCol.transform.position, direction = new Vector3(0, 0, 0);

        

        int pushForceIN = 0, pushForceForward = 2;

        float xAxis = sepAgentfromWall.x * lastWallCol.transform.localScale.z;
        float zAxis = sepAgentfromWall.z * lastWallCol.transform.localScale.x;



        if (zAxis >= Mathf.Abs(xAxis))
        {
            direction = new Vector3(pushForceForward, 0, -1 * pushForceIN);
        }
        else if (xAxis > Mathf.Abs(zAxis))
        {
            direction = new Vector3(-1 * pushForceIN, 0, -1 * pushForceForward);
        }
        else if (zAxis < -1 * Mathf.Abs(xAxis))
        {
            direction = new Vector3(-1 * pushForceForward, 0, pushForceIN);
        }
        else if (xAxis < -1 * Mathf.Abs(zAxis))
        {
            direction = new Vector3(pushForceIN, 0, pushForceForward);
        }

        direction = direction.normalized;


        //don't apply force if already moving in the right direction
        float directionalVelocity = Vector3.Dot(rb.velocity, direction);
        if (directionalVelocity > 0.6f)
        {
            direction = Vector3.zero;
        }

        //add inward force
        Vector3 inF = sepAgentfromWall.normalized * -0.1f;

        //don not add inward force if velocity is stopped.
        Vector3 finalForce = direction;
        if (rb.velocity.magnitude > 0.01f)
            finalForce += inF;
        else
            finalForce = -10 * inF;

        if (true)
        {
            Debug.DrawLine(transform.position, lastWallCol.transform.position, Color.red);
            Debug.DrawLine(transform.position, transform.position + finalForce * 5, Color.green);
        }

        return mass * (finalForce + inF);
    }

    #endregion

    #region Group Behaviors

    private Vector3 CalculateCrowdFollowingForce()
    {
        var panicAlert = 0.5f;
        var goalSetUp = ((1 - panicAlert) * (path[0] - transform.position));

        var neigVelCalculated = Vector3.zero;
        foreach (var n in perceivedNeighbors)
        {
            neigVelCalculated += ((path[0] - transform.position) * Mathf.Min((path[0] - transform.position).magnitude, 1));
        }

        neigVelCalculated = neigVelCalculated / perceivedNeighbors.Count;
        goalSetUp = (goalSetUp + panicAlert * neigVelCalculated).normalized;

        var forceSet = (((goalSetUp * Mathf.Min(goalSetUp.magnitude, 1)) - rb.velocity) / Parameters.T);
        var npForce = forceSet + CalculateAgentForce() + CalculateWallForce();

        if (npForce != Vector3.zero)
        {
            return npForce.normalized * Mathf.Min(npForce.magnitude, Parameters.maxSpeed);
        }
        else
        {
            return Vector3.zero;
        }

    }

    public Agent Alpha;
    bool hasAlpha = false, isAlpha = false;

    public void setLeader(Agent alpha)
    {
            hasAlpha = true;
            Alpha = alpha;
    }
    public void removeLeader()
    {
            hasAlpha = true;
    }
    public void makeLeader()
    {
            isAlpha = true;
    }
    private Vector3 CalculateLeaderForce()
    {
        if (!hasAlpha)
        {
            return Vector3.zero;
        }

        Vector3 differenceInPostion = transform.position - Alpha.transform.position;


        
        float distanceCalculated = Mathf.Abs(differenceInPostion.magnitude) - radius - Alpha.radius;
        float forceCalculated = 2 / distanceCalculated - 1;

        
        if (Vector3.Dot(rb.velocity, differenceInPostion.normalized) > 0.7f && forceCalculated > 0)
        {
            return Vector3.zero;
        }
        if (Vector3.Dot(rb.velocity, differenceInPostion.normalized) < -0.7f && forceCalculated < 0)
        {
            return Vector3.zero;
        }

        return differenceInPostion.normalized * mass * forceCalculated;
    }
    #endregion





    #endregion
}
