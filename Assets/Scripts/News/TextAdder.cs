using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextAdder : MonoBehaviour
{
    [SerializeField] GetOptionFile_D GD;
    [SerializeField] TMP_InputField Field;

    private void Awake()
    {
        Field.onEndEdit.AddListener(Enter);
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
}
