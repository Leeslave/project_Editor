using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TestGame : MonoBehaviour
{
    public void Clear()
    {
        GameSystem.Instance.ClearWork("TestGame");
        SceneManager.LoadScene("Screen");
    }
}
