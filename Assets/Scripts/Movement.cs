using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

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
    private float obstacleAddLength = 0;
    private bool isCurrentObstacle = false;
    private float zSize;
    private readonly System.Random random = new System.Random();
    private GameObject[] beams;
    private GameObject[] floors;
    private GameObject[] obstacles;
    private MeshRenderer obstacleSize;

    //for temp floor
    private GameObject oldFloor;

    //holds all floors and keeps track of current
    private Queue<GameObject> current = new Queue<GameObject>();

    private TextMeshProUGUI textScore;
    private float currentScore = 0;

    //Persistent data
    private SaveData data;

    GameObject newFloor;
    //So no duplicate obstacle (as it makes player fall)
    GameObject oldObstacle;
    private bool movePlat = false;

    void Start()
    {
        beams = GameObject.FindGameObjectsWithTag("pretty");
        //setting up quque for floor
        floors = GameObject.FindGameObjectsWithTag("floor");
        obstacles = GameObject.FindGameObjectsWithTag("obstacle");

        data = GameObject.Find("Data").GetComponent<SaveData>();

        textScore = GameObject.Find("Score").GetComponent<TextMeshProUGUI>();

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
        current.Peek().transform.localScale = new Vector3(2, 1, -(transform.position.z + zSize));
        current.Peek().transform.position = new Vector3(-1, 1, 0);

        //So 1st platform doesn't move from under player.
        GameObject temp = current.Peek();
        current.Dequeue();
        current.Enqueue(temp);

        newFloor = current.Peek();

        //Set to difficult obstacle - initiates non-duplicate obstacles.
        oldObstacle = GameObject.Find("Obstacle 3");

    }

    void Update()
    {
        //Display current distance
        if(transform.position.z > currentScore)
        {
            currentScore = transform.position.z;
            textScore.text = "Distance: " + transform.position.z.ToString("F0");
        }
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


        //Moving platforms
        if (movePlat)
        {
            newFloor.transform.position = Vector3.MoveTowards(newFloor.transform.position, new Vector3(-1, 1, (float)(distance * zSize) + obstacleAddLength), 0.5f);
        }


        //Keep updating offset for platforms
        Vector3 tempV3 = new Vector3(-1, 1, (float)(distance * zSize) + obstacleAddLength);

        if (newFloor.transform.position == tempV3 && isCurrentObstacle)
        {
            movePlat = false;
            isCurrentObstacle = false;
            //increase offset by obstacle size
            obstacleSize = newFloor.GetComponent<MeshRenderer>();
            obstacleAddLength += obstacleSize.bounds.size.z - zSize;    //-zSize to stop gap forming after platform

            //so no duplicate floor
            oldObstacle = newFloor;
        }
        else if (newFloor.transform.position == tempV3 && !isCurrentObstacle)
        {
            movePlat = false;
        }
        

        // Move the controller
        if (!dead)
        {
            characterController.Move(moveDirection * Time.deltaTime);

            

            if ((transform.position.z > (distance * zSize) + obstacleAddLength))
            {
                int temp = random.Next(0, 100);
                // 1 in 3 chance for obstacle
                isCurrentObstacle =  temp >= 66  ?  true : false;
            }


            //go in steps of zSize on z axis
            if (transform.position.z > (distance * zSize) + obstacleAddLength && !isCurrentObstacle && !movePlat)
            {

                distance++;
                newFloor = current.Peek();

                //move platform above camera + setup to be moved down.
                newFloor.transform.position = new Vector3(-1, 10, (float)(distance * zSize) + obstacleAddLength);
                movePlat = true;

                current.Dequeue();
                current.Enqueue(newFloor);
                
                oldFloor = newFloor;

            }
            else if (transform.position.z > (distance * zSize) + obstacleAddLength && isCurrentObstacle && !movePlat)
            {
                //TODO: don't have the same obstacle 2 times in a row.
                distance++;

                newFloor = obstacles[random.Next(0, obstacles.Length)];  //get a random obstacle

                while(newFloor.name == oldObstacle.name)
                {
                    newFloor = obstacles[random.Next(0, obstacles.Length)];  //get a random obstacle
                }

                //move platform above camera + setup to be moved down.
                newFloor.transform.position = new Vector3(-1, 10, (float)(distance * zSize) + obstacleAddLength);

                movePlat = true;

                //Debug.Log(obstacleSize.bounds.size.x + " + " + obstacleSize.bounds.size.y + " + " + obstacleSize.bounds.size.z + ".");
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
        //Score is distance
        data.setHighScore(transform.position.z);
        SceneManager.LoadScene("Menu", LoadSceneMode.Single);
        //dead = true;
        //transform.position = startPosition;
    }
    private void OnTriggerExit(Collider other)
    {
        dead = false;
    }
}
