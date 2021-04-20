using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Leave : MonoBehaviour
{
    //Text
    public GameObject text;

    // Start is called before the first frame update
    void Start()
    {
        text.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Show button
        text.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //Hide button
        text.SetActive(false);
    }
}
