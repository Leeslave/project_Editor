using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class MessengerIcon : UIICons
{
    [SerializeField] TMP_Text Text;
    int NewCount = 0;
    string[] Count = { "①", "②", "③", "④", "⑤", "⑥", "⑦", "⑧", "⑨" };

    // UIIcon과 동일.
    protected override void Start()
    {
        base.Start();
        Text.GetComponent<RectTransform>().anchoredPosition =
            new Vector2(ImageRect.sizeDelta.x * 0.5f, ImageRect.anchoredPosition.y - ImageRect.sizeDelta.y * 0.5f);
        Text.fontSize = MyText.fontSize * 2;
    }


    // UIIcon과 비슷하지만 프로세스 실행 중에도 Icon과 상호작용이 가능하며, 더블 클릭 시 끄고 킬 수 있다.
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
    /// 현재 읽지 않은 메세지의 수를 아이콘 우측 하단에 나타낸다.
    /// </summary>
    /// <param name="Change"> 변화량 </param>
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
