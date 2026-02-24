using TMPro;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class TutorialMessageBox : MonoBehaviour
{
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private TMP_Text messageText;

    [Header("Size")]
    [SerializeField] private float maxWidth  = 300f;
    [SerializeField] private float paddingH  = 16f;   // 좌우 패딩
    [SerializeField] private float paddingV  = 12f;   // 상하 패딩

    private void Reset()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void Show(string message)
    {
        string text = message.Replace("\\n", "\n");
        messageText.text = text;
        gameObject.SetActive(true);

        // 텍스트 기준으로 박스 크기를 직접 계산 (World Space Canvas에서 ContentSizeFitter 대체)
        float textMaxW = maxWidth - paddingH * 2f;
        Vector2 pref   = messageText.GetPreferredValues(text, textMaxW, float.PositiveInfinity);

        float w = Mathf.Min(pref.x, textMaxW) + paddingH * 2f;
        float h = pref.y                       + paddingV * 2f;

        rectTransform.sizeDelta = new Vector2(w, h);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public Vector2 GetSize()
    {
        return rectTransform.sizeDelta;
    }

    public void SetPosition(Vector2 canvasPosition)
    {
        rectTransform.anchoredPosition = canvasPosition;
    }
}
