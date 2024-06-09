using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GoToScreenBT_D : Buttons_M
{
    [SerializeField] bool OnType = true;
    private void OnEnable()
    {
        if (!OnType) return;
        if (GameSystem.Instance != null)
            GameSystem.LoadScene("Screen");
        else
            SceneManager.LoadScene("Screen");
    }

    protected override void Click(PointerEventData Data)
    {
        if (GameSystem.Instance != null)
            GameSystem.LoadScene("Screen");
        else
            SceneManager.LoadScene("Screen");
    }
}
