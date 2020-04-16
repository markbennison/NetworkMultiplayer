using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour
{

    Vector3 velocity;
    Vector3 bestGuessPosition;

    float localLatency; // TODO: Get from PlayerConnectionObject

    float latencySmoothingFactor = 10f;

    void Start()
    {
        
    }

    
    void Update()
    {
        // Verify Player has control of object

        transform.Translate(velocity * Time.deltaTime);

        if (!hasAuthority)
        {
            bestGuessPosition = bestGuessPosition + (velocity * Time.deltaTime);

            transform.position = Vector3.Lerp(transform.position, bestGuessPosition, Time.deltaTime * latencySmoothingFactor);

            // Another player, do nothing else?
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            this.transform.Translate(0, 1, 0);
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            Destroy(gameObject);
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            velocity = new Vector3(1, 0, 0);

            CmdUpdateVelocity(velocity, transform.position);
        }

    }

    /* ********** SERVER COMMANDS ********** */

    [Command]
    void CmdUpdateVelocity(Vector3 velocity, Vector3 position)
    {
        transform.position = position;
        this.velocity = velocity;

        //transform.position = position  + (velocity * externalLatency);

        // Update clients
        RpcUpdateVelocity(velocity, position);
    }

    /* ********** ********** */

    /* ********** CLIENT COMMANDS ********** */

    [ClientRpc]
    void RpcUpdateVelocity(Vector3 velocity, Vector3 position)
    {
        // Ignore own objects as should be correct (or run validation)
        if (hasAuthority)
        {
            return;
        }

        this.velocity = velocity;
        bestGuessPosition = position + (velocity * localLatency);
        //transform.position = position;


        //transform.position = position  + (velocity * (localLatency + externalLatency));
    }

    /* ********** ********** */
}

