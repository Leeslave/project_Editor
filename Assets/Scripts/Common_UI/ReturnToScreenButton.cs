using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class ReturnToScreenButton : PointerButton
{
    [SerializeField] private bool _returnOnEnable = true;
    // 활성화 시 복귀하도록 설정된 경우 업무 화면을 연다.
    private void OnEnable()
    {
        if (!_returnOnEnable) return;
        Return();
    }

    private void Return()
    {
        if (GameSystem.Instance == null || GameSystem.Instance.IsLoading) return;
        GameSystem.Instance.EnterScene(ScreenSceneRoute.Name);
    }

    protected override void Click(PointerEventData data)
    {
        Return();
    }
}
