using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainSceneManager : MonoBehaviour
{
    //public GameObject MainUI;

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        MainUIOn();
    }
    public void MainUIOn()
    {
        gameObject.SetActive(true);
    }

    public void MainUIOff()
    {
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
