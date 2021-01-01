using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AmmoCounter : MonoBehaviour {
    [SerializeField] Text text;
    PlayerShoot playerShoot;

	void Awake() {
        GameManager.GetInstance().OnLocalPlayerJoined += HandleOnLocalPlayerJoined;
    }
	
    void HandleOnLocalPlayerJoined(Player player)
    {
        playerShoot = player.gameObject.GetComponent<PlayerShoot>();
        playerShoot.OnWeaponSwich += HandleOnWeaponSwich;
        playerShoot.ActiveWeapon.Reloader.OnAmmoChanged += HandleOnAmmoChanged;
        HandleOnAmmoChanged();
    }

    void HandleOnWeaponSwich(Shooter shooter)
    {
        shooter.Reloader.OnAmmoChanged += HandleOnAmmoChanged;
        HandleOnAmmoChanged();
    }

    void HandleOnAmmoChanged()
    {
        text.text = playerShoot.ActiveWeapon.Reloader.ShotsRemainingInClip.ToString() + 
            " / " + playerShoot.ActiveWeapon.Reloader.RemaingInClip.ToString();
    }

    // Update is called once per frame
    void Update () {
		
	}
}
