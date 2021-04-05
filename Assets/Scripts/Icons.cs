using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Icons : MonoBehaviour
{
    //Chat bubble
    public GameObject chat;
    bool first = true;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //If the player enters
        if (other.CompareTag("Player"))
        {
            //If first time
            if(first)
            {
                //Play initial animation
                gameObject.GetComponent<Animator>().Play(0);
            }

            //View icon
            first = false;
            gameObject.GetComponent<Animator>().SetBool("leaving", false);
            gameObject.GetComponent<SpriteRenderer>().enabled = true;
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        //If player clicks on icon
        if(Input.GetMouseButtonDown(0) && Input.mousePosition == gameObject.transform.position)
        {
            //Chat about it

        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        //Hide icon
        gameObject.GetComponent<Animator>().SetBool("leaving", true);
    }
}
