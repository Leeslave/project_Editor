using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ArrayInfo : MonoBehaviour
{
    public void UpdateText(string value)
    {
        GetComponentInChildren<TextMeshPro>().text = value;
        return;
    }
}
