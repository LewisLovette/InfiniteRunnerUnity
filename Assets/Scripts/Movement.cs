using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    CharacterController characterController;

    public float speed = 6.0f;
    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;

    private Vector3 moveDirection = Vector3.zero;
    private Vector3 startPosition;
    private bool dead = false;
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        startPosition = new Vector3(11.5f, 3.06f, 0.46f);
    }

    void Update()
    {
        if (characterController.isGrounded)
        {
            // We are grounded, so recalculate
            // move direction directly from axes

            moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0.0f, Input.GetAxis("Vertical"));
            moveDirection *= speed;

            if (Input.GetButton("Jump"))
            {
                moveDirection.y = jumpSpeed;
            }
        }

        // Apply gravity. Gravity is multiplied by deltaTime twice (once here, and once below
        // when the moveDirection is multiplied by deltaTime). This is because gravity should be applied
        // as an acceleration (ms^-2)
        moveDirection.y -= gravity * Time.deltaTime;

        // Move the controller
        if (!dead)
        {
            characterController.Move(moveDirection * Time.deltaTime);
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        dead = true;
        transform.position = startPosition;
    }
    private void OnTriggerExit(Collider other)
    {
        dead = false;
    }
}
