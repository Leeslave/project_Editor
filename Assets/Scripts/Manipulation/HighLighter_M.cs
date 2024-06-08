using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HighLighter_M : MonoBehaviour
{
    public InfChange In;
    public Image Clicker;
    public int IsFolder;    // File : 0, Folder : 1

    private bool Selected = false;
    private Color ColorOn;
    private Color ColorHigh;
    private Color ColorOff;

    [HideInInspector]
    public List<string> Files;

    [SerializeField] int FolderType;

    private void Awake()
    {
        if (IsFolder == 1 && FolderType != -1) Files = DB_M.DB_Docs.InfSub[FolderType].ToList();
        ColorOff = Clicker.color;
        ColorHigh = Clicker.color + new Color(0, 0, 0, 0.5f);
        ColorOn = Clicker.color + new Color(0, 0, 0, 1);
        MyUi.ButtonInit(GetComponent<EventTrigger>(),OnPointer,OutPointer,Touch);
    }

    private void OnPointer(PointerEventData Event)
    {
        if(!Selected)Clicker.color = ColorHigh;
    }

    private void OutPointer(PointerEventData Event)
    {
        if(!Selected) Clicker.color = ColorOff;
    }

    private void Touch(PointerEventData Event)
    {
        if (!In.IsTouchAble()) return;
        Clicker.color = ColorOn;
        Selected = true;
        In.TouchManager(gameObject,IsFolder);
    }

    public void HighLightOff()
    {
        Selected = false;
        OutPointer(null);
    }
}
