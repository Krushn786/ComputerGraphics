using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEditor;

public class AmmoPickUp : PickUpItems {
    [SerializeField] EWeaponsType weaponType;
    [SerializeField] int amount;
    [Tooltip("if don't want to respawn set it -1")][SerializeField] float timeToSpawn;

    public override void OnPickUp(Transform player)
    {
        base.OnPickUp(player);
        var inventory = player.GetComponentInChildren<Container>();
        inventory.PutToContainer(weaponType.ToString(), amount);
        player.GetComponent<PlayerShoot>().ActiveWeapon.Reloader.HandleOnAmmoChanged();
        gameObject.SetActive(false);
        if (timeToSpawn == -1)
            return;
        GameManager.GetInstance().GetTimer().add(() =>
        {
            gameObject.SetActive(true);
        }, timeToSpawn);
    }
}
