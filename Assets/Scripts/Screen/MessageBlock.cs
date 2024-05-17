using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MessageBlock : MonoBehaviour
{
    public TalkParagraph data;

    public TMP_Text name;
    public TMP_Text talk;
    private const float delayTick = 0.1f;

    
    // 첫 생성시 클릭 이벤트 생성
    public void Awake()
    {
        GetComponent<Button>().onClick.AddListener(SetText);
    }

    
    /// <summary>
    /// 대사 실행 (클릭 이벤트 꺼짐)
    /// </summary>
    public void SetText()
    {
        name.text = data.talker;
        GetComponent<Button>().onClick.RemoveAllListeners();
        StartCoroutine(PlayText());
    }

    
    /// <summary>
    /// 대사 출력 (종료 후 다음 대사 버튼 활성화)
    /// </summary>
    /// <returns></returns>
    IEnumerator PlayText()
    {
        talk.text = "";
        float delay = data.textDelay;
        while (delay > 0)
        {
            yield return new WaitForSeconds(delayTick);
            talk.text += ".";
            delay -= delayTick;
        }

        talk.text = data.text;
        Button button = GetComponent<Button>();
        button.onClick.AddListener(MsgManager.Instance.NextMessage);
    }
}

