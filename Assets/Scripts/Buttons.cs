using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using FMOD.Studio;
using UnityEngine;

public class Buttons : MonoBehaviour
{
    //FMOD
    private EventInstance mouseOver;
    private PLAYBACK_STATE currentState;

    // Start is called before the first frame update
    void Start()
    {
        mouseOver = FMODUnity.RuntimeManager.CreateInstance("event:/UI/MenuSelect"); //Grab sound
    }

    // Update is called once per frame
    void Update()
    {
        mouseOver.setVolume(MainMenu.sfxVol);
        mouseOver.getPlaybackState(out currentState);
    }

    public void OnMouseEnter()
    {
        //Trigger sound
        mouseOver.start();
    }
}
