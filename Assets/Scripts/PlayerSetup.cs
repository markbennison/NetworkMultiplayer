using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(PlayerManager))]
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
            DisableRemoteComponents();
        }
        else
        {
            // Local player
            sceneCamera = Camera.main;
            if (sceneCamera != null)
            {
                sceneCamera.gameObject.SetActive(false);
            }

            //remove 3PP
            this.transform.GetChild(1).gameObject.SetActive(false);
        }

        GetComponent<PlayerManager>().Setup();
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        string networkID = GetComponent<NetworkIdentity>().netId.ToString() ;
        PlayerManager player = GetComponent<PlayerManager>();

        GameManager.RegisterPlayer(networkID, player);
    }

    void DisableRemoteComponents()
    {
        // Remote player
        for (int i = 0; i < componentsToDisable.Length; i++)
        {
            componentsToDisable[i].enabled = false;
        }

        //remove 1PP
        transform.GetChild(0).gameObject.SetActive(false);
    }

    void OnDisable()
    {
        if (sceneCamera != null)
        {
            sceneCamera.gameObject.SetActive(true);
        }

        GameManager.UnregisterPlayer(transform.name);
    }

}
