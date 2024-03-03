using System;
using System.Linq;
using TMPro;
using UnityEngine;

public class MouseManager_P : MonoBehaviour
{
    [SerializeField] MapManager_P MP;
    [SerializeField] Texture2D DrawingCursor;
    Polices_P SelectedPolice = null;
    WaitForSeconds WFS = new WaitForSeconds(0.1f);


    [SerializeField] TMP_Text HPTest;

    bool IsDrawingMode = false;
    enum cursor
    {
        Default,
        Drawing
    }

    cursor CursorType;
    Vector2 CursorGap;
    Vector3 anchorGap = new Vector3(400, 300);
    bool IsClick = false;
    bool IsIndexChanged = false;

    int CurIndX = 0;
    int CurIndY = 0;

    int LastX = 0;
    int LastY = 0;

    // Update is called once per frame
    private void Start()
    {
        CursorGap = new Vector2(0 ,DrawingCursor.height);
        CursorType = 0;
    }
    int i = 0;
    private void Update()
    {
        if (Input.GetMouseButton(0)) IsClick = true;
        if (Input.GetMouseButtonDown(1) && IsDrawingMode) 
        { IsDrawingMode = false; CursorChange(0); SelectedPolice.DeSelected(); SelectedPolice = null; }
        Vector3 CntPos = MyUi.GetMousePos() + anchorGap;
        int SubX = (int)(CntPos.x / MP.UpScaling), SubY = (int)(CntPos.y / MP.UpScaling);
        int Res = MP.TileType(SubX, SubY);
        if (CurIndX !=  SubX|| CurIndY != SubY)
        {            
            CurIndX = SubX; CurIndY = SubY;
            IsIndexChanged = true;
        }

        if (IsDrawingMode&&IsIndexChanged)
        {
            if (Res != 0 && SelectedPolice.DrawAble(Res))
            {
                if (Math.Abs(LastX - CurIndX) + Math.Abs(LastY - CurIndY) > 1)
                {
                    CursorChange(0);
                }
                else
                {
                    if (IsIndexChanged)
                    {
                        LastX = CurIndX;
                        LastY = CurIndY;
                        SelectedPolice.AddLine(MP.TilePos(CurIndX, CurIndY));
                        HPTest.text = $"HP : {SelectedPolice.Info.HP}";
                    }
                    CursorChange(1);
                }
            }
            else
            {
                CursorChange(0);
            }
        }
        else if (Res == 7 && IsClick && !IsDrawingMode)
        {
            IsDrawingMode = true;
            SelectedPolice = MP.GetPolice(CurIndX, CurIndY);
            LastX = CurIndX;
            LastY = CurIndY;
            SelectedPolice.Selected();
            if (SelectedPolice.Route.Count > 2) { var tmp = MP.PosToInd(SelectedPolice.Route.Last() + anchorGap); LastX = tmp.Item1; LastY = tmp.Item2; }
            else SelectedPolice.AddLine(MP.TilePos(CurIndX, CurIndY));
            HPTest.text = $"HP : {SelectedPolice.Info.HP}";
        }
        IsClick = false;
        IsIndexChanged = false;
    }

    public void CursorChange(int Type)
    {
        switch ((cursor)Type)
        {
            case cursor.Default:
                if (CursorType != cursor.Default) { CursorType = cursor.Default; Cursor.SetCursor(null, CursorGap, CursorMode.Auto); }
                break;
            case cursor.Drawing:
                if (CursorType != cursor.Drawing) { CursorType = cursor.Drawing; Cursor.SetCursor(DrawingCursor, CursorGap, CursorMode.Auto); }
                break;
        }
    }
}
