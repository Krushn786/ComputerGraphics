using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Respawner : MonoBehaviour {

	public void Respawn(GameObject obj, float inSeconds)
    {
        obj.SetActive(false);
        GameManager.GetInstance().GetTimer().add(() => {
            obj.SetActive(true);
        }, inSeconds);
        // also we can define a function and pass it as argument:
        // GameManager.Instance.Timer.add(function, inSeconds);
    }
}
