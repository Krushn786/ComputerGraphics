using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmorPickUp : PickUpItems
{
    [Tooltip("if don't want to respawn set it -1")] [SerializeField] float timeToSpawn;


    public override void OnPickUp(Transform item)
    {
        base.OnPickUp(item);
        var armor = item.GetComponentInChildren<Player>();
        if (armor.Armor <= 0)
            armor.Armor = 30;
        else if (armor.Armor > 0 && armor.Armor < 90)
            armor.Armor += 10;
        else
            armor.Armor = 100;

        gameObject.SetActive(false);
        if (timeToSpawn == -1)
            return;
        GameManager.GetInstance().GetTimer().add(() =>
        {
            gameObject.SetActive(true);
        }, timeToSpawn);
    }
}
