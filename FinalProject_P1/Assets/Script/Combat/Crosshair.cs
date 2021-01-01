using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Crosshair : MonoBehaviour {

    
    [SerializeField] Texture2D texture;
    [SerializeField] int size;
    [SerializeField] float maxAngle;
    [SerializeField] float minAngle;

    float lookHeight;

    /*public void LookHeight(float value) {
        lookHeight += value;

        if (lookHeight > maxAngle || lookHeight < minAngle) {
            lookHeight -= value;
        }

    }*/

    void OnGUI()
    {
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(transform.position);
        screenPosition.y = Screen.height - screenPosition.y;
        GUI.DrawTexture(new Rect(screenPosition.x, screenPosition.y - lookHeight, size, size), texture);
    }


        /*void OnGUI()
        {
            if (GameManager.GetInstance().LocalPlayer.PlayerState.WeaponState == PlayerState.EWeaponState.Aiming
                || GameManager.GetInstance().LocalPlayer.PlayerState.WeaponState == PlayerState.EWeaponState.AimingAndFiring)
            {
                Vector3 screenPosition = Camera.main.WorldToScreenPoint(transform.position);
                screenPosition.y = Screen.height - screenPosition.y;
                GUI.DrawTexture(new Rect(screenPosition.x - size / 2, screenPosition.y - size / 2, size, size), texture);
                GUI.depth = 0;
            }
        }*/
    }
