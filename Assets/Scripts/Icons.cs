using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FMOD.Studio;

public class Icons : MonoBehaviour
{
    //GameObject
    public GameObject eli;

    //Chat bubble
    public GameObject chat;
    public GameObject chatBubble;
    bool first = true;
    string newText = "";

    //Tracking
    int frames = 0;
    int currentLetter = 0;

    //FMOD
    private EventInstance appear;
    private PLAYBACK_STATE currentState;

    // Start is called before the first frame update
    void Start()
    {
        chat.SetActive(false);

        //Initialize action
        appear = FMODUnity.RuntimeManager.CreateInstance("event:/UI/IconAppear");
    }

    // Update is called once per frame
    void Update()
    {
        //Check if effect is playing
        appear.getPlaybackState(out currentState);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //If the player enters
        if (other.CompareTag("Player"))
        {
            //If first time
            if(first)
            {
                //Play initial animation
                gameObject.GetComponent<Animator>().Play(0);
            }

            //View icon
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

    private void OnTriggerStay2D(Collider2D collision)
    {
        Debug.Log("mouse: " + Camera.main.ScreenToWorldPoint(Input.mousePosition));
        Debug.Log("icon: " + gameObject.transform.position);

        if (Input.GetMouseButtonDown(0) &&
            (Camera.main.ScreenToWorldPoint(Input.mousePosition).x > gameObject.transform.position.x - 5
            && Camera.main.ScreenToWorldPoint(Input.mousePosition).x < gameObject.transform.position.x + 5
            && Camera.main.ScreenToWorldPoint(Input.mousePosition).y > gameObject.transform.position.y - 5
            && Camera.main.ScreenToWorldPoint(Input.mousePosition).y < gameObject.transform.position.y + 5))
        {
            Debug.Log("triggered");

            //Chat about it
            chat.SetActive(true);

        }

        if (chat.activeSelf)
        {

            //Takes string from text into char array
            char[] currentText = chat.GetComponent<Text>().text.ToCharArray();
            chat.GetComponent<Text>().text = "";

            //If the chat is a thought
            if (chat.CompareTag("Thought"))
            {
                //Format in corner of screen
                chat.transform.position = eli.transform.position + new Vector3(3, 4);
                chatBubble = Instantiate(chat); //CHANGE TO THOUGHT BUBBLE
                chatBubble.transform.position = chat.transform.position + new Vector3(0, -0.8f); //Chat bubble format
            }

            //Every 5 frames (if the current letter is still in the array)
            if (frames % 5 == 0 && currentLetter < currentText.Length)
            {
                //Add the next letter to the string and send it to the text
                newText += currentText[currentLetter].ToString();
                chat.GetComponent<Text>().text = newText;
                currentLetter++;
            }

            frames++;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        //Hide icon
        gameObject.GetComponent<Animator>().SetBool("leaving", true);
    }
}
