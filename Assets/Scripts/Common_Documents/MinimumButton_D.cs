using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class MinimumButton_D : Buttons_M
{
    [SerializeField] float MinY;
    [SerializeField] float MaxY;
    [SerializeField] Transform Moved;
    [SerializeField] TMP_Text Text;
    [SerializeField] bool MinimumType;
    float Gap;
    bool IsOn = true;

    protected override void Awake()
    {
        base.Awake();
        Gap = (MaxY - MinY) * 0.05f;
    }

    protected override void OnPointer(PointerEventData data)
    {
        
    }
    protected override void OutPointer(PointerEventData data)
    {
        
    }
    protected override void Click(PointerEventData Data)
    {
        if (!MinimumType)
        {
            StartCoroutine(Mover());
        }
    }

    public void ClickCall()
    {
        Click(null);
    }

    WaitForSeconds wfs = new WaitForSeconds(0.01f);
    IEnumerator Mover()
    {
        for(int i = 0; i < 20; i++)
        {
            if (IsOn) Moved.Translate(0,-Gap, 0);
            else Moved.Translate(0,Gap,0);
            yield return wfs;
        }
        if (IsOn) Text.text = "+";
        else Text.text = "-";
        IsOn = IsOn == false;
    }
}
