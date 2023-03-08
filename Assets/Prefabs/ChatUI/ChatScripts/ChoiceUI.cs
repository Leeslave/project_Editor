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

    /// <summary>
    /// 선택지 클릭 시 선택지 번호 반환
    /// </summary>
    public void OnChoiceDown()
    {
        chatUI.OnChoiceDown(this.choiceNum);
    }
}
