using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState : MonoBehaviour {

    public enum EMoveState
    {
        Walking, Running, Crouching
    }
	
    public enum EWeaponState
    {
        Idle, Aiming, Firing, AimingAndFiring//, Reloading 
    }

    public EMoveState MoveState;
    public EWeaponState WeaponState;

    InputController inputController;
    InputController InputController
    {
        get
        {
            if (inputController == null)
                inputController = GameManager.GetInstance().GetInputController();
            return inputController;
        }
    }

    void SetWeaponState()
    {
        WeaponState = EWeaponState.Idle;
        if (InputController.Fire1)
        {
            WeaponState = EWeaponState.Firing;
            if(InputController.Fire2)
            {
                WeaponState = EWeaponState.AimingAndFiring;
                return;
            }
        }
        if (InputController.Fire2)
            WeaponState = EWeaponState.Aiming;
    }

    void SetMoveState()
    {
        MoveState = EMoveState.Walking;
        if (InputController.Crouch)
            MoveState = EMoveState.Crouching;
        if (InputController.Run)
            MoveState = EMoveState.Running;
    }

	void Update () {
        SetMoveState();
        SetWeaponState();
	}
}
