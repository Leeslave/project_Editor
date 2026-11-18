using UnityEngine;

public class ExitADFGVX : MonoBehaviour
{
    public void Exit()
    {
        if (GameSystem.Instance == null || GameSystem.Instance.IsLoading) return;
        GameSystem.Instance.EnterScene(ScreenSceneRoute.Name);
    }
}
