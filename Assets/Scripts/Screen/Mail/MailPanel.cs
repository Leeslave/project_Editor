using TMPro;
using UnityEngine;

public class MailPanel : MonoBehaviour
{
    public string title;
    public TMP_Text titleUI;
    public bool isActive;

    public void Set(string _title)
    {
        title = _title;
        titleUI.text = _title;
        isActive = false;
    }

    /// <summary>
    /// 메일 활성화/비활성화 클릭 이벤트
    /// </summary>
    public void OnClick()
    {
        if (!isActive)
        {
            MailManager.Instance.ActiveMail(title);
        }
        else
        {
            MailManager.Instance.OffMail();
        }
    }
}
