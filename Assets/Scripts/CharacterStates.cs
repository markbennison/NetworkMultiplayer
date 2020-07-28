using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Networking;

public class CharacterStates : NetworkBehaviour
{
    [SerializeField]
    Animator characterAnimation;
    bool animateCharacter = false;

    [SyncVar]
    public bool grounded = true;

    [SyncVar]
    bool hasRifle = false;

    [SyncVar]
    bool isDead = false;
    [SyncVar]
    public float velocityX = 0f;
    [SyncVar]
    public float velocityZ = 0f;


    bool isShooting = false;
    bool isReloading = false;
    bool isWalking = false;
    bool isRunning = false;
    bool hasAmmo = false;
    bool melee = false;


    /* ********** SERVER COMMANDS ********** */
    [Command]
    public void CmdTriggerJump(string playerID)
    {
        PlayerManager player = GameManager.GetPlayer(playerID);
        player.gameObject.GetComponent<CharacterStates>().RpcTriggerJump();
    }

    [Command]
    public void CmdSetGrounded(string playerID, bool value)
    {
        PlayerManager player = GameManager.GetPlayer(playerID);
        player.gameObject.GetComponent<CharacterStates>().RpcSetGrounded(value);
    }

    [Command]
    public void CmdSetVelocityX(string playerID, float value)
    {
        PlayerManager player = GameManager.GetPlayer(playerID);
        player.gameObject.GetComponent<CharacterStates>().RpcSetVelocityX(value);
    }

    [Command]
    public void CmdSetVelocityZ(string playerID, float value)
    {
        PlayerManager player = GameManager.GetPlayer(playerID);
        player.gameObject.GetComponent<CharacterStates>().RpcSetVelocityZ(value);
    }



    /* ********** CLIENT COMMANDS ********** */

    [ClientRpc]
    public void RpcTriggerJump()
    {
        if (animateCharacter)
        {
            characterAnimation.SetTrigger("Jump");
        }
    }

    [ClientRpc]
    public void RpcSetGrounded(bool value)
    {
        grounded = value;
        if (animateCharacter)
        {
            characterAnimation.SetBool("Grounded", value);
        }
    }


    [ClientRpc]
    public void RpcHasRifle(bool value)
    {
        hasRifle = value;
        if (animateCharacter)
        {
            characterAnimation.SetBool("HasRifle", value);
        }

    }

    [ClientRpc]
    public void RpcTriggerDeath()
    {
        if (animateCharacter)
        {
            characterAnimation.SetTrigger("Death");
        }
    }

    [ClientRpc]
    void RpcSetVelocityX(float value)
    {
        velocityX = value;
        if (animateCharacter)
        {
            characterAnimation.SetFloat("VelocityX", value);
        }
    }
    
    [ClientRpc]
    void RpcSetVelocityZ(float value)
    {
        velocityZ = value;
        if (animateCharacter)
        {
            characterAnimation.SetFloat("VelocityZ", value);
        }

    }

    void Start()
    {
        if(characterAnimation != null)
        {
            animateCharacter = true;
        }
    }

}
