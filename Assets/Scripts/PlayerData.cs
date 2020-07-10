using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerData : NetworkBehaviour
{
    public float MaxHP;
    public float CurrentHP;
    public HPManager myHPManager;

    // Start is called before the first frame update
    void Start()
    {
        MaxHP = 100;
        CurrentHP = 100;
        foreach (Transform eachChild in transform)
        {
            if (eachChild.name == "Canvas")
            {
                myHPManager = eachChild.GetComponent<HPManager>();
            }
        }
        //myHPManager = GameObject.FindWithTag("Canvas").GetComponent<HPManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (hasAuthority)
        {
            myHPManager.currentHP = CurrentHP;
        }
    }
}
