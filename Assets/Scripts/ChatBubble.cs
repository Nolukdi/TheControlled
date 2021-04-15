using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD.Studio;

public class ChatBubble : MonoBehaviour
{
    //FMOD
    private EventInstance enter;
    private PLAYBACK_STATE currentState = PLAYBACK_STATE.STOPPED;

    // Start is called before the first frame update
    void Start()
    {
        enter = FMODUnity.RuntimeManager.CreateInstance("event:/UI/ChatBubble");
        enter.setVolume(0.5f);

        //If not already playing
        if(currentState != PLAYBACK_STATE.PLAYING)
        {
            //Play
            enter.start();
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Get if alreadying playing
        enter.getPlaybackState(out currentState);
    }
}
