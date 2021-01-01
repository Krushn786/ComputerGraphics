using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBack : MonoBehaviour
{
    public GameObject gameObject;
    bool chek = true;
    void Update()
    {
        if (!gameObject.activeInHierarchy)
        {
            if (chek)
            {
                chek = false;
                StartCoroutine(LateCall());
            }
        }
        if (gameObject.activeInHierarchy)
            chek = true;
    }

    IEnumerator LateCall()
    {
        yield return new WaitForSeconds(10);
        gameObject.SetActive(true);
        //Do Function here...
    }
}

