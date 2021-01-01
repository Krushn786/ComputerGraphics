using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    private Rigidbody rb;

    public Rotator[] pickUpObjects;

    public int totalObjects;

    //ADDING A DEFAULT SPEED.
    public float speed = 6.0f;

    //Timer Variable to display timer
    public TextMeshProUGUI timer;
    private float currentTime = 2f;

    //Count Variable and countText to count and display how many collectable items are caught
    private int count;
    public TextMeshProUGUI countText;

    //How many times the ball collided with the wall and other player
    private int collisionCount;

    //Boolean Variable for if the ball moved YET?
    private bool ballMoved = false;

    //All the keys that are used to play
    private string[] keysDown = { "up", "down", "left", "right", "w", "a", "s", "d", "space", "j" };

    //Collison Variable
    private bool collBallToGround;

    private bool timerReached = false;
    //To print message out when needed!
    public TextMeshProUGUI bannerText;

    //This Button Will reset the game
    public Button replayButton;




    // Start is called before the first frame update
    void Start()
    {
        pickUpObjects = FindObjectsOfType<Rotator>();
        rb = GetComponent<Rigidbody>();
        collBallToGround = true;
        bannerText.gameObject.SetActive(false);
        countText.text = "Score : 0";
        timer.text = "2.00";

        replayButton.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        totalObjects = pickUpObjects.Length;

        //Will check every frame to trigger the ball move and will hold on the timer
        if (ballMoved == false)
            triggerBallMove();

        if (Input.GetKeyDown("space") || Input.GetKeyDown("j"))
            jumpBall();


    }

    [System.Obsolete]
    private void FixedUpdate()
    {
        //This variables will update the variables that will help move the ball
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);


        if (!timerReached)
        {
            rb.AddForce(movement * speed);
        }

        else
        {
            rb.transform.position = new Vector3(0, 0.27f, 0);
            bannerText.gameObject.SetActive(true);
            bannerText.text = "Times Up!!!";
            speed = 0;
            replayButton.gameObject.SetActive(true);
        }


        if (ballMoved)
        {
            startTimer();
        }



    }

    //Checks if ball moved yet!
    void triggerBallMove()
    {
        for (int i = 0; i < keysDown.Length; i++)
        {
            if (Input.GetKeyDown(keysDown[i]))
            {
                ballMoved = true;

                //Will take the message of
                bannerText.gameObject.SetActive(false);
                bannerText.text = null;
                startTimer();
                i = keysDown.Length + 1;
            }
        }
        if (!ballMoved)
        {
            bannerText.gameObject.SetActive(true);
            bannerText.text = "Please use commands to move the ball!";
        }
    }

    //Will Start a timer when the ball moves!
    void startTimer()
    {
        timer.text = currentTime.ToString("f2");

        if (timer.text != "0.00")
            currentTime -= 0.01f * Time.deltaTime;
        if (timer.text == "1.99")
        {
            currentTime = 1.59f;
        }

        if (timer.text == "0.99")
        {
            currentTime = 0.59f;
        }

        if (timer.text == "0.00")
            timerReached = true;
        oneMinWarn();
    }
    //This will warn the player only 1 minute is left
    void oneMinWarn()
    {
        if (timer.text == ("1.00"))
        {
            bannerText.gameObject.SetActive(true);
            bannerText.text = "1 Minute Left";
        }

        if (timer.text.Equals("0.57"))
        {
            bannerText.gameObject.SetActive(false);
        }
    }

    void stopTimer()
    {
        if (timer.text.Equals("0.00"))
        {

        }
    }
    //Will make the ball jump when pressed "space" or "j"
    private void jumpBall()
    {

        //Make Ball Jump
        if (collBallToGround)
        {
            Vector3 jump = new Vector3(0.0f, 300.0f, 0.0f);
            rb.AddForce(jump);
            collBallToGround = false;
        }
    }

    // Will Check well the ball collides and takes action accordingly
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "Field")
        {
            collBallToGround = true;
        }

        if (collision.gameObject.tag == "Wall")
        {
            collisionCount += 1;

            if (count > 0)
            {
                count -= 1;
                countText.text = "Score : " + count.ToString();
            }
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("pickUp"))
        {

            other.gameObject.SetActive(false);

            count += 2;
            countText.text = "Score : " + count.ToString();
        }
        if (other.gameObject.CompareTag("redPickUp"))
        {

            other.gameObject.SetActive(false);

            count += 3;
            countText.text = "Score : " + count.ToString();
        }
        if (other.gameObject.CompareTag("bluePickUp"))
        {

            other.gameObject.SetActive(false);

            count += 4;
            countText.text = "Score : " + count.ToString();
        }

        if (other.gameObject.CompareTag("triggerPickUp"))
        {

            other.gameObject.SetActive(false);

            if (count > 0)
            {
                count -= 1;
            }

            countText.text = "Score : " + count.ToString();
        }
    }

    /**
     * This method was used for winScreen
     * 
    private void gameWinner()
    {
        bannerText.gameObject.SetActive(true);
        replayButton.gameObject.SetActive(true);
        bannerText.text = "You Win!!!";
    }
    */


}
