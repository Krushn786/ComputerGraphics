using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter : MonoBehaviour {
    [SerializeField] float rateOfFire;
    [SerializeField] Projectile projectile;
    [SerializeField] Transform hand;
    float nextFireAllowed;
    WeaponReloader reloader;
    [SerializeField] AudioController audioFire;
    [SerializeField] AudioController audioDropBulletToFloor;
    [SerializeField] Transform aimingTarget;

    [HideInInspector]
    public Transform muzzle;

    public bool canFire; //check player for fire. if nextFireAllowed based on rate of fire, return a boolean called this
    public WeaponReloader Reloader
    {
        get
        {
            return reloader;
        }
    }

    private void Awake()
    {
        muzzle = transform.Find("Model/Muzzle");
        reloader = GetComponent<WeaponReloader>();
    }

    public void equip()
    {
        transform.SetParent(hand);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    public void Reload()
    {
        if (reloader == null)
            return;
        reloader.Reload();
    }

    //virtual mean we can override it in the whole project
    public virtual void Fire()
    {
        canFire = false;
        if (Time.time < nextFireAllowed)
            return;
        if (reloader) //if the weapon doesn't have reloader don't check
        {
            if (reloader.IsReloading)
                return;
            if (reloader.ShotsRemainingInClip == 0)
                return;
            reloader.TakeFromClip(1);
        }
        audioFire.Play();
        muzzle.LookAt(aimingTarget); //To projectile the bullets to crosshair
        // create bullet
        Instantiate(projectile, muzzle.position, muzzle.rotation);
        nextFireAllowed = Time.time + rateOfFire;
        canFire = true;
        audioDropBulletToFloor.Play();
    }
}
