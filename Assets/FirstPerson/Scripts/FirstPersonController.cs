using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonController : MonoBehaviour
{
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
    CharacterController characterController;
    Animator characterAnimation;

    void Start()
    {
        characterController = gameObject.GetComponent<CharacterController>();
        characterAnimation = gameObject.GetComponentInChildren<Animator>();
    }

    void Update()
    {
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



        //moveForward = Input.GetAxis("Vertical") * speed;
        //moveSideway = Input.GetAxis("Horizontal") * speed;

        //moveForward = Input.GetAxis("Vertical");
        //moveSideway = Input.GetAxis("Horizontal");


        //Vector3 velocity = new Vector3(moveSideway, verticalVelocity, moveForward);
        //Vector3 velocityNormalised = new Vector3(moveSideway, verticalVelocity, moveForward).normalized * speed;

        

        

        if (moveForward < -SPEED_FLATRATE_JOG)
        {
            moveForward = -SPEED_FLATRATE_JOG;
        }

        rotateX = Input.GetAxis("Mouse X") * sensitivity;
        rotateY -= Input.GetAxis("Mouse Y") * sensitivity;

        rotateY = Mathf.Clamp(rotateY, -60f, 60f);


        if (characterController.isGrounded)
        {
            characterAnimation.SetBool("Grounded", true);
            if (Input.GetButtonDown("Jump"))
            {
                verticalVelocity = jumpDistance;
                characterAnimation.SetBool("Grounded", false);
                characterAnimation.SetTrigger("Jump");
            }
        }

        if (Input.GetButtonDown("Fire1"))
        {
            //characterAnimation.SetBool("Shooting", true);
        }
        else
        {
            //characterAnimation.SetBool("Shooting", false);
        }


        Vector3 normalisedMovement = new Vector3(Input.GetAxis("Horizontal"), 0,  Input.GetAxis("Vertical")).normalized;
        Vector3 movement = normalisedMovement * speed;
        Debug.Log("movement: " + movement);

        //Vector3 movement = new Vector3(moveSideway, verticalVelocity, moveForward);
        transform.Rotate(0, rotateX, 0);
        cam.transform.localRotation = Quaternion.Euler(rotateY, 0, 0);

        Vector3 worldMovement = new Vector3(movement.x, verticalVelocity, movement.z);
        worldMovement = transform.rotation * worldMovement;
        characterController.Move(worldMovement * Time.deltaTime);

        // ***** SET CHARACTER ANIMATIONS ***** //

        //if (moveForward > 0)
        //{
        //    characterAnimation.SetFloat("Speed", speed);
        //}
        //else if(moveForward < 0)
        //{
        //    characterAnimation.SetFloat("Speed", -speed);
        //}
        //else 
        //{
        //    characterAnimation.SetFloat("Speed", 0);
        //}

        //if (moveSideway > 0)
        //{
        //    characterAnimation.SetFloat("SpeedSideways", speed);
        //}
        //else if (moveSideway < 0)
        //{
        //    characterAnimation.SetFloat("SpeedSideways", -speed);
        //}
        //else
        //{
        //    characterAnimation.SetFloat("SpeedSideways", 0);
        //}

        characterAnimation.SetFloat("VelocityX", movement.x);
        characterAnimation.SetFloat("VelocityZ", movement.z);
        


    }

    void FixedUpdate()
    {
        if (!characterController.isGrounded)
        {
            verticalVelocity += Physics.gravity.y * Time.deltaTime;
        }
    }
}


