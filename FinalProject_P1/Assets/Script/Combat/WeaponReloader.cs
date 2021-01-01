using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponReloader : MonoBehaviour
{
    [SerializeField] int maxAmmo;
    [SerializeField] float reloadTime;
    [SerializeField] int clipSize;
    [SerializeField] Container inventory;
    [SerializeField] AudioController audioReload;
    [SerializeField] EWeaponsType weaponType;

    public int shotsFiredInClip;
    bool isReloading;
    System.Guid containerItemId;

    public event System.Action OnAmmoChanged;

    public int ShotsRemainingInClip
    {
        get
        {
            return clipSize - shotsFiredInClip;
        }
    }

    public int RemaingInClip
    {
        get
        {
            return inventory.GetRemainingAmount(containerItemId);
        }
    }

    void Awake()
    {
        inventory.OnContainerReady += () =>
        {
            containerItemId = inventory.add(weaponType.ToString(), maxAmmo);
        };
    }

    //void Start() 
    //{
    //    containerItemId = inventory.add(this.name, maxAmmo);
    //    //in tutorial videos it was in Awake() instead of Start() but
    //    //there was a problem that it invoked before awake in the inventory
    //    //so items list is null while inventory is invoking add method
    //}

    public bool IsReloading
    {
        get
        {
            return isReloading;
        }
    }

    public void Reload()
    {
        if (isReloading)
            return;
        isReloading = true;
        audioReload.Play();
        GameManager.GetInstance().GetTimer().add(() => {
            ExecuteToReload(inventory.TakeFromContainer(containerItemId, clipSize - ShotsRemainingInClip));
        }, reloadTime);
        
    }

    void ExecuteToReload(int amount)
    {
        isReloading = false;
        shotsFiredInClip -= amount;
        HandleOnAmmoChanged();
    }

    public void TakeFromClip(int amount)
    {
        shotsFiredInClip += amount;
        HandleOnAmmoChanged();
    }

    public void HandleOnAmmoChanged()
    {
        if (OnAmmoChanged != null)
            OnAmmoChanged();
    }
}
