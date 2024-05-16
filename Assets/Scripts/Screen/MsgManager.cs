using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MsgManager : SingletonObject<MsgManager>
{
    public List<MessageData> messages = new();

    public void SetMessages()
    {
        messages.Clear();

        messages = ObjectDatabase.MessageList;
    }
    
    
}
