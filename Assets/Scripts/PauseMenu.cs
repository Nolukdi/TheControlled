using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FMOD.Studio;

public class PauseMenu : MonoBehaviour
{
    //Menus
    public GameObject mainMenu;
    public GameObject optionsMenu;
    public GameObject audioMenu;

    //Sliders
    public Slider sfx;
    public Slider music;

    public bool paused = false;

    Bus bus;

    // Start is called before the first frame update
    void Start()
    {
        mainMenu.SetActive(false);
        optionsMenu.SetActive(false);
        audioMenu.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        //If player presses excape
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //Show menu and pause game
            mainMenu.SetActive(true);
            Time.timeScale = 0;
            paused = true;
        }
    }

    //Start game
    public void StartGame()
    {
        //Resume gameplay
        Time.timeScale = 1;
        mainMenu.SetActive(false);
        paused = false;
    }

    //Options menu
    public void Options()
    {
        optionsMenu.SetActive(true);
        mainMenu.SetActive(false);
    }

    //Back
    public void Back()
    {
        optionsMenu.SetActive(false);
        mainMenu.SetActive(true);
    }

    //Exit game
    public void Exit()
    {
        Application.Quit();
        Debug.Log("Quit!");
    }

    //Opens audio menu
    public void Audio()
    {
        //Change to audio menu
        audioMenu.SetActive(true);
        optionsMenu.SetActive(false);
    }

    //Closes audio menu
    public void AudioBack()
    {
        //Change out of audio menu
        audioMenu.SetActive(false);
        optionsMenu.SetActive(true);
    }

    //Applies sfx settings
    public void SFXSlider()
    {
        MainMenu.sfxVol = sfx.value;
    }

    //Applies music settings
    public void MusicSlider()
    {
        MainMenu.musicVol = music.value;
    }
}
