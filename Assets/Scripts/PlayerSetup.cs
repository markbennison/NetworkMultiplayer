using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(PlayerManager))]
public class PlayerSetup : NetworkBehaviour
{
    [SerializeField]
    Behaviour[] componentsToDisable;

    Camera sceneCamera;

    void Start()
    {
        if (!isLocalPlayer)
        {
            DisableRemoteComponents();
        }
        else
        {
            // Local (this) player
            sceneCamera = Camera.main;
            if (sceneCamera != null)
            {
                sceneCamera.gameObject.SetActive(false);
            }

            // Remove 3PP model for local player (xBot humanoid)
            this.transform.GetChild(1).gameObject.SetActive(false);
        }

        GetComponent<PlayerManager>().Setup();
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        string networkID = GetComponent<NetworkIdentity>().netId.ToString();
        PlayerManager player = GetComponent<PlayerManager>();

        GameManager.RegisterPlayer(networkID, player);
    }

    void DisableRemoteComponents()
    {
        // Remote (other) player
        for (int i = 0; i < componentsToDisable.Length; i++)
        {
            componentsToDisable[i].enabled = false;
        }

        // Remove 1PP model for remote players (Gun with Arms)
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