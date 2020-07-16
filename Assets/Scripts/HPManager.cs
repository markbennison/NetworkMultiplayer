using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Networking;
public class HPManager : NetworkBehaviour
{
    public GameObject panelCurrentHP;
    private RectTransform lifeBarRectTransform;
    public float currentHP;
    public float maxHP;
    // Start is called before the first frame update
    void Start()
    {
        maxHP = 100;
        currentHP = 100;
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

    [Command]
    void CmdApplyDamage(float damage)
    {
        Debug.Log("SERVER ACKNOWLEDGES DAMAGE: " + damage);
        currentHP -= damage;

        // Update clients
        RpcUpdateHealth(currentHP);

        if (currentHP <= 0.0)
        {
            Invoke("CmdSelfTerminate", 0);
            //Invoke("RpcSelfTerminate", 0);
        }
    }

    [Command]
    void CmdSelfTerminate()
    {
        Destroy(gameObject);
    }


    /* ********** ********** */

    /* ********** CLIENT COMMANDS ********** */

    [ClientRpc]
    void RpcUpdateHealth(float newHP)
    {
        currentHP = newHP;
    }

    [ClientRpc]
    void RpcSelfTerminate()
    {
        Destroy(gameObject);
    }

    /* ********** ********** */


}