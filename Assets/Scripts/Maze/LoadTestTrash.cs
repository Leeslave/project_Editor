using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadTestTrash : MonoBehaviour
{
    public static LoadTestTrash Instance;
    [SerializeField] bool Test;
    [SerializeField] public string LoadScene;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this; DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    public void SceneLoad()
    {
        StartCoroutine(SceneLoadT());
    }

    public Image ProgressBar;
    IEnumerator SceneLoadT()
    {
        if (Test) PlayerPrefs.SetInt("DocumentTest", 1);
        else PlayerPrefs.SetInt("DocumentTest", 0);
        var j = SceneManager.LoadSceneAsync(LoadScene); j.allowSceneActivation = false;
        while (!j.isDone)
        {
            ProgressBar.fillAmount = j.progress;
            if(j.progress >= 0.9f)
            {
                ProgressBar.fillAmount = 1;
                if(Input.GetKeyDown(KeyCode.Space)) j.allowSceneActivation = true;
            }
            yield return null;
        }
    }
}
