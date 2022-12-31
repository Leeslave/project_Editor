using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackToMain : MonoBehaviour
{
    public GameObject MainCamera;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void backToMain()
    {
        gameObject.SetActive(false);
        GameObject.FindWithTag("Screen").GetComponent<MainSceneManager>().MainUIOn();
        MainCamera.GetComponent<CameraMove>().moveToMain();
    }
}
