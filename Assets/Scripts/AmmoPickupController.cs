using System.Collections;
using System.Collections.Generic;
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

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider collidedObject)
    {
        if (collidedObject.tag == "Player")
        {
            Debug.Log("PICKUP: " + collidedObject.name);
            CharacterStates characterStates = collidedObject.GetComponent<CharacterStates>();
            characterStates.CmdIncreaseAmmo(collidedObject.name, ammoAmount);
            Destroy(this.gameObject);
        }
    }

}
