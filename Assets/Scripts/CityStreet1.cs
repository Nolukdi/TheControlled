using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using UnityEngine.UI;
using UnityEngine;

public class CityStreet1 : MonoBehaviour
{
    //FMOD
    private EventInstance helpHead;
    private PLAYBACK_STATE currentHeadState;
    private EventInstance cityAmb;
    private PLAYBACK_STATE cityAmbState;
    private EventInstance choice;

    //Chats
    public GameObject chat; //Chat bubble prefab
    public GameObject[] chat1 = new GameObject[26]; //Script array
    public GameObject[] chat2 = new GameObject[36];
    public int currentOpenChat = 0;
    char[] currentText; //Holds text taken from script array
    string newText; //Re-writes text here one letter at a time
    int currentLetter = 0;
    public GameObject thought; //Thought bubble prefab
    GameObject chatBubble;
    GameObject chatBubble2;
    //Seconds
    public float seconds = 0.0f;

    //Gameobjects
    public GameObject eli;
    public GameObject train;
    public GameObject calla;
    public GameObject jay;
    public GameObject pauseMenu;

    //Tracking
    public float exChatter = 0;
    public bool chatter = false;
    float chatFrames = 0;
    float endTimer = 0;
    bool endTime = false;
    bool first = false;
    public int choiceNum = 0;
    bool onPath1 = false;
    bool onPath2 = false;
    bool onPath1_2 = false;
    bool onPath2_2 = false;


    // Start is called before the first frame update
    void Start()
    {
        //Get song and set parameter
        helpHead = FMODUnity.RuntimeManager.CreateInstance("event:/Music/Helping Head");
        helpHead.setParameterByName("CloseToDoor", 0.8f);
        cityAmb = FMODUnity.RuntimeManager.CreateInstance("event:/Ambience/City");
        cityAmb.setParameterByName("TrainComing", 0.0f);
        cityAmb.setParameterByName("TrainDistance", 0.0f);
        cityAmb.setParameterByName("ExtraChatter", 0.0f);
        cityAmb.start();
        choice = FMODUnity.RuntimeManager.CreateInstance("event:/UI/Choice");
    }

    // Update is called once per frame
    void Update()
    {
        if (pauseMenu.GetComponent<PauseMenu>().paused)
        {
            cityAmb.setPaused(true);
            helpHead.setPaused(true);
        }

        else
        {
            helpHead.setPaused(false);
            cityAmb.setPaused(false);
        }

        //Get playing state
        helpHead.getPlaybackState(out currentHeadState);
        cityAmb.getPlaybackState(out cityAmbState);
        cityAmb.getParameterByName("ExtraChatter", out exChatter);

        //Set sound volumes
        cityAmb.setVolume(MainMenu.sfxVol);
        helpHead.setVolume(MainMenu.musicVol);

        #region

        //If not already playing
        if (currentHeadState != PLAYBACK_STATE.PLAYING)
        {
            //Play
            helpHead.start();
        }

        //If the train arrives
        if (seconds % 55 >= 0 && seconds % 55 <= 0.01)
        {
            //Sound the horn
            cityAmb.setParameterByName("TrainComing", 1f);
        }

        //If the train is arriving
        if (seconds % 58 >= 0 && seconds % 58 <= 0.01)
        {
            //Sound the train rattles
            cityAmb.setParameterByName("TrainDistance", 1f);
        }

        //If more time passes
        if (seconds % 60 >= 0 && seconds % 60 <= 0.01)
        {
            //Stop train horn
            cityAmb.setParameterByName("TrainComing", 0f);
        }

        //If the train has left
        if (seconds % 70 >= 0 && seconds % 70 <= 0.01)
        {
            //Stop train noises
            cityAmb.setParameterByName("TrainDistance", 0f);
        }

        //If enough time has passed for more chatter
        if (seconds % 30 >= 0 && seconds % 30 <= 0.01)
        {
            chatter = true;
        }

        //If too much time has passed
        if (seconds % 45 >= 0 && seconds % 45 <= 0.01)
        {
            chatter = false;
        }

        //If chattering
        if(chatter && exChatter < 1)
        {
            //Increase
            cityAmb.setParameterByName("ExtraChatter", exChatter + 0.01f);
        }

        //If not
        else if(!chatter && exChatter > 0)
        {
            //Decrease
            cityAmb.setParameterByName("ExtraChatter", exChatter - 0.01f);
        }

        #endregion

        if (calla.GetComponent<Calla>().talking)
        {
            //If this is the first chat
            if (!first)
            {
                //Run once
                first = true;
                currentText = "".ToCharArray(); //Empty chat
                currentText = DisplayChat(currentOpenChat, chat1);
            }

            //If player advances chat past the specified point
            if (Input.GetMouseButtonDown(0) && !chat1[currentOpenChat].CompareTag("Choice"))
            {
                //Destroy all bubbles
                foreach(GameObject n in GameObject.FindGameObjectsWithTag("ChatBubble"))
                {
                    Destroy(n);
                }

                //If the current chat is not the last one
                if (currentOpenChat < chat1.Length - 1)
                {
                    //Destroy the last bubble and reset
                    Destroy(chatBubble);
                    Destroy(chatBubble2);
                    currentLetter = 0;
                    newText = "";
                    chat1[currentOpenChat].SetActive(false);

                    //Advance chat
                    currentOpenChat++;

                    //Display next chat
                    currentText = DisplayChat(currentOpenChat, chat1);
                }

                //If the current chat is the last
                else if (currentOpenChat == chat1.Length - 1)
                {
                    //Reset
                    currentLetter = 0;
                    currentOpenChat = 0;
                    Destroy(chatBubble);
                    newText = "";

                    //Set last chat
                    chat1[chat1.Length - 1].GetComponent<Text>().text = newText;
                    chat1[currentOpenChat].SetActive(false);

                    //No longer talking
                    calla.GetComponent<Calla>().talking = false;
                }
            }

            //If the current chat is a choice and the player picks choice 1
            if (Input.GetMouseButtonDown(0) && chat1[currentOpenChat].CompareTag("Choice") && chatBubble.GetComponent<ChatBubble>().chosen)
            {
                Destroy(chatBubble);
                Destroy(chatBubble2);
                currentLetter = 0;
                newText = "";
                chat1[currentOpenChat].SetActive(false);
                currentOpenChat++;

                //Display next chat
                currentText = DisplayChat(currentOpenChat, chat1);

                choiceNum++;

                //If first choice of the conversation
                if(choiceNum == 1)
                {
                    //Dismisses Calla
                    Calla.relationship--;
                }

                if (choiceNum == 2)
                {
                    currentOpenChat = 19;
                    PlayerController.medication++;
                    currentText = DisplayChat(currentOpenChat, chat1);
                }
            }

            //If the current chat is a choice and the player picks choice 2
            else if (chat1[currentOpenChat].CompareTag("Choice") && chatBubble2 != null)
            {
                if (chatBubble2.GetComponent<ChatBubble>().chosen)
                {
                    Destroy(chatBubble);
                    Destroy(chatBubble2);
                    currentLetter = 0;
                    newText = "";
                    chat1[currentOpenChat].SetActive(false);
                    currentOpenChat++;

                    //Display next chat
                    currentText = DisplayChat(currentOpenChat, chat1);

                    choiceNum++;

                    //If it is the first choice of the conversation
                    if (choiceNum == 1)
                    {
                        //Advance to next part of convo
                        currentOpenChat = 10;
                        Calla.relationship++; //Accepts Calla
                        currentText = DisplayChat(currentOpenChat, chat1);
                    }

                    else if (choiceNum == 3)
                    {
                        currentOpenChat = 21;
                        Calla.relationship++; //Works with Calla
                        currentText = DisplayChat(currentOpenChat, chat1);
                    }
                }
            }

            //Every 5 frames (if the current letter is still in the array)
            if ((chatFrames % 0.025f >= 0 && chatFrames % 0.025f <= 0.01) && currentLetter < currentText.Length)
            {
                //Add the next letter to the string and send it to the text
                newText += currentText[currentLetter].ToString();
                chat1[currentOpenChat].GetComponent<Text>().text = newText;
                currentLetter++;
            }

            //If the convo ends early
            if (currentOpenChat == 9 && newText.Length > 3)
            {
                //Reset
                Destroy(chatBubble);
                Destroy(chatBubble2);
                currentLetter = 0;
                newText = "";
                chat1[currentOpenChat].SetActive(false);
                calla.GetComponent<Calla>().talking = false;
                currentOpenChat = 0;
            }

            //If skipping part of convo
            if (currentOpenChat == 18 && Input.GetMouseButtonDown(0))
            {
                //Reset
                currentLetter = 0;
                newText = "";
                chat1[currentOpenChat].SetActive(false);
                currentOpenChat = 21;
                currentText = DisplayChat(currentOpenChat, chat1);
            }

            //Increment frames
            chatFrames += Time.deltaTime;
        }

        if (jay.GetComponent<Jay>().talking)
        {
            //If this is the first chat
            if (!first)
            {
                //Run once
                first = true;
                currentText = "".ToCharArray(); //Empty chat
                currentText = DisplayChat(currentOpenChat, chat2);
            }

            //If player advances chat past the specified point
            if (Input.GetMouseButtonDown(0) && !chat2[currentOpenChat].CompareTag("Choice") && !endTime)
            {
                //Destroy all bubbles
                foreach (GameObject n in GameObject.FindGameObjectsWithTag("ChatBubble"))
                {
                    Destroy(n);
                }

                //If the current chat is not the last one
                if (currentOpenChat < chat2.Length - 1)
                {
                    //Destroy the last bubble and reset
                    Destroy(chatBubble);
                    Destroy(chatBubble2);
                    currentLetter = 0;
                    newText = "";
                    chat2[currentOpenChat].SetActive(false);

                    //Advance chat
                    currentOpenChat++;

                    //Display next chat
                    currentText = DisplayChat(currentOpenChat, chat2);
                }

                //If the current chat is the last
                else if (currentOpenChat == chat2.Length - 1)
                {
                    //Reset
                    currentLetter = 0;
                    currentOpenChat = 0;
                    Destroy(chatBubble);
                    newText = "";

                    //Set last chat
                    chat2[chat2.Length - 1].GetComponent<Text>().text = newText;
                    chat2[currentOpenChat].SetActive(false);

                    //No longer talking
                    jay.GetComponent<Jay>().talking = false;
                }
            }

            //If the current chat is a choice
            if (chatBubble != null && chat2[currentOpenChat].CompareTag("Choice") && chatBubble2 != null)
            {
                //If the player picks the first chat bubble
                if (Input.GetMouseButtonDown(0) && chatBubble.GetComponent<ChatBubble>().chosen)
                {
                    //Reset
                    Destroy(chatBubble);
                    Destroy(chatBubble2);
                    currentLetter = 0;
                    newText = "";
                    chat2[currentOpenChat].SetActive(false);
                    currentOpenChat++;

                    //Play sound
                    choice.start();

                    //Display next chat
                    currentText = DisplayChat(currentOpenChat, chat2);

                    choiceNum++;

                    //If first choice of the conversation
                    if (choiceNum == 1)
                    {
                        onPath1 = true;
                    }
                }

                //if the player picks the second chat bubble
                else if (chatBubble2.GetComponent<ChatBubble>().chosen)
                {
                    Destroy(chatBubble);
                    Destroy(chatBubble2);
                    currentLetter = 0;
                    newText = "";
                    chat2[currentOpenChat].SetActive(false);
                    currentOpenChat++;

                    //Display next chat
                    currentText = DisplayChat(currentOpenChat, chat2);

                    //Play sound
                    choice.start();

                    choiceNum++;

                    //If it is the first choice of the conversation
                    if (choiceNum == 1)
                    {
                        currentOpenChat = 23;
                        currentText = DisplayChat(currentOpenChat, chat2);
                        onPath2 = true;
                    }

                    //If we started the first path
                    else if (choiceNum == 2 && onPath1)
                    {
                        currentOpenChat = 22;
                        currentText = DisplayChat(currentOpenChat, chat2);
                        onPath1_2 = true;
                    }

                    else if (choiceNum == 3 && onPath1)
                    {
                        currentOpenChat = 19;
                        currentText = DisplayChat(currentOpenChat, chat2);
                    }

                    //If we started on the second path
                    else if (choiceNum == 2 && onPath2)
                    {
                        currentOpenChat = 30;
                        currentText = DisplayChat(currentOpenChat, chat2);
                        onPath2_2 = true;
                    }

                    else if (choiceNum == 3 && onPath2_2)
                    {
                        currentOpenChat = 34;
                        currentText = DisplayChat(currentOpenChat, chat2);
                    }
                }
            }

            //Every 5 frames (if the current letter is still in the array)
            if ((chatFrames % 0.025f >= 0 && chatFrames % 0.025f <= 0.01) && currentLetter < currentText.Length)
            {
                //Add the next letter to the string and send it to the text
                newText += currentText[currentLetter].ToString();
                chat2[currentOpenChat].GetComponent<Text>().text = newText;
                currentLetter++;
            }


            //If we hit any end of the conversation
            if ((currentOpenChat == 18 || currentOpenChat == 21 || currentOpenChat == 22 || currentOpenChat == 29 || currentOpenChat == 33) && Input.GetMouseButtonDown(0))
            {
                //Start timer
                endTime = true;
            }

            //If ending conversation
            if(endTime)
            {
                //Count up
                endTimer += Time.deltaTime;
            }

            //If time to end
            if(endTimer > 2)
            {
                endTimer = 0;
                endTime = false;

                chat2[currentOpenChat].SetActive(false);
                jay.GetComponent<Jay>().talking = false;

                //Reset
                currentLetter = 0;
                currentOpenChat = 0;
                Destroy(chatBubble);
                newText = "";

                //Destroy all bubbles
                foreach (GameObject n in GameObject.FindGameObjectsWithTag("ChatBubble"))
                {
                    Destroy(n);
                }
            }

            //Increment frames
            chatFrames += Time.deltaTime;
        }

        //Count seconds
        seconds += Time.deltaTime;
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
            chats[currentOpenChat].transform.position = eli.transform.position + new Vector3(1.35f, 2.15f);
            chatBubble = Instantiate(chat);
            chatBubble.transform.position = chats[currentOpenChat].transform.position + new Vector3(0, -0.7f); //Chat bubble format
        }

        //If the chat isn't eli speaking
        else if (chats[currentOpenChat].CompareTag("OtherChat"))
        {
            //Formats near Calla
            if (calla.GetComponent<Calla>().talking) { chats[currentOpenChat].transform.position = calla.transform.position - new Vector3(1.35f, -2.15f); }
            if (jay.GetComponent<Jay>().talking) { chats[currentOpenChat].transform.position = jay.transform.position - new Vector3(1.35f, -2.15f); }
            chatBubble = Instantiate(chat);
            chatBubble.transform.position = chats[currentOpenChat].transform.position + new Vector3(0, -0.7f); //Chat bubble format
            chatBubble.transform.rotation = new Quaternion(chatBubble.transform.rotation.x, chatBubble.transform.rotation.y + 180, chatBubble.transform.rotation.z, chatBubble.transform.rotation.w);
        }

        //If the chat is a choice
        else if (chats[currentOpenChat].CompareTag("Choice"))
        {
            //Gets other text
            chats[currentOpenChat].transform.GetChild(0).gameObject.SetActive(true);

            //Formats on either side of eli
            chats[currentOpenChat].transform.position = eli.transform.position + new Vector3(1.5f, 2.15f);
            chatBubble = Instantiate(chat);
            chatBubble.transform.position = chats[currentOpenChat].transform.position + new Vector3(0, -0.7f); //Chat bubble format
            chatBubble2 = Instantiate(chat);
            chatBubble2.transform.position = chats[currentOpenChat].transform.GetChild(0).position + new Vector3(0, -0.7f);
            chatBubble2.transform.rotation = new Quaternion(chatBubble2.transform.rotation.x, chatBubble2.transform.rotation.y + 180, chatBubble2.transform.rotation.z, chatBubble2.transform.rotation.w);
        }

        //Return the array of individual letters to write out
        return text;
    }

}