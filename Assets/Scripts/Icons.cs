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
    public GameObject thought;
    GameObject chatBubble = null;
    public GameObject eliChatImage;
    string newText = "";
    char[] currentText;

    //Tracking
    int frames = 0;
    int currentLetter = 0;
    bool first = true;
    bool entered = false;
    public int z = -4;

    //FMOD
    private EventInstance appear;
    private PLAYBACK_STATE currentState;

    // Start is called before the first frame update
    void Start()
    {
        //Start chat off
        chat.SetActive(false);

        //Initialize action
        appear = FMODUnity.RuntimeManager.CreateInstance("event:/UI/IconAppear");
    }

    // Update is called once per frame
    void Update()
    {
        //Check if effect is playing
        appear.getPlaybackState(out currentState);

        //If triggered
        if (chat.activeSelf)
        {

            if (newText == "")
            {
                //Takes string from text into char array
                currentText = chat.GetComponent<Text>().text.ToCharArray();
                chat.GetComponent<Text>().text = "";
            }

            //If the chat is a thought
            if (chat.CompareTag("Thought") && chatBubble == null)
            {
                //Format in corner of screen
                chat.transform.position = new Vector3(Camera.main.ScreenToWorldPoint(new Vector2(Screen.height, Screen.width)).x + 3, Camera.main.ScreenToWorldPoint(new Vector2(Screen.height, Screen.width)).y + 3, z);
                chatBubble = Instantiate(thought);
                chatBubble.transform.parent = chat.transform;
                chatBubble.transform.position = chat.transform.position + new Vector3(0, -0.8f); //Chat bubble format
            }

            else if (chat.CompareTag("EliChat") && chatBubble == null)
            {
                //Format in corner of screen
                chat.transform.position = eli.transform.position + new Vector3(5, 4.5f);
                chatBubble = Instantiate(eliChatImage); //CHANGE TO THOUGHT BUBBLE
                chatBubble.transform.parent = chat.transform;
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

            //If user clicks
            if(Input.GetMouseButtonDown(0) && newText.ToCharArray().Length > 2)
            {
                //Chat disappears and icon becomes unavailable
                chat.SetActive(false);
                chatBubble.SetActive(false);
                gameObject.SetActive(false);
            }

            frames++;
        }
    }

    //When entering icon range
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
            //Chat about it
            chat.SetActive(true);

            //Remove icon
            gameObject.GetComponent<Animator>().SetBool("leaving", true);
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
