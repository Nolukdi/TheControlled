using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using UnityEngine;

public class CityStreet1 : MonoBehaviour
{
    //FMOD
    private EventInstance helpHead;
    private PLAYBACK_STATE currentHeadState;
    private EventInstance cityAmb;
    private PLAYBACK_STATE cityAmbState;

    //Seconds
    public float seconds = 0.0f;

    //Gameobjects
    public GameObject eli;
    public GameObject train;

    //Tracking
    public float exChatter = 0;
    public bool chatter = false;

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
    }

    // Update is called once per frame
    void Update()
    {
        //Get playing state
        helpHead.getPlaybackState(out currentHeadState);
        cityAmb.getPlaybackState(out cityAmbState);
        cityAmb.getParameterByName("ExtraChatter", out exChatter);

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
        
        //Count seconds
        seconds += Time.deltaTime;
    }
}
