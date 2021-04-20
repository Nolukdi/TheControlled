using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Bedroom : MonoBehaviour
{
    //GameObjects
    public GameObject blackBG; //Pitch black
    public GameObject darkBG; //Darker with watercolor
    public GameObject waterDrip; //Water dripping anim
    public GameObject eliCry; //Eli crying anim
    public GameObject eli;  //Main character
    public GameObject chat; //Chat bubble prefab
    public GameObject thought; //Thought bubble prefab
    GameObject chatBubble; //Temporary variable

    //Booleans
    static bool isOpening = true;
    bool first = false;
    bool once = false;
    bool exiting = false;
    bool initial = false;
    bool leaving = false;
    bool left = false;

    //Frame counters
    float openingFrames = 0;

    //Chats
    public GameObject[] openingChat = new GameObject[9]; //Script array
    public int currentOpenChat = 0;
    char[] currentText; //Holds text taken from script array
    string newText; //Re-writes text here one letter at a time
    int currentLetter = 0;

    //FMOD
    private EventInstance drips;
    private EventInstance helpHead;
    private EventInstance doorOpen;
    public float helpParam;
    private PLAYBACK_STATE currentState;
    private PLAYBACK_STATE currentHeadState;
    private PLAYBACK_STATE doorState;

    // Start is called before the first frame update
    void Start()
    {
        //Make dark invisible
        darkBG.GetComponent<SpriteRenderer>().color = new Color(darkBG.GetComponent<SpriteRenderer>().color.r, darkBG.GetComponent<SpriteRenderer>().color.g, darkBG.GetComponent<SpriteRenderer>().color.b, 0);
        drips = FMODUnity.RuntimeManager.CreateInstance("event:/SFX/TearsFalling"); //Grab sound
        helpHead = FMODUnity.RuntimeManager.CreateInstance("event:/Music/Helping Head");
        doorOpen = FMODUnity.RuntimeManager.CreateInstance("event:/SFX/DoorOpen");
        helpHead.setParameterByName("CloseToDoor", 0);
    }

    // Update is called once per frame
    void Update()
    {
        helpHead.getParameterByName("CloseToDoor", out helpParam);
        helpHead.getPlaybackState(out currentHeadState);
        doorOpen.getPlaybackState(out doorState);

        if(currentHeadState != PLAYBACK_STATE.PLAYING && !exiting)
        {
            //Play song on loop
            helpHead.start();
        }

        //If the player away from toward the door (and the parameter is still loud)
        if(Input.GetKey(KeyCode.D) && helpParam > 0)
        {
            //Decrease
            helpHead.setParameterByName("CloseToDoor", helpParam - 0.0005f);
        }

        //If the player moves toward the door (and the parameter isn't at max)
        if(Input.GetKey(KeyCode.A) && helpParam < 1)
        {
            //Increase parameter value
            helpHead.setParameterByName("CloseToDoor", helpParam + 0.0005f);
        }

        //If this is the opening scene
        if(isOpening)
        {
            //Slowly bring the background in
            darkBG.GetComponent<SpriteRenderer>().color += new Color(0, 0, 0, 0.1f * Time.deltaTime);

            //Get if sound is playing
            drips.getPlaybackState(out currentState);

            //If background not loaded in yet
            if (darkBG.GetComponent<SpriteRenderer>().color.a < 1)
            {
                //Display opening animations
                blackBG.SetActive(true);
                darkBG.SetActive(true);
                waterDrip.SetActive(true);
                eliCry.SetActive(false);
            }

            //If the background has faded in
            if(darkBG.GetComponent<SpriteRenderer>().color.a >= 0.9 && !once)
            {
                //Start chat
                currentText = DisplayChat(currentOpenChat, openingChat);
                once = true; //Ensures this is not called multiple times
            }
            
            //If this is the first chat
            if(!first)
            {
                //Run once
                first = true;
                currentText = "".ToCharArray(); //Empty chat
            }

            //If first drip and enough time has passed
            if (!initial && openingFrames > 0.5f)
            {
                //Trigger sound
                drips.start();
                openingFrames = 0;
                initial = true;
            }

            //If not first drip
            else if(initial && openingFrames > 2.8f)
            {
                //Trigger sound
                drips.start();
                openingFrames = 0;
                initial = true;
            }


            //If player advances chat past the specified point
            if (Input.GetMouseButtonDown(0) && once)
            {
                //If the current chat is not the last one
                if (currentOpenChat < openingChat.Length - 1)
                {
                    //Destroy the last bubble and reset
                    Destroy(chatBubble);
                    currentLetter = 0;
                    newText = "";
                    openingChat[currentOpenChat].SetActive(false);
                    
                    //Advance chat
                    currentOpenChat++;

                    //Display next chat
                    currentText = DisplayChat(currentOpenChat, openingChat);
                }

                //If the current chat is the last
                else if (currentOpenChat == openingChat.Length - 1)
                {
                    //End opening
                    blackBG.SetActive(false);
                    darkBG.SetActive(false);
                    waterDrip.SetActive(false);
                    eliCry.SetActive(false);

                    //Reset
                    currentLetter = 0;
                    currentOpenChat = 0;
                    Destroy(chatBubble);
                    newText = "";

                    //Set last chat
                    openingChat[openingChat.Length - 1].GetComponent<Text>().text = newText;
                    openingChat[currentOpenChat].SetActive(false);

                    //No longer opening
                    isOpening = false;

                    //If sound is playing
                    if (currentState == PLAYBACK_STATE.PLAYING)
                    {
                        //Stop
                        drips.stop(STOP_MODE.IMMEDIATE);
                    }
                }

                //If Eli has described his hurt
                if(currentOpenChat == 7)
                {
                    //Stop/start anims
                    waterDrip.SetActive(false);
                    eliCry.SetActive(true);
                }
            }

            //Every 5 frames (if the current letter is still in the array)
            if ((openingFrames % 0.025f >= 0 && openingFrames % 0.025f <= 0.01) && currentLetter < currentText.Length)
            {
                //Add the next letter to the string and send it to the text
                newText += currentText[currentLetter].ToString();
                openingChat[currentOpenChat].GetComponent<Text>().text = newText;
                currentLetter++;
            }

            //Increment frames
            openingFrames += Time.deltaTime;
        }
    }

    //Method to display chat bubbles and text
    char[] DisplayChat(int currentOpenChat, GameObject[] chats)
    {
        //Sets the current text up
        chats[currentOpenChat].SetActive(true);

        //Takes string from text into char array
        char[] text = chats[currentOpenChat].GetComponent<Text>().text.ToCharArray();
        chats[currentOpenChat].GetComponent<Text>().text = "";

        //If the chat is a thought
        if (chats[currentOpenChat].CompareTag("Thought"))
        {
            //Format in corner of screen
            chats[currentOpenChat].transform.position = eli.transform.position + new Vector3(3, 4);
            chatBubble = Instantiate(thought);
            chatBubble.transform.position = chats[currentOpenChat].transform.position + new Vector3(0, -0.585f); //Thought bubble format
        }

        //If the chat is eli speaking
        else if (chats[currentOpenChat].CompareTag("EliChat"))
        {
            //Formats near eli
            chats[currentOpenChat].transform.position = eli.transform.position + new Vector3(1, 2);
            chatBubble = Instantiate(chat);
            chatBubble.transform.position = chats[currentOpenChat].transform.position + new Vector3(0, -0.8f); //Chat bubble format
        }

        //Return the array of individual letters to write out
        return text;
    }

    //Method to leave room
    public void LeaveRoom()
    {
        //Play sound
        exiting = true;
        doorOpen.start();
        helpHead.stop(STOP_MODE.ALLOWFADEOUT);

        //If sound is over
        if(doorState != PLAYBACK_STATE.PLAYING)
        {
            //Load next scene
            SceneManager.LoadScene("CityStreet1");
        }
    }
}
