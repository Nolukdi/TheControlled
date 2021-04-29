using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jay : MonoBehaviour
{
    //Related choices
    public static int relationship = 10;

    //Gameobjects
    public GameObject eli;

    //Talking states
    public bool talking = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (gameObject.transform.position.x - eli.transform.position.x < 0)
        {
            gameObject.transform.rotation = new Quaternion(0, 0, 0, gameObject.transform.rotation.w);
        }

        else
        {
            gameObject.transform.rotation = new Quaternion(0, 180, 0, gameObject.transform.rotation.w);
        }
    }
}
