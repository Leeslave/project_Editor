using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class InterChiper : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void AddText(string value)
    {
        GetComponentInChildren<TextMeshPro>().text += value;
        return;
    }

    public void ClearText()
    {
        GetComponentInChildren<TextMeshPro>().text = "";
        return;
    }
}
