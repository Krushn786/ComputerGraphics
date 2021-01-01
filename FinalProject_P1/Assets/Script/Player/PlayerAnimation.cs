using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour {

    private Animator animator;
    [SerializeField] float aimingAngleAnimationSensitivity; 

	private PlayerAim playerAim {
		get {
			return GameManager.GetInstance().LocalPlayer.playerAim;
		}
	}

	void Awake () {
        animator = GetComponentInChildren<Animator>();
	}
	
	void Update () {
        animator.SetFloat("Vertical", GameManager.GetInstance().GetInputController().Vertical);
        animator.SetFloat("Horizontal", GameManager.GetInstance().GetInputController().Horizontal);
        animator.SetBool("isRunning", GameManager.GetInstance().GetInputController().Run);
        animator.SetBool("isCrouched", GameManager.GetInstance().GetInputController().Crouch);
		animator.SetFloat("AimAngle", playerAim.GetAngle());// * aimingAngleAnimationSensitivity);
		animator.SetBool("isAiming",
			GameManager.GetInstance().LocalPlayer.PlayerState.WeaponState == PlayerState.EWeaponState.Aiming ||
			GameManager.GetInstance().LocalPlayer.PlayerState.WeaponState == PlayerState.EWeaponState.AimingAndFiring);

			//GameManager.GetInstance().GetInputController().Fire2);
	}
}
