using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD.Studio;

public class Opponent : MonoBehaviour
{
    //GameObjects
    public GameObject eli;

    //Booleans
    public bool fighting = false;
    public bool punch = true;
    public bool stunned = false;

    //Ints
    public int health = 20;

    //Physics
    Vector2 desiredVelocity;
    Vector2 currentVelocity;
    Vector2 steeringForce;
    public Collider2D outer;
    public Collider2D inner;

    //Time
    float punchTime = 0.0f;
    float stunTime = 0.0f;
    float waitTime = 0.0f;


    private EventInstance punch1Miss;
    private EventInstance punch1Hit;
    private EventInstance punch3Hit;
    private EventInstance punch3Miss;

    // Start is called before the first frame update
    void Start()
    {
        punch1Hit = FMODUnity.RuntimeManager.CreateInstance("event:/PlayerSounds/Punch1Hit");
        punch3Hit = FMODUnity.RuntimeManager.CreateInstance("event:/PlayerSounds/Punch3Hit");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (fighting)
        {
            //Always face eli
            if (eli.transform.position.x - gameObject.transform.position.x > 0 && health > 0)
            {
                gameObject.transform.rotation = new Quaternion(0, 0, 0, gameObject.transform.rotation.w);
            }

            else if (eli.transform.position.x - gameObject.transform.position.x <= 0 && health > 0)
            {
                gameObject.transform.rotation = new Quaternion(0, 180, 0, gameObject.transform.rotation.w);
            }


            //Seek eli unless too close
            if (Mathf.Abs(eli.transform.position.x - gameObject.transform.position.x) > 3.5f && !stunned && health > 0)
            {
                desiredVelocity = (eli.transform.position - gameObject.transform.position).normalized;
                steeringForce = desiredVelocity - currentVelocity;

                gameObject.transform.position += new Vector3(steeringForce.x, steeringForce.y) * Time.deltaTime * 2;

                gameObject.GetComponent<Animator>().SetFloat("speed", 1);

                //Reset wait time
                waitTime = 0;
                punch = false;

                //Reset all fighting variables when Eli exits range

            }

            //When eli is too close
            else if (Mathf.Abs(eli.transform.position.x - gameObject.transform.position.x) <= 3.5f && !stunned && health > 0)
            {
                gameObject.GetComponent<Animator>().SetFloat("speed", 0);

                //If enough time has passed
                if (waitTime > 0.5f)
                {
                    //If Eli isn't punching or dashing
                    if (!eli.GetComponent<PlayerController>().fighting && !eli.GetComponent<PlayerController>().dashing)
                    {
                        //If we aren't punching already
                        if (!punch)
                        {
                            //Throw punch
                            punch = true;
                            gameObject.GetComponent<Animator>().SetBool("firstPunch", punch);
                            eli.GetComponent<PlayerController>().punched = true;
                        }

                        punchTime += Time.deltaTime;
                    }
                }

                //If hit by Eli's initial punch
                if (eli.GetComponent<Animator>().GetBool("firstPunch"))
                {
                    //Hit once
                    stunned = true;
                    health = 15;
                    punch1Hit.start();
                }

                //If hit by Eli's combo
                else if (eli.GetComponent<Animator>().GetBool("secondPunch"))
                {
                    //Hit again
                    stunned = true;
                    health = 10;
                    punch1Hit.start();
                }

                //If hit by Eli's final
                else if (eli.GetComponent<Animator>().GetBool("thirdPunch"))
                {
                    //Hit again
                    stunned = true;
                    health = 0;
                    punch3Hit.start();
                }

                //Continue waiting
                waitTime += Time.deltaTime;
            }

            //If stunned
            if (stunned)
            {
                //Count
                stunTime += Time.deltaTime;

                //If stunned for enough
                if (stunTime > 2)
                {
                    //Reset
                    stunTime = 0;
                    stunned = false;
                }
            }

            //if the punch has finished
            if (punchTime > 1)
            {
                punchTime = 0;
                punch = false;
                gameObject.GetComponent<Animator>().SetBool("firstPunch", punch);
            }
        }
    }
}
