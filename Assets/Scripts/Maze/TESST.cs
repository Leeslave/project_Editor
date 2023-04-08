using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TESST : MonoBehaviour
{
    private void Start()
    {
        if (PlayerPrefs.HasKey("Clear"))
        {
            GetComponent<TMP_Text>().text = "Clear : " + PlayerPrefs.GetString("Clear");
        }
    }
}
