using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FMOD.Studio;

public class Chatcons : MonoBehaviour
{
    //GameObject
    public GameObject eli;
    public GameObject chatPerson;

    //Tracking
    bool first = true;
    bool entered = false;
    public int z = -4;

    //FMOD
    private EventInstance appear;
    private PLAYBACK_STATE currentState;


    // Start is called before the first frame update
    void Start()
    {
        //Initialize action
        appear = FMODUnity.RuntimeManager.CreateInstance("event:/UI/IconAppear");
    }

    // Update is called once per frame
    void Update()
    {
        //Check if effect is playing
        appear.getPlaybackState(out currentState);
    }

    //When entering icon range
    private void OnTriggerEnter2D(Collider2D other)
    {
        //If the player enters
        if (other.CompareTag("Player"))
        {
            //If first time
            if (first)
            {
                //Play initial animation
                gameObject.GetComponent<Animator>().Play(0);
            }

            //View icon
            entered = true;
            first = false;
            gameObject.GetComponent<Animator>().SetBool("leaving", false);
            gameObject.GetComponent<SpriteRenderer>().enabled = true;

            //If not playing already
            if (currentState != PLAYBACK_STATE.PLAYING)
            {
                //Trigger sound
                appear.start();
            }
        }
    }

    //When clicking icon
    public void OnMouseDown()
    {
        //If inside trigger
        if (entered)
        {
            //If Calla
            if (chatPerson.CompareTag("Calla")) { chatPerson.GetComponent<Calla>().talking = true; }
            if (chatPerson.CompareTag("Jay")) { chatPerson.GetComponent<Jay>().talking = true; }

            //Remove icon
            gameObject.GetComponent<Animator>().SetBool("leaving", true);
            gameObject.SetActive(false);
        }
    }


    //While exiting
    private void OnTriggerExit2D(Collider2D other)
    {
        //Hide icon
        gameObject.GetComponent<Animator>().SetBool("leaving", true);
        entered = false;
    }
}
