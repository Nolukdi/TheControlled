using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prompts : MonoBehaviour
{
    //Player
    public GameObject eli;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Follow player
        gameObject.transform.position = new Vector3(eli.transform.position.x - 3, gameObject.transform.position.y);
    }
}
