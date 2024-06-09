using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class BatchField_D : MonoBehaviour
{
    [SerializeField] protected bool IsWindow = false;

    protected bool AttatchAble = true;
    
    private void Awake()
    {
        if (GetComponent<EventTrigger>() == null) gameObject.AddComponent<EventTrigger>();
        EventTrigger ET = GetComponent<EventTrigger>();
        MyUi.AddEvent(ET,EventTriggerType.PointerEnter,(PointerEventData Data) => DB_M.DB_Docs.CntFileForAttach.Attatch = BatchAccess);
        MyUi.AddEvent(ET, EventTriggerType.PointerExit, OutPointer);
    }

    void OutPointer(PointerEventData Data)
    {
        if (DB_M.DB_Docs.CntFileForAttach.Attatch == this.BatchAccess) DB_M.DB_Docs.CntFileForAttach.Attatch = null;
    }

    void BatchAccess()
    {
        if (DB_M.DB_Docs.CntFileForAttach.AttatchType >=0)
        {
            if (!AttatchAble) return;
            AttatchAble = false;
            switch (DB_M.DB_Docs.CntFileForAttach.AttatchType)
            {
                case 1: StartCoroutine(BatchType1()); break;
                case 2: StartCoroutine(BatchType2()); break;
                case 3: StartCoroutine(BatchType3()); break;
                case 4: StartCoroutine(BatchType4()); break;
                default: StartCoroutine(BatchETC()); break;
            }
        }
        else
        {
            BatchFail();
        }
    }
    protected virtual IEnumerator BatchType1() { yield return null; }
    protected virtual IEnumerator BatchType2() { yield return null; }
    protected virtual IEnumerator BatchType3() { yield return null; }
    protected virtual IEnumerator BatchType4() { yield return null; }
    protected virtual IEnumerator BatchETC() { yield return null; }
    protected virtual void BatchFail() { }

}
