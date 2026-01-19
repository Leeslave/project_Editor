using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitADFGVX : MonoBehaviour
{
    public void Exit()
    {
        StartCoroutine(ExitCoroutine());
    }

    private IEnumerator ExitCoroutine()
    {
        var loadScene = SceneManager.LoadSceneAsync("Screen", LoadSceneMode.Additive);
        yield return new WaitUntil(() => loadScene.isDone);
        SceneManager.UnloadSceneAsync("ADFGVX");
    }
}
