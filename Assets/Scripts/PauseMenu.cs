using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD.Studio;

public class PauseMenu : MonoBehaviour
{
    //Menus
    public GameObject mainMenu;
    public GameObject optionsMenu;

    public bool paused = false;

    Bus bus;

    // Start is called before the first frame update
    void Start()
    {
        mainMenu.SetActive(false);
        optionsMenu.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
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
}
