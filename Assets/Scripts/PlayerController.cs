using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //Animation
    Animator anim;

    //Rigidbody
    Rigidbody2D rb;

    //Visuals
    public GameObject ePrompt;
    public GameObject wPrompt;
    public Sprite sprint;

    //Tracking
    public bool facingRight = true;
    public float speed = 3f;
    float timer = 0;
    bool fighting = false;
    bool combo = false;
    bool dashing = false;

    //FMOD
    private EventInstance eventRef;
    private PLAYBACK_STATE currentState;


    // Start is called before the first frame update
    void Start()
    {
        //Get items
        anim = gameObject.GetComponent<Animator>();
        rb = gameObject.GetComponent<Rigidbody2D>();

        //Set initial values
        timer = 0;
        ePrompt.SetActive(false);
        wPrompt.SetActive(false);

        eventRef = FMODUnity.RuntimeManager.CreateInstance("event:/PlayerSounds/Footsteps");
        
    }

    // Update is called once per frame
    void Update()
    {
        eventRef.getPlaybackState(out currentState);

        //If player presses D
        if (Input.GetKey(KeyCode.D))
        {
            //Move right
            gameObject.transform.position += (new Vector3(speed, 0) * Time.deltaTime);
            anim.SetFloat("speed", 1);
            gameObject.transform.rotation = new Quaternion(0, 0, 0, 1);
            facingRight = true;

            //If not playing already
            if (currentState != PLAYBACK_STATE.PLAYING)
            {
                //Trigger sound
                eventRef.start();
            }
        }

        //If the player presses A
        else if (Input.GetKey(KeyCode.A))
        {
            //Move left
            gameObject.transform.position -= (new Vector3(speed, 0) * Time.deltaTime);
            anim.SetFloat("speed", 1);
            gameObject.transform.rotation = new Quaternion(0, 180, 0, 1);
            facingRight = false;


            //If not playing already
            if (currentState != PLAYBACK_STATE.PLAYING)
            {
                //Trigger sound
                eventRef.start();
            }
        }

        //If the player is not pressing either
        else
        {
            //Stop moving
            anim.SetFloat("speed", 0);

            //Stop sound
            eventRef.stop(STOP_MODE.ALLOWFADEOUT);
        }

        //If the player presses F
        if (Input.GetKeyDown(KeyCode.F))
        {
            //Starting fighting
            anim.SetBool("firstPunch", true);
            fighting = true;
        }

        //If the player presses shift
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            //Dash
            dashing = true;
            timer = 0.0f;
            gameObject.GetComponent<SpriteRenderer>().sprite = sprint;
            anim.enabled = false;
        }
        
        //If the player is dashing
        if(dashing)
        {
            //Remove hitbox
            gameObject.GetComponent<BoxCollider2D>().enabled = false;

            //If facing right
            if(facingRight)
            {
                //Dash right
                gameObject.transform.position += (new Vector3(speed * 15, 0) * Time.deltaTime);
            }

            //If facing left
            else
            {
                //Dash left
                gameObject.transform.position -= (new Vector3(speed * 15, 0) * Time.deltaTime);
            }

            //If dash is over
            if(timer > 0.25f)
            {
                //Stop dashing
                dashing = false;
                timer = 0.0f;
                anim.enabled = true;

                //Resume hitbox
                gameObject.GetComponent<BoxCollider2D>().enabled = true;
            }

            //Increment
            timer += Time.deltaTime;
        }

        //If the player is fighting
        if (fighting)
        {
            //Timer counts up
            timer += Time.deltaTime;

            //If the player is not on second punch
            if (!combo)
            {
                //E
                ePrompt.SetActive(true);
                wPrompt.SetActive(false);


                //If the player presses the assigned prompt (and timer is above minimum)
                if (Input.GetKeyDown(KeyCode.E) && timer >= 0.3f)
                {
                    //Move onto next and reset timer
                    anim.SetBool("secondPunch", true);
                    combo = true;
                    timer = 0;
                    ePrompt.SetActive(false);
                }
            }

            //If the player is on second punch
            else
            {
                //W
                wPrompt.SetActive(true);
                ePrompt.SetActive(false);

                //If the player presses the assigned prompt
                if (Input.GetKeyDown(KeyCode.W) && timer >= 0.3f)
                {
                    //Finish fight (timer will automatically reset)
                    anim.SetBool("thirdPunch", true);
                    combo = false;
                    fighting = false;
                    anim.SetBool("firstPunch", false);
                    anim.SetBool("secondPunch", false);
                    wPrompt.SetActive(false);
                    ePrompt.SetActive(false);
                }
            }

            //If the timer has reached limit
            if (timer >= 1.25)
            {
                //Reset player and timer
                fighting = false;
                anim.SetBool("firstPunch", false);
                anim.SetBool("secondPunch", false);
                anim.SetBool("thirdPunch", false);
                ePrompt.SetActive(false);
                timer = 0;
                wPrompt.SetActive(false);
                combo = false;
            }
        }
    }
}
