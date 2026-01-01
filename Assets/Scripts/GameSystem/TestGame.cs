using GameService;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TestGame : MonoBehaviour
{
    IWorkService workService;

    void Start()
    {
        workService = ServiceProvider.Get<IWorkService>();
    }
    
    public void Clear()
    {
        workService.ClearWork("TestGame");
        SceneManager.LoadScene("Screen");
    }
}
