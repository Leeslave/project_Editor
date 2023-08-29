using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class MessengerIcon : UIICons
{
    [SerializeField] TMP_Text Text;
    int NewCount = 0;
    string[] Count = { "��", "��", "��", "��", "��", "��", "��", "��", "��" };

    // UIIcon�� ����.
    protected override void Start()
    {
        base.Start();
        Text.GetComponent<RectTransform>().anchoredPosition =
            new Vector2(ImageRect.sizeDelta.x * 0.5f, ImageRect.anchoredPosition.y - ImageRect.sizeDelta.y * 0.5f);
        Text.fontSize = MyText.fontSize * 2;
    }


    // UIIcon�� ��������� ���μ��� ���� �߿��� Icon�� ��ȣ�ۿ��� �����ϸ�, ���� Ŭ�� �� ���� ų �� �ִ�.
    protected override void OpenIcon(PointerEventData Data)
    {
        if (DragDoubleCheck)
        {
            DragDoubleCheck = false;
            return;
        }
        if (Data.clickCount == 2)
        {
            OpenedProcess.SetActive(!OpenedProcess.activeSelf);
            ClickEvent();
        }
    }

    /// <summary>
    /// ���� ���� ���� �޼����� ���� ������ ���� �ϴܿ� ��Ÿ����.
    /// </summary>
    /// <param name="Change"> ��ȭ�� </param>
    public void ChangeCount(int Change)
    {
        NewCount += Change;
        if (NewCount < 0) { NewCount = 0; return; }
        else if (NewCount == 0) Text.gameObject.SetActive(false);
        else 
        {
            if (!Text.gameObject.activeSelf) Text.gameObject.SetActive(true);
            Text.text = Count[NewCount-1];
        }
    }
}
