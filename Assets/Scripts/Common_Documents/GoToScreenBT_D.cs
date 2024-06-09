using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoToScreenBT_D : MonoBehaviour
{
    private void OnEnable()
    {
        if (GameSystem.Instance != null)
            GameSystem.LoadScene("Screen");
        else
            SceneManager.LoadScene("Screen");
    }
}
