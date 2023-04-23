using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugUI : MonoBehaviour
{
    public void OnButtonClicked()
    {
        Debug.Log(gameObject.name + "Clicked!");
    }
}
