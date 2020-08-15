using System;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour
{
    public GameObject cam;

    CharacterController characterController;
    Animator gunAnimation;
    CharacterStates characterStates;
    Collider collider;
    string playerid;

    const string PLAYER_TAG = "Player";

    // Consider accounting for latency in movement
    //Vector3 bestGuessPosition;
    //float localLatency; 

    Vector3 velocity;

    float sensitivity = 2f;
    float jumpDistance = 6f;

    const float SPEED_FLATRATE_WALK = 2f;
    const float SPEED_FLATRATE_JOG = 3f;
    const float SPEED_FLATRATE_RUN = 6f;
    const float RUN_ENERGY_LIMIT = 120f;

    bool haste = false;
    float speed = SPEED_FLATRATE_WALK;
    float runEnergy = RUN_ENERGY_LIMIT;
    float rotateX, rotateY, verticalVelocity;

    bool shooting = false;
    float shotCooldown = 0;

    void Start()
    {
        collider = GetComponent<Collider>();
        playerid = collider.name;

        characterStates = gameObject.GetComponent<CharacterStates>();
        characterController = gameObject.GetComponent<CharacterController>();

        // Find Gun Animation controller in child object
        Animator[] animations;
        animations = gameObject.GetComponentsInChildren<Animator>();
        foreach (Animator anim in animations)
        {
            if (anim.gameObject.name == "QuasarGun")
            {
                gunAnimation = anim;
            }
        }

        gunAnimation.SetBool("HasAmmo", characterStates.HasAmmo);
    }

    private void Animator()
    {
        throw new NotImplementedException();
    }

    void Update()
    {
        // Verify Player has control of object
        if (!hasAuthority)
        {
            // Consider accounting for latency in movement
            //bestGuessPosition = bestGuessPosition + (velocity * Time.deltaTime);
            //transform.position = Vector3.Lerp(transform.position, bestGuessPosition, Time.deltaTime * latencySmoothingFactor);

            // Another player, do nothing else?
            return;
        }

        // Player has authority over this object, continue:

        // Determine speed and rotation
        CheckHaste();
        CalculateSpeed();
        CalculateLookRotation();

        // Check if initating a jump, and only if grounded
        CheckJumping();

        // Apply movement based on previous calculations
        ApplyMovement();

        // Check if shooting and apply cooldown for gun (auto)rechambering
        CheckShooting();
        RechamberingGun();
    }

    void FixedUpdate()
    {
        gunAnimation.SetBool("HasAmmo", characterStates.HasAmmo);

        if (!characterController.isGrounded)
        {
            verticalVelocity += Physics.gravity.y * Time.deltaTime;
        }

        if (!hasAuthority)
        {
            return;
        }

        if (characterStates.HasAmmo && shooting && shotCooldown <= 0f)
        {
            shotCooldown = 4f * Time.deltaTime;
            Shoot();
        }
    }

    void CalculateSpeed()
    {
        // Determine travel speed
        if (runEnergy > 0 && haste)
        {
            speed = SPEED_FLATRATE_RUN;
            runEnergy--;
        }
        else if (haste)
        {
            speed = SPEED_FLATRATE_JOG;
        }
        else if (runEnergy < RUN_ENERGY_LIMIT && !haste)
        {
            speed = SPEED_FLATRATE_WALK;
            runEnergy++;
        }
        else
        {
            speed = SPEED_FLATRATE_WALK;
        }
    }

    void CalculateLookRotation()
    {
        rotateX = Input.GetAxis("Mouse X") * sensitivity;
        rotateY -= Input.GetAxis("Mouse Y") * sensitivity;

        rotateY = Mathf.Clamp(rotateY, -60f, 60f);
    }

    void CheckJumping()
    {
        if (characterController.isGrounded)
        {
            characterStates.CmdSetGrounded(playerid, true);
            if (Input.GetButtonDown("Jump"))
            {
                verticalVelocity = jumpDistance;
                characterStates.CmdSetGrounded(playerid, false);
                characterStates.CmdTriggerJump(playerid);
            }
        }
    }

    void CheckHaste()
    {
        if (Input.GetButtonDown("Fire3"))
        {
            haste = true;
        }

        if (Input.GetButtonUp("Fire3"))
        {
            haste = false;
        }
    }

    void CheckShooting()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            gunAnimation.SetBool("Shooting", true);
            shooting = true;
        }
        if (Input.GetButtonUp("Fire1"))
        {
            gunAnimation.SetBool("Shooting", false);
            shooting = false;
        }
    }

    void RechamberingGun()
    {
        // Countdown until next bullet can be fired.
        if (shotCooldown > 0f)
        {
            shotCooldown = shotCooldown - Time.deltaTime;
        }
    }

    void ApplyMovement()
    {
        Vector3 normalisedMovement = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized;
        Vector3 movement = normalisedMovement * speed;

        transform.Rotate(0, rotateX, 0);
        cam.transform.localRotation = Quaternion.Euler(rotateY, 0, 0);

        Vector3 worldMovement = new Vector3(movement.x, verticalVelocity, movement.z);
        worldMovement = transform.rotation * worldMovement;
        characterController.Move(worldMovement * Time.deltaTime);

        characterStates.CmdSetVelocityX(playerid, movement.x);
        characterStates.CmdSetVelocityZ(playerid, movement.z);
    }

    [Client]
    void Shoot()
    {
        characterStates.CmdDecreaseAmmo(playerid, 1);
        RaycastHit hitObject;

        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hitObject, 20))
        {
            Debug.DrawLine(cam.transform.position, hitObject.point, Color.magenta, 0.5f); ;
            Debug.Log("TEST: " + hitObject.distance.ToString());

            float damage = 15 - hitObject.distance;
            if (damage < 1)
            {
                damage = 1;
            }

            if (hitObject.collider.tag == PLAYER_TAG)
            {
                CmdPlayerHit(hitObject.collider.name, damage);
            }
        }
    }

    /* ********** *************** ********** */
    /* ********** SERVER COMMANDS ********** */
    /* ********** *************** ********** */

    [Command]
    void CmdPlayerHit(string playerID, float damage)
    {
        Debug.Log("SERVER ACKNOWLEDGES DAMAGE to " + playerID + ": " + damage);

        PlayerManager player = GameManager.GetPlayer(playerID);

        // Update clients
        player.RpcApplyDamage(damage);
    }

    [Command]
    void CmdUpdateVelocity(Vector3 velocity, Vector3 position)
    {
        transform.position = position;
        this.velocity = velocity;

        //transform.position = position  + (velocity * externalLatency);

        // Update clients
        RpcUpdateVelocity(velocity, position);
    }

    /* ********** *************** ********** */
    /* ***** SERVER-TO-CLIENT COMMANDS ***** */
    /* ********** *************** ********** */

    [ClientRpc]
    void RpcUpdateVelocity(Vector3 velocity, Vector3 position)
    {
        // Ignore own objects as should be correct (or run validation)
        if (hasAuthority)
        {
            return;
        }

        this.velocity = velocity;

        // Consider more lerping between positions to acoutn for latency?
        //bestGuessPosition = position + (velocity * localLatency);
        //transform.position = position;
        //transform.position = position  + (velocity * (localLatency + externalLatency));
    }
}