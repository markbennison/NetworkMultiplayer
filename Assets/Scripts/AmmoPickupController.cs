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
            //CharacterStates characterStates = collidedObject.GetComponent<CharacterStates>();
            //characterStates.CmdIncreaseAmmo(collidedObject.name, ammoAmount);
            //Destroy(this.gameObject);

            CmdPickupAction(collidedObject.gameObject, this.gameObject);
        }
    }

    /* ********** *************** ********** */
    /* ********** SERVER COMMANDS ********** */
    /* ********** *************** ********** */

    [Command]
    public void CmdPickupAction(GameObject player, GameObject pickup)
    {
        //PlayerManager player = GameManager.GetPlayer(playerID);
        //player.gameObject.GetComponent<CharacterStates>().RpcTriggerJump();

        CharacterStates characterStates = player.GetComponent<CharacterStates>();
        characterStates.CmdIncreaseAmmo(player.name, ammoAmount);
        
        RpcPickupAction(pickup);
    }

    /* ********** *************** ********** */
    /* ***** SERVER-TO-CLIENT COMMANDS ***** */
    /* ********** *************** ********** */

    [ClientRpc]
    public void RpcPickupAction(GameObject pickup)
    {
        Destroy(pickup.gameObject);
    }
}