using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using UnityEditor.Build;

public class MainText_N : MonoBehaviour
{
    [SerializeField] TextMannager_N TM;
    [SerializeField] TMP_Text Text;
    [SerializeField] RectTransform ReBuild;
    [SerializeField] bool IsNews;
    /*[NonSerialized]*/
    public int MyInd;
    [NonSerialized] public bool OnButton;
    TMP_InputField Field;
    [SerializeField] TMP_Text FieldText;
    [SerializeField] int DocsType; // 0 : N, 1 : Docs
    RectTransform MyRect;
    char LastChar = ' ';
    Vector2 Sub = new Vector3(0, 20);

    private void Awake()
    {
        MyRect = GetComponent<RectTransform>();
        Field = GetComponent<TMP_InputField>();
        Field.onEndEdit.AddListener(Enter);
        Field.onValueChanged.AddListener(Delete);
    }

    bool GetEnter = false;
    void Enter(string text)
    {
        if (!OnButton)
        {
            Text.text = Field.text;
            gameObject.SetActive(false);
            Text.gameObject.SetActive(true);
            //LayoutRebuilder.ForceRebuildLayoutImmediate(ReBuild);
        }
    }
    void Delete(string text)
    {
        if (text.Length != 0)
        {
            MyRect.sizeDelta = new Vector2(MyRect.sizeDelta.x, Field.preferredHeight + 20);
            LayoutRebuilder.ForceRebuildLayoutImmediate(ReBuild);
        }
    }


    public void AddUnder()
    {
        TM.ActiveText("Empty", MyInd);
    }

    public void DelSelf()
    {
        TM.RemoveText(MyInd);
    }


    public void DelLine()
    {
        Text.text = "";
        Field.text = "";
        MyRect.sizeDelta = new Vector2(550, 40);
    }
    public void AddLine(string text, int Ind)
    {
        Text.text = text;
        MyInd = Ind;
    }

    private void OnEnable()
    {
        GetEnter = false;
        OnButton = false;
        Field.ActivateInputField();
        Field.text = Text.text;
        LayoutRebuilder.ForceRebuildLayoutImmediate(ReBuild);
        MyRect.sizeDelta = new Vector2(MyRect.sizeDelta.x, Field.preferredHeight + 20);
        Field.MoveTextEnd(false);
    }

    private void OnDisable()
    {
        Text.text = Field.text;

        gameObject.SetActive(false);
        Text.gameObject.SetActive(true);
        CheckMyText();
        LayoutRebuilder.ForceRebuildLayoutImmediate(ReBuild);
    }

    public void CheckMyText()
    {
        TM.ValidText(IsNews, transform.parent.GetSiblingIndex() - 4, Text.text);
    }
}
