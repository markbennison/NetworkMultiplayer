using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class PlayerSetup : NetworkBehaviour
{
    [SerializeField]
    Behaviour[] componentsToDisable;

    Camera sceneCamera;

    bool cameraCheck = false;

    // Start is called before the first frame update
    void Start()
    {
        if (!isLocalPlayer)
        {
            // Remote player
            for (int i = 0; i < componentsToDisable.Length; i++)
            {
                componentsToDisable[i].enabled = false;
            }
        }
        else
        {
            // Local player
            sceneCamera = Camera.main;
            if (sceneCamera != null)
            {
                sceneCamera.gameObject.SetActive(false);
            }
        }
    }

    void OnDisable()
    {
        if (sceneCamera != null)
        {
            sceneCamera.gameObject.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //if (!cameraCheck)
        //{
        //    RpcRemoveOtherCameras();
        //    cameraCheck = true;
        //}
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
