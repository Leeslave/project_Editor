using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MessengerTab : MonoBehaviour
{
    [SerializeField] Messenger_M MM;
    [SerializeField] TMP_Text From;
    [SerializeField] TMP_Text Title;
    [SerializeField] int ind;
    Image image;
    public bool IsOpened;
    

    Color N = new Color(0.8f,0.8f,0.8f);
    Color Y = new Color(0.95f,0.95f,0.95f);

    private void Awake()
    {
        image = GetComponent<Image>();
        MyUi.AddEvent(GetComponent<EventTrigger>(),EventTriggerType.PointerClick,Click);
    }

    public void NewTab(string from, string title)
    {
        gameObject.SetActive(true);
        From.text = from;
        Title.text = title;
    }

    void Click(PointerEventData Data)
    {
        if (Data.clickCount == 2)
        {
            MM.OpenMessage(ind);
            image.color = Y;
        }
    }

    

    private void OnEnable()
    {
        image.color = N;
        IsOpened = false;
    }

}
