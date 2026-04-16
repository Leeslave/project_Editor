using GameService;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TestGame : MonoBehaviour
{
    public void Clear()
    {
        workService.ClearWork("TestGame");
        StartCoroutine(LoadScreen());
    }

    private IEnumerator LoadScreen()
    {
        var loadScene = SceneManager.LoadSceneAsync("Screen", LoadSceneMode.Additive);
        yield return new WaitUntil(() => loadScene.isDone);
        SceneManager.UnloadSceneAsync("TestGame");
    }
}
