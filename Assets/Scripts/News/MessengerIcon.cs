using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class MessengerIcon : UIICons
{
    [SerializeField] GameObject Messenger;
    [SerializeField] TMP_Text Text;
    int NewCount = 0;
    string[] Count = { "¨ç", "¨è", "¨é", "¨ê", "¨ë", "¨ì", "¨í", "¨î", "¨ï" };

    protected override void Awake()
    {
        base.Awake();

    }

    protected override void ClickEvent(PointerEventData Data)
    {
        if (DragDoubleCheck)
        {
            DragDoubleCheck = false;
            return;
        }
        if (Data.clickCount == 2) Messenger.SetActive(true);
    }

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
