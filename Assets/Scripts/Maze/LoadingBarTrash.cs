using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingBarTrash : MonoBehaviour
{
    private void Start()
    {
        LoadTestTrash.Instance.ProgressBar = GetComponent<Image>();
        LoadTestTrash.Instance.SceneLoad();
    }
}
