using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class NextScene : MonoBehaviour
{
    public string Next;
    void Update()
    {
        if (Input.anyKeyDown)
        {
            SceneManager.LoadScene(Next);
        }
    }
}
