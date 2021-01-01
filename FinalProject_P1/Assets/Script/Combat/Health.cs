using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : Destructible {

    [SerializeField] private float timeToRespawn;

    public override void Die()
    {
        base.Die();
        GameManager.GetInstance().GetRespawner().Respawn(gameObject, timeToRespawn);
    }

    void OnEnable()
    {
        Reset();
    }

    public override void TakeDamage(float damageAmount)
    {
        base.TakeDamage(damageAmount);
        print("ramaining damage = " + GetHitPointsRemaining());
    }
}
