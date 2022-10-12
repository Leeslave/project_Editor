using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RowButton : MonoBehaviour
{
    private ADFGVXscript ADFGVX;
    public int row;

    void Awake()
    {
        ADFGVX = GameObject.Find("ADFGVX_Script").GetComponent<ADFGVXscript>();
        this.GetComponentsInChildren<SpriteRenderer>()[1].enabled = false;
    }

    private void OnMouseDown()
    {
        ADFGVX.OnDecRowDown(row);        
    }

    private void OnMouseEnter()
    {
        this.GetComponentsInChildren<SpriteRenderer>()[1].enabled = true;
    }

    private void OnMouseExit()
    {
        this.GetComponentsInChildren<SpriteRenderer>()[1].enabled = false;
    }
}
