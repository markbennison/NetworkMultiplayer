using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
public class PlayerObject : NetworkBehaviour
{
    public GameObject playerUnitPrefab;

    void Start()
    {
        if (!isLocalPlayer)
        {
            // Another player, do nothing
            return;
        }

        // Create instance on local machine
        Instantiate(playerUnitPrefab);
    }

    void Update()
    {
        
    }
}
