using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DebugLog : MonoBehaviour
{
    private TextField mainLogTextField;
    private TextField logTextField;
    private SpriteRenderer logGuideSprite;
    private AudioSource audioSource;
    private BoxCollider2D boxCollider2D;

    private void Start()
    {
        mainLogTextField = transform.Find("MainLogTextField").GetComponent<TextField>();
        logTextField = transform.Find("LogTextField").GetComponent<TextField>();
        logGuideSprite = transform.Find("LogGuideSprite").GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        boxCollider2D = GetComponent<BoxCollider2D>();

        logGuideSprite.size = new Vector2(0.1f, 220);
        logTextField.SetTextColor(Color.clear);
        boxCollider2D.offset = new Vector2(-9.57f, 0f);
        boxCollider2D.size = new Vector2(223f, 9.3f);
    }

    public void SetLayer(int layer)
    {
        this.gameObject.layer = layer;
    }

    public void DebugInfo(string value)
    {
        string timePlusLog = System.DateTime.Now.ToString("[HH:mm:ss]") + "[info] " + value;
        mainLogTextField.SetText(timePlusLog);
        AddDebugLog(timePlusLog);
    }

    public void DebugError(string value)
    {
        string timePlusLog = System.DateTime.Now.ToString("[HH:mm:ss]") + "[error] " + value;
        mainLogTextField.SetText(timePlusLog);
        AddDebugLog(timePlusLog);
        audioSource.Play();
    }

    private void AddDebugLog(string value)
    {
        int count = 0;
        foreach(char c in logTextField.GetText())
        {
            if (c == '\n')
                count++;
        }
        if(count<5)
            logTextField.SetText(logTextField.GetText() + value + "\n");
        else if(count == 5)
        {
            logTextField.SetText(logTextField.GetText().Substring(logTextField.GetText().IndexOf('\n') + 1));
            logTextField.SetText(logTextField.GetText() + value + "\n");
        }
    }

    private void OnMouseEnter()
    {
        logGuideSprite.size = new Vector2(5.3f, 220);
        logTextField.SetTextColor(Color.white);
        boxCollider2D.offset = new Vector2(-9.57f, 16.5f);
        boxCollider2D.size = new Vector2(223f, 42f);
    }

    private void OnMouseExit()
    {
        logGuideSprite.size = new Vector2(0.1f, 220);
        logTextField.SetTextColor(Color.clear);
        boxCollider2D.offset = new Vector2(-9.57f, 0f);
        boxCollider2D.size = new Vector2(223f, 9.3f);
    }
}
