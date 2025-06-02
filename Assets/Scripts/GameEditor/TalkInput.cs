using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TalkInput : MonoBehaviour
{
    public ChatEditor editor;
    [HideInInspector]
    public int index = 0;
    
    public TMP_InputField talker;
    public TMP_InputField talkerInfo;
    public TMP_InputField context;
    public TMP_Dropdown fontSize;
    public TMP_InputField textDelay;
    
    public List<Transform> characters;
    public TMP_Dropdown bgm;
    public TMP_Dropdown background;
    public TMP_Dropdown action;
    public TMP_InputField actionParam;


    public void SubmitInput()
    {
        TalkParagraph data = new(talker.text, talkerInfo.text, context.text);
        
        data.fontSize = fontSize.options[fontSize.value].text;
        data.textDelay = float.Parse(textDelay.text);
        
        data.bgm = bgm.options[bgm.value].text;
        if (background.options[background.value].image == null)
        {
            data.background = "none";
        }
        else
        {
            data.background = background.options[background.value].image.name;
        }
        data.action = action.options[action.value].text;
        data.actionParam = actionParam.text;
        
        // CG 연결
        for (int i = 0; i < 4; i++)
        {
            var charPanel = characters[i];
            var charName = charPanel.GetChild(0).GetComponent<TMP_Dropdown>();
            if (charName.value != 0)
            {
                CharacterCG newChar = new();
                newChar.fileName = charName.options[charName.value].text;
                if (int.TryParse(charPanel.GetChild(1).GetComponent<TMP_InputField>().text, out int num))
                {
                    newChar.index = num;
                }
                newChar.isHighlight = charPanel.GetChild(2).GetComponent<Toggle>().isOn;
                
                data.characters[i] = newChar;
            }
            else
            {
                data.characters[i] = new CharacterCG();
            }
        }
        
        // 데이터 제출
        editor.SaveParagraph(index, data);
    }


    public void LoadInput(TalkParagraph data)
    {
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
        for (int i = 0; i < 4; i++)
        {
            var charPanel = characters[i];
            if (string.IsNullOrEmpty(data.characters[i].fileName))
            {
                charPanel.GetChild(0).GetComponent<TMP_Dropdown>().value = 0;
                charPanel.GetChild(1).GetComponent<TMP_InputField>().text = "0";
                charPanel.GetChild(2).GetComponent<Toggle>().isOn = false;
            }
            else
            {
                TMP_Dropdown dropdown = characters[i].GetChild(0).GetComponent<TMP_Dropdown>();
                dropdown.value = FindIndex(dropdown, data.characters[i].fileName);
                charPanel.GetChild(1).GetComponent<TMP_InputField>().text = data.characters[i].index.ToString();
                charPanel.GetChild(2).GetComponent<Toggle>().isOn = data.characters[i].isHighlight;
            }
        }
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

            // image가 null이 아닌 경우에만 name 속성에 접근
            if (dropdown.options[i].image != null)
            {
                if (dropdown.options[i].image.name == targetString)
                {
                    return i;
                }
            }
        }
        // 일치하는 항목이 없는 경우 처리
        return 0;
    }
}
