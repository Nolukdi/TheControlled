using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitToAppear : MonoBehaviour
{
    //Gameobject
    public GameObject trackObj;
    public GameObject currentObj;

    //Booleans
    public bool forDisappear = false;

    // Start is called before the first frame update
    void Start()
    {
        currentObj.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (forDisappear) { WaitForDisappear();}
    }

    void WaitForDisappear()
    {
        //If it disappears
        if (!trackObj.activeSelf)
        {
            //Appear
            currentObj.SetActive(true);
            gameObject.GetComponent<WaitToAppear>().enabled = false;
        }
    }
}
