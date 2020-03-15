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

    //stores z val
    private int distance = 0;
    private float zSize;
    private GameObject[] beams;
    private GameObject[] floors;
    //holds all floors and keeps track of current
    private Queue<GameObject> current = new Queue<GameObject>();

    void Start()
    {
        beams = GameObject.FindGameObjectsWithTag("pretty");
        
        //setting up quque for floor
        floors = GameObject.FindGameObjectsWithTag("floor");
        Debug.Log(floors[0].name + floors[2].name + floors[2].name);
        foreach(var obj in floors)
        {
            current.Enqueue(obj);
        }
        zSize = Mathf.Abs(current.Peek().transform.localScale.z);

        //tempFloor = GameObject.Find("floor");
        //current.Enqueue(tempFloor);

        //tempFloor = GameObject.Find("floor (1)");
        //current.Enqueue(tempFloor);

        //tempFloor = GameObject.Find("floor (2)");
        //current.Enqueue(tempFloor);

        GameObject.Find("floor");

        characterController = GetComponent<CharacterController>();
        startPosition = new Vector3(0, 2.5f, 0);

        //Spawn platform underneath player 
        //TODO: (currently hard-coded)
        current.Peek().transform.localScale = new Vector3(2, 1, -(transform.position.z + 1.5f));
        current.Peek().transform.position = new Vector3(-1, 1, 0);

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
            
            //go in steps of 1.5 on z axis
            if(transform.position.z > (distance * zSize))
            {
                distance++;
                GameObject oldFloor = current.Peek();
                oldFloor.transform.position = new Vector3(-1, 1, (float)(distance * 1.5));
                current.Dequeue();
                current.Enqueue(oldFloor);
            }

            //change distance of platform based on player position - note that platform will never get smaller
            //if(transform.position.z > platDistance) floor.transform.localScale = new Vector3(2, 1, -(transform.position.z+1.5f));
            //floor.transform.position = new Vector3(-1, 1, 0);

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
