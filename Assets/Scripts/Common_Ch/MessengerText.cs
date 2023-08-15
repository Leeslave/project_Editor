using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

public class MessengerText : MonoBehaviour
{
    [SerializeField] TMP_Text Title;
    [SerializeField] TMP_Text Sender;
    [SerializeField] TMP_Text Attatch;
    [SerializeField] GameObject[] Attatchs;
    [SerializeField] TMP_Text[] AttatchName;
    [SerializeField] TMP_Text Main;
    GameObject[] Includes;

    public void OpenContent(string from, string title, string main, GameObject[] includes, string[] includesname)
    {
        int i;
        for (i = 0; i < includes.Length; i++)
        {
            Attatchs[i].SetActive(true);
            AttatchName[i].text = includesname[i];
        }
        Attatch.text = $"Ã·ºÎ {includes.Length}°³";
        Title.text = title;
        Sender.text = from;
        Main.text = main;
        Includes = includes;
    }

    public void OpenIncludes(int num)
    {
        Includes[num].SetActive(true);
    }
    
}
