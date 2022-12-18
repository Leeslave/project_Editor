using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DateScript : MonoBehaviour
{
    AudioSource audioSource;

    void ScreenSwitch()
    {
        GameObject.FindWithTag("MainCamera").GetComponent<CameraMove>().moveRight();
    }

    void SetTrueMessage()
    {
        Debug.Log("method");
        gameObject.SetActive(true);
        audioSource.Play();
        Invoke("ScreenSwitch", 2);
    }


    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);
        audioSource = GetComponent<AudioSource>();
        Invoke("SetTrueMessage", 2);
        Debug.Log("Play");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
