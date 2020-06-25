using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
public class PlayerConnectionObject : NetworkBehaviour
{
    public GameObject playerFirstPersonPrefab;
    public GameObject playerThirdPersonPrefab;

    public string PlayerName = "Anonymous";
    void Start()
    {
        if (!isLocalPlayer)
        {
            // Another player, do nothing
            return;
        }

        // Spawn This Player
        CmdSpawnMyUnit();
    }

    void Update()
    {

        if (!isLocalPlayer)
        {
            // Another player, do nothing
            return;
        }

        //if (Input.GetKeyDown(KeyCode.S))
        //{
        //    CmdSpawnMyUnit();
        //}

        //if (Input.GetKeyDown(KeyCode.Q))
        //{
        //    string playerName = "Player " + Random.Range(1, 100);

        //    CmdChangePlayerName(playerName);
        //}

    }

    /* ********** SERVER COMMANDS ********** */

    [Command]
    void CmdSpawnMyUnit()
    {
        // Create instance on local machine
        GameObject go = Instantiate(playerFirstPersonPrefab);

        // Spawn instance on server (and to other clients)
        NetworkServer.SpawnWithClientAuthority(go, connectionToClient);
    }

    [Command]
    void CmdChangePlayerName(string playerName)
    {
        PlayerName = playerName;

        // Validate name is suitable?
        // Todo

        // Update clients
        RpcChangePlayerName(playerName);
    }

    /* ********** ********** */

    /* ********** CLIENT COMMANDS ********** */

    [ClientRpc]
    void RpcChangePlayerName(string playerName)
    {
        Debug.Log("Player name changed: " + playerName);
        PlayerName = playerName;
    }

    /* ********** ********** */
}
