using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Train : MonoBehaviour
{
    //Counter
    public float seconds = 0.0f;

    //Move
    public bool move = false;

    // Start is called before the first frame update
    void Start()
    {
        //Start off screen
        gameObject.transform.position = new Vector3(-16, gameObject.transform.position.y, gameObject.transform.position.z);
    }

    // Update is called once per frame
    void Update()
    {
        //If enough time has passed
        if(seconds % 60 >= 0 && seconds % 60 <= 0.01)
        {
            //Move the train across the screen
            move = true;
        }

        //If moving
        if(move)
        {
            //Increase x
            gameObject.transform.position = new Vector3(gameObject.transform.position.x + 0.25f, gameObject.transform.position.y, gameObject.transform.position.z);
        }

        //If the train has moved across the screen
        if(gameObject.transform.position.x > 18)
        {
            //Reset
            move = false;
            gameObject.transform.position = new Vector3(-16, gameObject.transform.position.y, gameObject.transform.position.z);
        }

        //Count seconds
        seconds += Time.deltaTime;
    }
}
