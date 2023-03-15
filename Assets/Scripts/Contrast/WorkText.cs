using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WorkText : MonoBehaviour
{
    public string ErrorType = "";
    int CurIndex = 1;

    public void ChangeText(string cnt)
    {
        transform.GetChild(CurIndex++).GetComponent<TMP_Text>().text = cnt;
    }
}