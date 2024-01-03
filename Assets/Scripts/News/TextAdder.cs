using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Rendering.Universal;
using UnityEngine;
using UnityEngine.UI;

public class TextAdder : MonoBehaviour
{
    [SerializeField] List<TMP_Text> Revises;
    [SerializeField] List<GameObject> RevisesBack;
    [SerializeField] TMP_InputField Field;
    [SerializeField] Button AddBT;

    private void Awake()
    {
        Field.onEndEdit.AddListener(Enter);
        AddBT.onClick.AddListener(Add);
    }
    void Enter(string text)
    {
        if (Input.GetKey(KeyCode.Return) || Input.GetKey(KeyCode.KeypadEnter))
        {
            Field.text += "\n";
            Field.MoveTextEnd(false);
            Field.ActivateInputField();
        }
    }

    void Add()
    {
        if (Field.text.Length == 0)
        {
            print("Error \"10597\" : Enter Text!");
            return;
        }
        for(int i = 0; i < RevisesBack.Count; i++)
        {
            if (!RevisesBack[i].activeSelf)
            {
                RevisesBack[i].transform.SetAsLastSibling();
                RevisesBack[i].SetActive(true);
                Revises[i].text = Field.text;
                Field.text = "";
                return;
            }
        }
        print("Error \"10597\" : No Enough Storage!");
    }
}
