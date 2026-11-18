using UnityEngine;
using UnityEngine.EventSystems;

public class ScreenButton : MonoBehaviour, IPointerClickHandler
{
    /**
    *   스크린 활성화, 비활성화 트리거 코드
    *   - 스크린 아이콘을 눌러 스크린을 활성화
    *   - down 버튼을 눌러 스크린 비활성화
    */
    
    public void OnPointerClick(PointerEventData eventData)
    {
        if (GameSystem.Instance == null || GameSystem.Instance.IsLoading) return;
        GameSystem.Instance.EnterScene(ScreenSceneRoute.Name);
    }
}
