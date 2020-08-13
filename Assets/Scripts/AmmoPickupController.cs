using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(BoxCollider))]
public class AmmoPickupController : NetworkBehaviour
{
    BoxCollider collider;
    int ammoAmount = 50;
    void Start()
    {
        collider = GetComponent<BoxCollider>();
    }

    private void OnTriggerEnter(Collider collidedObject)
    {
        if (collidedObject.tag == "Player")
        {
            CmdPickupAction(collidedObject.gameObject, this.gameObject);
        }
    }

    /* ********** *************** ********** */
    /* ********** SERVER COMMANDS ********** */
    /* ********** *************** ********** */

    [Command]
    public void CmdPickupAction(GameObject player, GameObject pickup)
    {
        CharacterStates characterStates = player.GetComponent<CharacterStates>();
        characterStates.CmdIncreaseAmmo(player.name, ammoAmount);

        // Hide pickup underground
        transform.position = new Vector3 (0, -10, 0);

        // Comman Clients to also move/hide pickup and return after time
        RpcPickupAction(pickup);

        // Coroutine waits set time to bring pickup back (server)
        StartCoroutine(Respawn(pickup));
    }

    IEnumerator Respawn(GameObject pickup)
    {
        yield return new WaitForSeconds(5f);
        transform.position = Vector3.zero;
    }

    /* ********** *************** ********** */
    /* ***** SERVER-TO-CLIENT COMMANDS ***** */
    /* ********** *************** ********** */

    [ClientRpc]
    public void RpcPickupAction(GameObject pickup)
    {
        // Pull pickup from being hidden underground
        transform.position = new Vector3(0, -10, 0);

        // Coroutine waits set time to bring pickup back (clients)
        StartCoroutine(Respawn(pickup));
    }
}