using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Networking;
public class PlayerManager : NetworkBehaviour
{
    public GameObject panelCurrentHP;
    private RectTransform lifeBarRectTransform;

    [SyncVar]
    public float currentHP;
    private float maxHP = 100;
    // Start is called before the first frame update
    void Start()
    {
        SetDefaults();
        //if (lifeBarRectTransform != null)
        //{
            lifeBarRectTransform = (RectTransform)panelCurrentHP.transform;
        //}
    }

    // Update is called once per frame
    void Update()
    {
        //if (lifeBarRectTransform != null)
        //{
            lifeBarRectTransform.sizeDelta = new Vector2(currentHP, lifeBarRectTransform.sizeDelta.y);
        //} 
    }

    [ClientRpc]
    public void RpcApplyDamage(float amount)
    {
        currentHP -= amount;

        Debug.Log(transform.name + " health: " + currentHP);
    }

    public void SetDefaults()
    {
        currentHP = maxHP;
    }

    //void ApplyDamage(float damage)
    //{
    //    if (currentHP <= 0.0)
    //    {
    //        return;
    //    }

    //    currentHP -= damage;

    //    if (currentHP <= 0.0)
    //    {
    //        Invoke("SelfTerminate", 0);
    //    }
    //}

    //void SelfTerminate()
    //{
    //    Destroy(gameObject);
    //}

    /* ********** SERVER COMMANDS ********** */

    //[Command]
    //void CmdApplyDamage(float damage)
    //{
    //    string playerID = "<player placeholder>";
    //    Debug.Log("SERVER ACKNOWLEDGES DAMAGE to " + playerID + ": " + damage);

    //    // Update clients
    //    RpcUpdateHealth(damage);


    //}


    [Command]
    void CmdSelfTerminate()
    {
        Destroy(gameObject);
    }


    /* ********** ********** */

    /* ********** CLIENT COMMANDS ********** */

    [ClientRpc]
    void RpcUpdateHealth(float damage)
    {
        Debug.Log("RpcUpdateHealth " + damage);
        currentHP -= damage;

        if (currentHP <= 0.0)
        {
            //Invoke("CmdSelfTerminate", 0);
            //Invoke("RpcSelfTerminate", 0);
        }
    }

    [ClientRpc]
    void RpcSelfTerminate()
    {
        Destroy(gameObject);
    }

    /* ********** ********** */


}