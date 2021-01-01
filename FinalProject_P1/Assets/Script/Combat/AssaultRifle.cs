using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssaultRifle : Shooter { //inherit form Shooter, not MonoBehaviour
    public override void Fire()
    {
        base.Fire();
        if (canFire)
        {
            // we can fire the gun
        }
    }

    void Update()
    {
        if (GameManager.GetInstance().GetInputController().Reload)
        {
            Reload();
        }
    }
}
