using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChoiceUI : MonoBehaviour
{
    private ChatUI chatUI;
    public int choiceNum;

    private void Start()
    {
        chatUI = GameObject.FindObjectOfType<ChatUI>();
    }

    public void OnChoiceDown()
    {
        chatUI.OnChoiceDown(this.choiceNum);
    }
}
