using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChoiceInput : MonoBehaviour
{
    public ChatEditor editor;
    [HideInInspector]
    public int index = 0;
    
    public List<Transform> choices = new();
    
    public List<TMP_Dropdown> characters;
    public TMP_Dropdown bgm;
    public TMP_Dropdown background;
    public TMP_Dropdown action;
    public TMP_InputField actionParam;


    public void SubmitInput()
    {
        
        
    }


    public void LoadInput(ChoiceParagraph data)
    {
        
    }
    
    
    private static int FindIndex(TMP_Dropdown dropdown, string targetString)
    {
        if (String.IsNullOrEmpty(targetString))
            targetString = "none";
        
        for (int i = 0; i < dropdown.options.Count; i++)
        {
            if (dropdown.options[i].text == targetString)
            {
                return i;
            }
        }

        // 일치하는 항목이 없는 경우 처리
        return 0;
    }
}
