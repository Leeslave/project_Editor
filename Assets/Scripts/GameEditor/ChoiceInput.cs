using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChoiceInput : MonoBehaviour
{
    public ChatEditor editor;
    [HideInInspector]
    public int index = 0;
    
    public List<Transform> choices = new();
    
    public List<Transform> characters;
    public TMP_Dropdown background;


    public void SubmitInput()
    {
        ChoiceParagraph data = new();
        
        if (background.options[background.value].image == null)
        {
            data.background = "none";
        }
        else
        {
            data.background = background.options[background.value].image.name;
        }
        
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
        
        // 선택지 연결
        for (int i = 0; i < 3; i++)
        {
            var choicePanel = choices[i];
            var choiceText = choicePanel.GetChild(0).GetComponent<TMP_InputField>();
            if (choiceText.text.Length == 0)
            {
                continue;
            }
            Choice newChoice;
            newChoice.text = choiceText.text;
            newChoice.isEnding = choicePanel.GetChild(1).GetComponent<Toggle>().isOn;
            TMP_Dropdown choiceReaction = choicePanel.GetChild(2).GetComponent<TMP_Dropdown>();
            newChoice.reaction = choiceReaction.options[choiceReaction.value].text;
            newChoice.reactionParam = choicePanel.GetChild(3).GetComponent<TMP_InputField>().text;
            
            data.choiceList[i] = newChoice;
        }
        
        // 데이터 제출
        editor.SaveParagraph(index, data);
    }


    public void LoadInput(ChoiceParagraph data)
    {
        background.value = FindIndex(background, data.background);
        
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

        // 선택지 연결
        for (int i = 0; i < 3; i++)
        {
            if (string.IsNullOrEmpty(data.choiceList[i].text))
            {
                continue;
            }
            Transform choicePanel = choices[i];
            choicePanel.GetChild(0).GetComponent<TMP_InputField>().text = data.choiceList[i].text;
            choicePanel.GetChild(1).GetComponent<Toggle>().isOn = data.choiceList[i].isEnding;
            TMP_Dropdown reactionDropdown = choicePanel.GetChild(2).GetComponent<TMP_Dropdown>();
            reactionDropdown.value = FindIndex(reactionDropdown, data.choiceList[i].reaction);
            choicePanel.GetChild(3).GetComponent<TMP_InputField>().text = data.choiceList[i].reactionParam;
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
        }

        // 일치하는 항목이 없는 경우 처리
        return 0;
    }
}
