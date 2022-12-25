using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameManage : MonoBehaviour
{
    GameObject child = null;
    // Start is called before the first frame update
    void Start()
    {
        //gameObject.SetActive(false);
    }


    public void FrameOn()
    {
        child = transform.GetChild(0).gameObject;
        Debug.Log("Frame");
        child.SetActive(true);
    }

    public void FrameOff()
    {
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
