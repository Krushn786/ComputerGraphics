using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShoot : MonoBehaviour {

    [SerializeField] float timeToSwichWeapon;
    Shooter[] weapons;
    Shooter activeWeapon;
    int activeWeaponIndex = 0;
    bool canFire;
    Transform weaponsHolster;
    public event System.Action<Shooter> OnWeaponSwich;

    public Shooter ActiveWeapon
    {
        get
        {
            return activeWeapon;
        }
    }

    void Awake()
    {
        weaponsHolster = transform.Find("Weapons");
        weapons = weaponsHolster.GetComponentsInChildren<Shooter>();
        if(weapons.Length > 0)
            equip(activeWeaponIndex);
    }

    void SwichWeapon(int direction)
    {
        canFire = false;
        activeWeaponIndex += direction;
        if (activeWeaponIndex > weapons.Length - 1)
            activeWeaponIndex = 0;
        else if (activeWeaponIndex < 0)
            activeWeaponIndex = weapons.Length - 1;
        GameManager.GetInstance().GetTimer().add(() => {
            equip(activeWeaponIndex);
        }, timeToSwichWeapon);
    }

    void deactivateWeapons()
    {
        for (int i = 0; i < weapons.Length; i++)
        {
            weapons[i].transform.SetParent(weaponsHolster);
            weapons[i].gameObject.SetActive(false);
        }
    }

    void equip(int index)
    {
        canFire = true;
        activeWeapon = weapons[index];
        deactivateWeapons();
        activeWeapon.gameObject.SetActive(true);
        activeWeapon.equip();
        if(OnWeaponSwich != null)
            OnWeaponSwich(activeWeapon);
    }

    void Update()
    {
        if (GameManager.GetInstance().GetInputController().MouseWheelUp)
            SwichWeapon(1);
        if (GameManager.GetInstance().GetInputController().MouseWheelDown)
            SwichWeapon(-1);
        if (!canFire)
            return;
        if (GameManager.GetInstance().GetInputController().Fire1) {
            activeWeapon.Fire();
        }
    }
}
