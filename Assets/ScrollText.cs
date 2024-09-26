using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScrollText : MonoBehaviour
{
    public ScrollRect scrollRect;   // Scroll View의 ScrollRect 컴포넌트
    public TextMeshProUGUI contentText; // 또는 일반 Text를 사용하려면 Text로 변경
    private string messageLog = ""; // 전체 텍스트를 저장하는 변수

    public bool forceScroll = true;

    // 새로운 텍스트를 추가하는 함수
    public void AddMessage(string newMessage)
    {
        // 기존 텍스트에 새 텍스트 추가
        messageLog += newMessage + "\n";
        contentText.text = messageLog;

        Canvas.ForceUpdateCanvases();  // 즉시 레이아웃 업데이트 강제
        
        // 프레임 끝에서 스크롤을 업데이트하도록 요청
        if (forceScroll)
        {
            scrollRect.verticalNormalizedPosition = 0f; // 스크롤을 제일 아래로 설정
            Canvas.ForceUpdateCanvases(); // 다시 강제 업데이트
        }
    }
}