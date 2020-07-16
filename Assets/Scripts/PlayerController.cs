using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour
{

    Vector3 velocity;
    Vector3 bestGuessPosition;

    float localLatency; // TODO: Get from PlayerConnectionObject

    float latencySmoothingFactor = 10f;

    public GameObject cam;

    float sensitivity = 2f;
    float jumpDistance = 6f;

    const float SPEED_FLATRATE_WALK = 2f;
    const float SPEED_FLATRATE_JOG = 3f;
    const float SPEED_FLATRATE_RUN = 6f;
    const float RUN_ENERGY_LIMIT = 120f;

    bool haste = false;
    float speed = SPEED_FLATRATE_WALK;
    float runEnergy = RUN_ENERGY_LIMIT;
    float moveForward, moveSideway, rotateX, rotateY, verticalVelocity;

    bool shooting = false;
    float shotCooldown = 0;

    CharacterController characterController;
    Animator characterAnimation;

    void Start()
    {


        characterController = gameObject.GetComponent<CharacterController>();
        //        characterAnimation = gameObject.GetComponentInChildren<Animator>();
        Animator[] characterAnimations;
        characterAnimations = gameObject.GetComponentsInChildren<Animator>();
        foreach (Animator anim in characterAnimations)
        {
            if (anim.gameObject.name == "QuasarGun")
            {
                characterAnimation = anim;
            }
        }

        //characterAnimation = gameObject.GetComponentInChildren<Animator>();
        characterAnimation.SetBool("HasAmmo", true);
    }

    private void Animator()
    {
        throw new NotImplementedException();
    }

    void Update()
    {

        // Verify Player has control of object

        //transform.Translate(velocity * Time.deltaTime);

        if (!hasAuthority)
        {
            bestGuessPosition = bestGuessPosition + (velocity * Time.deltaTime);

            //transform.position = Vector3.Lerp(transform.position, bestGuessPosition, Time.deltaTime * latencySmoothingFactor);

            // Another player, do nothing else?
            return;
        }

        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    this.transform.Translate(0, 1, 0);
        //}

        //if (Input.GetKeyDown(KeyCode.D))
        //{
        //    Destroy(gameObject);
        //}

        //if (Input.GetKeyDown(KeyCode.W))
        //{
        //    velocity = new Vector3(1, 0, 0);

        //    CmdUpdateVelocity(velocity, transform.position);
        //}

        // ***** ************************** ***** //
        // **** DETERMINE SPEED & DIRECTION ***** //
        // ***** ************************** ***** //

        if (Input.GetButtonDown("Fire3"))
        {
            haste = true;
        }

        if (Input.GetButtonUp("Fire3"))
        {
            haste = false;
        }

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


        if (moveForward < -SPEED_FLATRATE_JOG)
        {
            moveForward = -SPEED_FLATRATE_JOG;
        }

        rotateX = Input.GetAxis("Mouse X") * sensitivity;
        rotateY -= Input.GetAxis("Mouse Y") * sensitivity;

        rotateY = Mathf.Clamp(rotateY, -60f, 60f);


        if (characterController.isGrounded)
        {
            //characterAnimation.SetBool("Grounded", true);
            if (Input.GetButtonDown("Jump"))
            {
                verticalVelocity = jumpDistance;
                //characterAnimation.SetBool("Grounded", false);
                characterAnimation.SetTrigger("Jump");
            }
        }

        if (Input.GetButtonDown("Fire1"))
        {
            characterAnimation.SetBool("Shooting", true);
            shooting = true;
        }
        if (Input.GetButtonUp("Fire1"))
        {
            characterAnimation.SetBool("Shooting", false);
            shooting = false;
        }

        // Countdown until next bullet can be fired.
        if (shotCooldown > 0f)
        {
            shotCooldown = shotCooldown - Time.deltaTime;
        }



        Vector3 normalisedMovement = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized;
        Vector3 movement = normalisedMovement * speed;

        //Vector3 movement = new Vector3(moveSideway, verticalVelocity, moveForward);
        transform.Rotate(0, rotateX, 0);
        cam.transform.localRotation = Quaternion.Euler(rotateY, 0, 0);

        Vector3 worldMovement = new Vector3(movement.x, verticalVelocity, movement.z);
        worldMovement = transform.rotation * worldMovement;
        characterController.Move(worldMovement * Time.deltaTime);


        if (movement.z > 0 || movement.z < 0 || movement.x > 0 || movement.x < 0)
        {
            characterAnimation.SetBool("Walking", true);
        }
        else
        {
            characterAnimation.SetBool("Walking", false);
        }
        
    }

    void FixedUpdate()
    {

        if (!characterController.isGrounded)
        {
            verticalVelocity += Physics.gravity.y * Time.deltaTime;
        }

        if (!hasAuthority)
        {
            return;
        }


        if (shooting && shotCooldown <= 0f)
        {
            shotCooldown = 4f * Time.deltaTime;
            Shoot();
        }

    }

    [Client]
    void Shoot()
    {
        
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

            hitObject.collider.SendMessageUpwards("CmdApplyDamage", damage, SendMessageOptions.DontRequireReceiver);
        }
    }


    /* ********** SERVER COMMANDS ********** */

    [Command]
    void CmdUpdateVelocity(Vector3 velocity, Vector3 position)
    {
        transform.position = position;
        this.velocity = velocity;

        //transform.position = position  + (velocity * externalLatency);

        // Update clients
        RpcUpdateVelocity(velocity, position);
    }

    /* ********** ********** */

    /* ********** CLIENT COMMANDS ********** */

    [ClientRpc]
    void RpcUpdateVelocity(Vector3 velocity, Vector3 position)
    {
        // Ignore own objects as should be correct (or run validation)
        if (hasAuthority)
        {
            return;
        }

        this.velocity = velocity;
        bestGuessPosition = position + (velocity * localLatency);
        //transform.position = position;

        //transform.position = position  + (velocity * (localLatency + externalLatency));
    }

    /* ********** ********** */
}

