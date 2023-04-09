using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugUI : MonoBehaviour
{
    public string debugMessage;
    public void OnTestEvent()
    {
        Debug.Log(debugMessage);
    }
}
