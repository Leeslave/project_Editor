using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 문서 대조 미니게임 - 로그인 화면
/// 씬 시작 시 표시되며, 로그인 성공 시 OnLoginSuccess 이벤트를 발생시킵니다.
/// </summary>

public class DocGame_LoginScreen : MonoBehaviour
{
    [Header("입력 필드")]
    [SerializeField] TMP_InputField idField;
    [SerializeField] TMP_InputField pwField;

    [Header("피드백 텍스트")]
    [SerializeField] TMP_Text statusText;

    [Header("버튼")]
    [SerializeField] Button loginButton;

    [Header("기본 계정 정보")]
    [SerializeField] string correctId;
    [SerializeField] string correctPw;


    // 로그인 성공 시 외부에서 구독
    public event Action OnLoginSuccess;

    static readonly string ColorRed   = "#FF4444";
    static readonly string ColorGreen = "#44FF88";

    void Start()
    {
        loginButton.onClick.AddListener(TryLogin);
        pwField.onSubmit.AddListener(_ => TryLogin());
        statusText.text = "";
    }


    void TryLogin()
    {
        string id = idField.text.Trim();
        string pw = pwField.text;

        if (id == correctId && pw == correctPw)
        {
            statusText.text = Colored("ACCESS GRANTED", ColorGreen);
            loginButton.interactable = false;
            idField.interactable = false;
            pwField.interactable = false;
            StartCoroutine(LoginAccess());
        }
        else
        {
            statusText.text = Colored("ID또는 PW가 잘못되었습니다.", ColorRed);
            pwField.text = "";
            pwField.Select();
        }
    }

    private IEnumerator LoginAccess()
    {
        yield return new WaitForSeconds(2f);
        OnLoginSuccess?.Invoke();
    }

    static string Colored(string text, string hexColor) => $"<color={hexColor}>{text}</color>";
}
