using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //Related choices
    public static int medication = 0;

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
    public bool fighting = false;
    public bool combo = false;
    public bool dashing = false;
    public bool canFight = false;
    public bool punched = false;
    float stunTimer = 0;

    //FMOD
    private EventInstance footsteps;
    private EventInstance punch1Miss;
    private EventInstance punch3Miss;
    private EventInstance hit1Success;
    private EventInstance hit2Success;
    private EventInstance hit3Success;
    private PLAYBACK_STATE currentFootState;


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

        footsteps = FMODUnity.RuntimeManager.CreateInstance("event:/PlayerSounds/Footsteps");
        punch1Miss = FMODUnity.RuntimeManager.CreateInstance("event:/PlayerSounds/Punch1Miss");
        punch3Miss = FMODUnity.RuntimeManager.CreateInstance("event:/PlayerSounds/Punch3Miss");
        hit1Success = FMODUnity.RuntimeManager.CreateInstance("event:/UI/HitSuccess1");
        hit2Success = FMODUnity.RuntimeManager.CreateInstance("event:/UI/HitSuccess2");
        hit3Success = FMODUnity.RuntimeManager.CreateInstance("event:/UI/HitSuccess3");
    }

    // Update is called once per frame
    void Update()
    {
        footsteps.getPlaybackState(out currentFootState);

        //If not stunned
        if (!punched)
        {
            //If player presses D
            if (Input.GetKey(KeyCode.D))
            {
                //Move right
                gameObject.transform.position += (new Vector3(speed, 0) * Time.deltaTime);
                anim.SetFloat("speed", 1);
                gameObject.transform.rotation = new Quaternion(0, 0, 0, 1);
                facingRight = true;

                //If not playing already
                if (currentFootState != PLAYBACK_STATE.PLAYING)
                {
                    //Trigger sound
                    footsteps.start();
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
                if (currentFootState != PLAYBACK_STATE.PLAYING)
                {
                    //Trigger sound
                    footsteps.start();
                }
            }

            //If the player is not pressing either
            else
            {
                //Stop moving
                anim.SetFloat("speed", 0);

                //Stop sound
                footsteps.stop(STOP_MODE.ALLOWFADEOUT);
            }

            if (canFight)
            {
                //If the player presses F
                if (Input.GetKeyDown(KeyCode.F))
                {
                    //Starting fighting
                    anim.SetBool("firstPunch", true);
                    fighting = true;
                    punch1Miss.start();

                    hit1Success.start();
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
                if (dashing)
                {
                    //Remove hitbox
                    gameObject.GetComponent<BoxCollider2D>().enabled = false;

                    //If facing right
                    if (facingRight)
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
                    if (timer > 0.25f)
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

                            //Play sounds (miss and success for quicktime)
                            punch1Miss.start();
                            hit2Success.start();
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

                            //Play sounds (miss and success for quicktime)
                            hit3Success.start();
                            punch3Miss.start();
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

        //If stunned
        else
        {
            //Count
            stunTimer += Time.deltaTime;
            anim.SetFloat("speed", 0);

            //If stunned for enough time
            if(stunTimer > 2f)
            {
                //Reset
                stunTimer = 0;
                punched = false;
            }
        }
    }
}
