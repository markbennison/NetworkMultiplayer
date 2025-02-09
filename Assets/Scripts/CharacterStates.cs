﻿using UnityEngine;
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

    // ANTI-HACK SecureInt implementation for ammo
    //[SyncVar]
    //int ammoCount = 0;
    [SyncVar]
    SecureInt ammoCount = new SecureInt();

    bool isShooting = false;
    bool isReloading = false;
    bool isWalking = false;
    bool isRunning = false;
    
    bool melee = false;

    public int AmmoCount {
        get
        {
            return this.ammoCount.Get();
        }
    }

    public bool HasAmmo
    {
        get
        {
            if (AmmoCount > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    void Start()
    {
        if (characterAnimation != null)
        {
            animateCharacter = true;
        }
    }

    /* ********** *************** ********** */
    /* ********** SERVER COMMANDS ********** */
    /* ********** *************** ********** */

    [Command]
    public void CmdTriggerJump(string playerID)
    {
        PlayerManager player = GameManager.GetPlayer(playerID);
        player.gameObject.GetComponent<CharacterStates>().RpcTriggerJump();
    }

    [Command]
    public void CmdTriggerDeath(string playerID)
    {
        PlayerManager player = GameManager.GetPlayer(playerID);
        player.gameObject.GetComponent<CharacterStates>().RpcTriggerDeath();
    }


    [Command]
    public void CmdSetGrounded(string playerID, bool value)
    {
        PlayerManager player = GameManager.GetPlayer(playerID);
        player.gameObject.GetComponent<CharacterStates>().RpcSetGrounded(value);
    }

    [Command]
    public void CmdSetAmmo(string playerID, int value)
    {
        PlayerManager player = GameManager.GetPlayer(playerID);
        player.gameObject.GetComponent<CharacterStates>().RpcSetAmmo(value);
    }

    [Command]
    public void CmdIncreaseAmmo(string playerID, int value)
    {
        PlayerManager player = GameManager.GetPlayer(playerID);
        player.gameObject.GetComponent<CharacterStates>().RpcIncreaseAmmo(value);
    }

    [Command]
    public void CmdDecreaseAmmo(string playerID, int value)
    {
        PlayerManager player = GameManager.GetPlayer(playerID);
        player.gameObject.GetComponent<CharacterStates>().RpcDecreaseAmmo(value);
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

    /* ********** *************** ********** */
    /* ***** SERVER-TO-CLIENT COMMANDS ***** */
    /* ********** *************** ********** */

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
    public void RpcSetAmmo(int value)
    {
        ammoCount.Set(value);
    }

    [ClientRpc]
    public void RpcIncreaseAmmo(int value)
    {
        ammoCount.AddInt(value);
    }

    [ClientRpc]
    public void RpcDecreaseAmmo(int value)
    {
        ammoCount.SubtractInt(value);
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
}