using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ElementButton : MonoBehaviour
{
    private ADFGVXscript ADFGVX;
    public int row;
    public int line;

    void Awake()
    {
        ADFGVX = GameObject.Find("ADFGVX_Script").GetComponent<ADFGVXscript>();
        this.GetComponentsInChildren<SpriteRenderer>()[1].enabled = false;
    }
    public void ChangeButtonText(char value)
    {
        this.GetComponentInChildren<TextMeshPro>().text = value.ToString();
    }

    void OnMouseDown()
    {
        ADFGVX.OnEncElementDown(row, line);
        this.GetComponentsInChildren<SpriteRenderer>()[1].color = new Color(0, 1, 0);
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
