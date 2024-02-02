using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class VK_PupilExitChecker : MonoBehaviour
{
    [Header("눈동자가 Checker의 밖으로 나갔을 때 발생하는 이벤트")]
    public UnityEvent PupilExitEvent;
    
    private void OnTriggerExit2D(Collider2D other)
    {
        Debug.Log(other.name);
        
        if(other.name == "PupilCollider")
            PupilExitEvent.Invoke();
    }
}
