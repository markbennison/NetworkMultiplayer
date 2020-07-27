using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerData : NetworkBehaviour
{
    public float MaxHP;
    public float CurrentHP;
    public PlayerManager player;

    // Start is called before the first frame update
    void Start()
    {
        MaxHP = 100;
        CurrentHP = 100;
        foreach (Transform eachChild in transform)
        {
            if (eachChild.name == "Canvas")
            {
                player = eachChild.GetComponent<PlayerManager>();
            }
        }
        //myHPManager = GameObject.FindWithTag("Canvas").GetComponent<HPManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (hasAuthority)
        {
            player.currentHP = CurrentHP;
        }
    }
}
