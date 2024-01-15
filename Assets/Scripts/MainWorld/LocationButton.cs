using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationButton : MonoBehaviour
{
    /**
     지역 이동 버튼 (문)
     - 지역과 위치 설정시 해당 지역의 해당 위치로 이동
     - 접근 불가 지역이면 접근 불가
        접근 제한 메시지 있으면 출력
    */
    [SerializeField]
    private World location;     // 이동할 지역
    [SerializeField]
    private int position;       // 이동할 지역 내 위치
    [SerializeField]
    private bool isOpen;        // 접근 가능 여부
    [SerializeField]
    private List<string> texts = new();
    private List<Paragraph> message;

    void Awake()
    {
        if (texts.Count > 0)
        {
            foreach (var text in texts)
            {
                message.Add(new TalkParagraph(text));
            }
        }
    }

    /// <summary>
    /// 장소 이동하기
    /// </summary>
    public void MoveLocation()
    {
        if (isOpen == false)
        {
            RestrictLocation();
            return;
        }
        WorldSceneManager.Instance.SetPosition(position);
        WorldSceneManager.Instance.MoveLocation(location);
    }


    /// <summary>
    /// 제한된 장소에 접근
    /// </summary>
    private void RestrictLocation()
    {
        if (message.Count > 0)
            Chat.Instance.StartChat(message);
    }
}
