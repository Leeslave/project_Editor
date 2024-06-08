using UnityEngine;

public class MessagePanel : MonoBehaviour
{
    public int count;
    public bool isRead;

    public void Awake()
    {
        isRead = false;
    }

    /// <summary>
    /// 메시지 패널 활성화 클릭이벤트
    /// </summary>
    public void Onclicked()
    {
        isRead = true;
        MsgManager.Instance.MsgPanel.parent.parent.gameObject.SetActive(true);
        MsgManager.Instance.SetMessagePanel(count);
    }
}
