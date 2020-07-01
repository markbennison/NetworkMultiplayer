using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class PlayerControlSwitch : NetworkBehaviour
{

    bool cameraCheck = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!cameraCheck)
        {
            RpcRemoveOtherCameras();
            cameraCheck = true;
        }
    }


    /* ********** CLIENT COMMANDS ********** */

    [ClientRpc]
    void RpcRemoveOtherCameras()
    {
        if (hasAuthority)
        {
            this.transform.GetChild(1).gameObject.SetActive(false);
            Debug.Log("3PP Deleted");
            return;
        }

        this.transform.GetChild(0).gameObject.SetActive(false);

        Debug.Log("Camera Deleted");
    }

    /* ********** ********** */
}
