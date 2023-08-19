using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DumpedIconManager : MonoBehaviour
{
    [SerializeField] GameObject Sample;
    [SerializeField] public GameObject Opt;
    [SerializeField] Windows_M WM;
    [SerializeField] GameObject CntIcon;
    AttatchFile_N AN;
    List<GameObject> Dumped = new List<GameObject>(30);
    List<DumpedIcon> DumpedIcon = new List<DumpedIcon>(30);
    [SerializeField] RectTransform MyRect;


    public int CurClicked = -1;
    int Occupied = 0;

    private void Awake()
    {
        AN = CntIcon.GetComponent<AttatchFile_N>();
        for (int i = 0; i < 30; i++) Dumped.Add(Instantiate(Sample, MyRect));
        for (int i = 0; i < 30; i++) DumpedIcon.Add(Dumped[i].GetComponent<DumpedIcon>());
        MyUi.AddEvent(GetComponent<EventTrigger>(), EventTriggerType.PointerEnter, OnPoint);
        MyUi.AddEvent(GetComponent<EventTrigger>(), EventTriggerType.PointerExit, OutPoint);
    }

    void OnPoint(PointerEventData Data)
    {
        if (CntIcon.activeSelf) AN.IsDumped = true;
    }
    void OutPoint(PointerEventData Data)
    {
        if (CntIcon.activeSelf) AN.IsDumped = false;
    }

    public void Restore()
    {
        Dumped[CurClicked].SetActive(false);
        DumpedIcon[CurClicked].DumpedCancle();
    }

    public void Remove()
    {
        Dumped[CurClicked].SetActive(false);
        DumpedIcon[CurClicked].ClearDump();
    }
    public void DumpAdd(GameObject Before, Sprite Image, string Name)
    {
        Dumped[Occupied].SetActive(true);
        DumpedIcon[Occupied].Dumped(Before, Image, Name, Occupied++);
        LayoutRebuilder.ForceRebuildLayoutImmediate(MyRect);
    }
}
