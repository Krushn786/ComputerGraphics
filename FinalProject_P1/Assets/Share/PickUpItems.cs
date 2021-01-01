using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpItems : MonoBehaviour {

	void OnTriggerEnter(Collider other) {
        if (other.tag != "Player")
            return;
        PickUp(other.transform);
    }

    void PickUp(Transform player)
    {
        OnPickUp(player);
    }

    public virtual void OnPickUp(Transform item)
    {
        //PickUp
    }
}
