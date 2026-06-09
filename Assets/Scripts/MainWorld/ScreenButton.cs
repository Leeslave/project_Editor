using UnityEngine;
using UnityEngine.EventSystems;

public class ScreenButton : MonoBehaviour, IPointerClickHandler
{
    /**
    *   스크린 활성화, 비활성화 트리거 코드
    *   - 스크린 아이콘을 눌러 스크린을 활성화
    *   - down 버튼을 눌러 스크린 비활성화
    */
    
    private bool enter = false;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (enter) return;
        
        enter = true;
        // 스크린 활성화
        GameSystem.Instance.EnterScene("Screen", () =>
        {
            enter = false;
        });
    }
}
