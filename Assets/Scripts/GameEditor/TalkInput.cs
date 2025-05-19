using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TalkInput : MonoBehaviour
{
    public ChatEditor editor;
    public int index = 0;
    
    public TMP_InputField talker;
    public TMP_InputField talkerInfo;
    public TMP_InputField context;

    public TMP_Dropdown fontSize;
    public TMP_InputField textDelay;
    
    public List<TMP_Dropdown> characters;
    public TMP_Dropdown bgm;
    public TMP_Dropdown background;
    public TMP_Dropdown action;
    public TMP_InputField actionParam;


    public void SubmitInput()
    {
        TalkParagraph data = new(talker.text, talkerInfo.text,  context.text);
        
        data.fontSize = fontSize.options[fontSize.value].text;
        data.textDelay = float.Parse(textDelay.text);
        
        data.bgm = bgm.options[bgm.value].text;
        data.background = background.options[background.value].text;
        data.action = action.options[action.value].text;
        data.actionParam = actionParam.text;
        
        // CG 연결
        
        
        
    }


    public void LoadInput(TalkParagraph data)
    {
        Debug.Log(data);
        // 대사
        talker.text = data.talker;
        talkerInfo.text = data.talkerInfo;
        context.text = data.text;
        
        // 대사 추가 정보
        fontSize.value = FindIndex(fontSize, data.fontSize);
        textDelay.text = data.textDelay.ToString();
        
        // 기타 정보
        bgm.value = FindIndex(bgm, data.bgm);
        background.value = FindIndex(background, data.background);
        action.value = FindIndex(action, data.action);
        actionParam.text = data.actionParam;
        
        // CG 연결
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
