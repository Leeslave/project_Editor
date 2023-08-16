using UnityEngine;

public class Messenger_M : MonoBehaviour
{
    [SerializeField] MessengerIcon MI;
    [SerializeField] MessengerTab[] Tabs;
    int ActiveTab = 0;

    public void NewMessage(string from, string title, string main, GameObject[] includes, string[] includesname)
    {
        Tabs[ActiveTab++].NewTab(from,title,main,includes,includesname);
        MI.ChangeCount(1);
    }
    public void CloseMessage()
    {
        MI.ChangeCount(-1);
    }

    
}
