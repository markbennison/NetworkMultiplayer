using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonController : MonoBehaviour
{
    public GameObject cam;
    public float speed = 2f;
    public float sensitivity = 2f;
    public float jumpDistance = 5f;
    float moveForward, moveSideway, rotateX, rotateY, verticalVelocity;
    CharacterController characterController;
    Animator characterAnimation;

    void Start()
    {
        characterController = gameObject.GetComponent<CharacterController>();
        characterAnimation = gameObject.GetComponent<Animator>();
    }

    void Update()
    {
        moveForward = Input.GetAxis("Vertical") * speed;
        moveSideway = Input.GetAxis("Horizontal") * speed;

        rotateX = Input.GetAxis("Mouse X") * sensitivity;
        rotateY -= Input.GetAxis("Mouse Y") * sensitivity;

        rotateY = Mathf.Clamp(rotateY, -60f, 60f);

        print(rotateY);

        Vector3 movement = new Vector3(moveSideway, verticalVelocity, moveForward);
        transform.Rotate(0, rotateX, 0);
        cam.transform.localRotation = Quaternion.Euler(rotateY, 0, 0);

        movement = transform.rotation * movement;
        characterController.Move(movement * Time.deltaTime);

        if (characterController.isGrounded)
        {
            if (Input.GetButtonDown("Jump"))
            {
                verticalVelocity = jumpDistance;
            }
        }

        if (Input.GetButtonDown("Fire1"))
        {
            characterAnimation.SetBool("Shooting", true);
        }
        else
        {
            characterAnimation.SetBool("Shooting", false);
        }

        if(characterController.velocity.x > 0 || characterController.velocity.z > 0 || characterController.velocity.x < 0 || characterController.velocity.z < 0)
        {
            characterAnimation.SetFloat("Speed", 1);
        }
        else
        {
            characterAnimation.SetFloat("Speed", 0);
        }
    }

    void FixedUpdate()
    {
        if (!characterController.isGrounded)
        {
            verticalVelocity += Physics.gravity.y * Time.deltaTime;
        }
    }
}
