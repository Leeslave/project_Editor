using UnityEngine;

/// <summary>
/// 씬 시작 진입점.
/// 로그인 화면을 활성화하고, 로그인 성공 후 메인 게임 UI로 전환합니다.
/// </summary>
public class DocGame_SceneInit : MonoBehaviour
{
    [Header("화면 전환")]
    [SerializeField] GameObject loginScreen;
    [SerializeField] GameObject mainScreen;

    [Header("로그인")]
    [SerializeField] DocGame_LoginScreen loginController;

    void Start()
    {
        loginScreen.SetActive(true);
        mainScreen.SetActive(false);

        loginController.OnLoginSuccess += HandleLoginSuccess;
    }

    void OnDestroy()
    {
        if (loginController != null)
            loginController.OnLoginSuccess -= HandleLoginSuccess;
    }

    void HandleLoginSuccess()
    {
        loginScreen.SetActive(false);
        mainScreen.SetActive(true);
    }
}
