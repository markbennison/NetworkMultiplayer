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
            CharacterStates characterStates = collidedObject.GetComponent<CharacterStates>();
            characterStates.CmdIncreaseAmmo(collidedObject.name, ammoAmount);
            Destroy(this.gameObject);
        }
    }
}