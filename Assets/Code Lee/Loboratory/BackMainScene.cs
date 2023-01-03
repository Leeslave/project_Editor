using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackMainScene : MonoBehaviour
{
    public GameObject MainUI;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void backMain()
    {

        SceneManager.UnloadSceneAsync("TempScene");
        Debug.Log(GameObject.FindWithTag("MainUI"));
        GameObject.FindWithTag("MainUI").transform.Find("Screen").gameObject.SetActive(true);
        GameObject.FindWithTag("MainGameSystem").transform.Find("Main Camera").gameObject.SetActive(true);
        GameObject.FindWithTag("MainGameSystem").transform.Find("EventSystem").gameObject.SetActive(true);
        //MainUI.GetComponent<MainSceneManager>().MainUIOn();
        //SceneManager.LoadSceneAsync("Lobortory");
    }
}
