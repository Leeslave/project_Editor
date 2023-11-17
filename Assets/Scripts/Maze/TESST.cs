using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class TESST : MonoBehaviour
{
    private void Start()
    {
        GetComponent<TMP_Text>().text = "Clear : " + File.Exists("Assets\\Resources\\GameData\\Maze\\C");
    }
}
