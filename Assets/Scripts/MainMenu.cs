using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;
using FMOD.Studio;

public class MainMenu : MonoBehaviour
{
    //Backgrounds
    public GameObject greenBG;
    public GameObject redBG;

    //GameObjects
    public GameObject cord;

    //Menus
    public GameObject mainMenu;
    public GameObject optionsMenu;

    //Tracking
    public bool changing = false;
    public bool change = false;
    public float seconds = 0;
    public bool options = false;

    //FMOD
    private EventInstance lightClick;
    private EventInstance theInvest;
    private PLAYBACK_STATE currentState;
    private PLAYBACK_STATE currentSong;

    // Start is called before the first frame update
    void Start()
    {
        //Set initial menus
        greenBG.SetActive(true);
        redBG.SetActive(false);
        optionsMenu.SetActive(false);
        mainMenu.SetActive(true);

        lightClick = FMODUnity.RuntimeManager.CreateInstance("event:/SFX/MenuChange"); //Grab sound
        theInvest = FMODUnity.RuntimeManager.CreateInstance("event:/Music/The Investigation");
        //Play it
        theInvest.start();
    }

    // Update is called once per frame
    void Update()
    {
        //Get sfx states
        lightClick.getPlaybackState(out currentState);
        theInvest.getPlaybackState(out currentSong);

        //If the menus are changing
        if (changing)
        {
            //And enough time as passed (for the animation)
            if(seconds > 1 && seconds < 1.05f)
            {
                //Reset timer
                seconds = 0;

                //Trigger change of menus
                change = true;
                changing = false;
                cord.GetComponent<Animator>().SetBool("changeMenu", false);
            }

            //Or enough time has passed (for the sound effect)
            else if(seconds > 0.7 && seconds < 0.75)
            {
                //Play sound
                if (currentState != PLAYBACK_STATE.PLAYING)
                {
                    lightClick.start();
                }
            }

            //Count seconds
            seconds += Time.deltaTime;
        }

        //If it is time to change and changing to options
        if (change && options)
        {
            //Change green to red
            greenBG.SetActive(false);
            redBG.SetActive(true);

            //Change menus
            optionsMenu.SetActive(true);
            mainMenu.SetActive(false);
            change = false;
        }

        //If it is time to change, and we are not changing to options
        else if (change && !options)
        {
            //Change menus
            optionsMenu.SetActive(false);
            mainMenu.SetActive(true);

            //Change green to red
            greenBG.SetActive(true);
            redBG.SetActive(false);
            change = false;
        }
    }

    //Start game
    public void StartGame()
    {
        //Stop song
        theInvest.stop(STOP_MODE.ALLOWFADEOUT);

        //Load bedroom scene
        SceneManager.LoadScene("EliBedroom");
    }

    //Options menu
    public void Options()
    {
        //Trigger anim
        cord.GetComponent<Animator>().SetBool("changeMenu", true);
        changing = true;
        options = true;
    }

    //Back
    public void Back()
    {
        //Start changing anim
        cord.GetComponent<Animator>().SetBool("changeMenu", true);
        changing = true;
        options = false;
    }

    //Exit game
    public void Exit()
    {
        Application.Quit();
        Debug.Log("Quit!");
    }
}
