using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Info : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void UpdateText(string value)
    {
        GetComponentInChildren<TextMeshPro>().text = value;
        return;
    }
}
