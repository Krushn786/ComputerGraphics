using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickUp : PickUpItems
{
    [Tooltip("if don't want to respawn set it -1")] [SerializeField] float timeToSpawn;


    public override void OnPickUp(Transform item)
    {
        base.OnPickUp(item);
        var health = item.GetComponentInChildren<Player>();
        if (health.PlayerHealth > 90)
            health.PlayerHealth = 100;
        else
            health.PlayerHealth += 10;
        gameObject.SetActive(false);
        if (timeToSpawn == -1)
            return;
        GameManager.GetInstance().GetTimer().add(() =>
        {
            gameObject.SetActive(true);
        }, timeToSpawn);
    }
}
