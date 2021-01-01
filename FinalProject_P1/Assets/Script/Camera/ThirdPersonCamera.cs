using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ThirdPersonCamera : MonoBehaviour
{
    [SerializeField]
    Vector3 cameraOffset;
    [SerializeField]
    float damping;

    Transform cameraLookTarget;
    Player localPlayer;

    // Start is called before the first frame update
    void Awake()
    {
        GameManager.GetInstance().OnLocalPlayerJoined += ThirdPersonCamera_OnLocalPlayerJoined;// ThirdPersonCamera_OnLocalPlayerJoined;
    }

    private void ThirdPersonCamera_OnLocalPlayerJoined(Player player)
    {
        localPlayer = player;

        cameraLookTarget = localPlayer.transform.Find("cameraLookTarget");
        if (cameraLookTarget == null)
        {
            //print("Inside if");
            cameraLookTarget = localPlayer.transform;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        var rightOffset = localPlayer.transform.right * cameraOffset.x;
        var upOffset = localPlayer.transform.up * cameraOffset.y;
        var forwardOffset = localPlayer.transform.forward * cameraOffset.z;
        var targetPosition = cameraLookTarget.position + rightOffset + upOffset + forwardOffset;

        var targetRotation = Quaternion.LookRotation(cameraLookTarget.position - targetPosition, Vector3.up);

        transform.position = Vector3.Lerp(transform.position, targetPosition, damping * Time.deltaTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, damping * Time.deltaTime);
    }
}


/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour {

    [System.Serializable]
    public class CameraRig
    {
        public Vector3 CameraOffset;
        public float Damping; //control the moving speed of camera
        public float CroucingHeight;
    }

    
    // Use this for initialization
    public Player localPlayer;
    Transform cameraLookTarget;
	void Awake () {
		
		cameraLookTarget = localPlayer.transform.Find("cameraLookTarget");

		if (cameraLookTarget == null)
		{
			cameraLookTarget = localPlayer.transform;
		}
	}

    [SerializeField] CameraRig defaultCameraRig;
    [SerializeField] CameraRig aimingCameraRig;
   
    // Update is called once per frame
    void Update () {
        CameraRig cameraRig = defaultCameraRig;
        if (localPlayer.PlayerState.WeaponState == PlayerState.EWeaponState.Aiming
            || localPlayer.PlayerState.WeaponState == PlayerState.EWeaponState.AimingAndFiring)
            cameraRig = aimingCameraRig;
        Vector3 targetPosition = cameraLookTarget.position + localPlayer.transform.forward * cameraRig.CameraOffset.z +
                                                 localPlayer.transform.up * (cameraRig.CameraOffset.y +
                                                 (GameManager.Instance.LocalPlayer.PlayerState.MoveState == PlayerState.EMoveState.Crouching? cameraRig.CroucingHeight: 0)
                                                 ) +
                                                 localPlayer.transform.right * cameraRig.CameraOffset.x;
        Quaternion targetRotation = Quaternion.LookRotation(cameraLookTarget.position - targetPosition, Vector3.up);

        transform.position = Vector3.Lerp(transform.position, targetPosition, cameraRig.Damping * Time.deltaTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, cameraRig.Damping * Time.deltaTime);

	}
}*/
