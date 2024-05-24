using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Hint : MonoBehaviour
{
    public GameObject text;
    public void Activate() { text.SetActive(true); }
    private void Start()
    {
        text.SetActive(false);
    }
}
