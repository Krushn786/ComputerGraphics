using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ShootNode : Node
{
    private NavMeshAgent agent;
    private EnemyAI ai;
    private Transform target;

    private Vector3 currentVelocity;
    private float smoothDamp;

    private Shooter shooter = new Shooter();

    private float shootingSpeed = 1;
    private float burstDurationMax = 4f;
    private float burstDurationMin =5f;

    private bool shouldFire = false;

    public ShootNode(NavMeshAgent agent, EnemyAI ai, Transform target, Shooter shooter)
    {
        this.agent = agent;
        this.ai = ai;
        this.target = target;
        smoothDamp = 1f;
        this.shooter = shooter;
    }

    public override NodeState Evaluate()
    {
        agent.isStopped = true;
        ai.SetColor(Color.green);
        Vector3 direction = target.position - ai.transform.position;
        Vector3 currentDirection = Vector3.SmoothDamp(ai.transform.forward, direction, ref currentVelocity, smoothDamp);
        Quaternion rotation = Quaternion.LookRotation(currentDirection, Vector3.up);
        ai.transform.rotation = rotation;
        StartBurst();
        ShootAtPlayer();

        return NodeState.RUNNING;
    }

    private void ShootAtPlayer() {
        Debug.Log("Shoot");
        if (!shouldFire)
        {
            return;
        }
        else {
            shooter.Fire();
        }
    }
    
    void StartBurst()
    {
        Debug.Log("Start");
        if (target.GetComponent<Player>().PlayerHealth < 0)
        {
            return;
        }
        
        shouldFire = true;

        GameManager.GetInstance().GetTimer().add(EndBurst, Random.Range(burstDurationMin, burstDurationMax));
    }


    void EndBurst()
    {
        Debug.Log("End");
        shouldFire = false;
        //shooter.canFire = false;

        if (target.GetComponent<Player>().PlayerHealth < 0)
        {
            return;
        }
        CheckReload();

        GameManager.GetInstance().GetTimer().add(StartBurst, shootingSpeed);
    }

    void CheckReload()
    {
        Debug.Log("reloading");
        if (shooter.Reloader.ShotsRemainingInClip == 0)
        {
            shooter.Reload();
        }
    }
}
