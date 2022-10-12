using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineButton : MonoBehaviour
{
    private ADFGVXscript ADFGVX;
    public int line;

    void Awake()
    {
        ADFGVX = GameObject.Find("ADFGVX_Script").GetComponent<ADFGVXscript>();
        GetComponentsInChildren<SpriteRenderer>()[1].enabled = false;
    }

    private void OnMouseDown()
    {
        ADFGVX.OnDecLineDown(line);
        
    }

    private void OnMouseEnter()
    {
        GetComponentsInChildren<SpriteRenderer>()[1].enabled = true;
    }

    private void OnMouseExit()
    {
        GetComponentsInChildren<SpriteRenderer>()[1].enabled = false;
    }
}

