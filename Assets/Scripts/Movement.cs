using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    CharacterController characterController;

    public float speed = 6.0f;
    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;

    private float platDistance = 0;

    private Vector3 moveDirection = Vector3.zero;
    private Vector3 startPosition;
    private bool dead = false;

    private GameObject floor;
    private GameObject[] beams;

    void Start()
    {
        beams = GameObject.FindGameObjectsWithTag("pretty");
        floor = GameObject.Find("floor");
        characterController = GetComponent<CharacterController>();
        startPosition = new Vector3(11.5f, 3.06f, 0.46f);
    }

    void Update()
    {
        //so platform only gets longer.
        if (transform.position.z > platDistance) platDistance = Mathf.Abs(transform.position.z);

        foreach(var obj in beams)
        {
            obj.transform.LookAt(new Vector3(transform.position.x, transform.position.y-1.5f, transform.position.z));
        }

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
            
            //change distance of platform based on player position - note that platform will never get smaller
            if(transform.position.z > platDistance) floor.transform.localScale = new Vector3(2, 1, -(transform.position.z+1.5f));
            floor.transform.position = new Vector3(11, 1, 0);
            
            ////Random shit
            //floor.transform.position = new Vector3(transform.position.x, 1, transform.position.z-2);
            //floor.transform.localScale = new Vector3(transform.position.x, 0, transform.position.z);
        }

    }

    //Resetting player to start position when the fall off of a platform.
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
