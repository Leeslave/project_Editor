using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Polices_P : MonoBehaviour
{
    [NonSerialized] public List<Vector3> Route;
    [NonSerialized] public Police Info;
    [NonSerialized] public Color LineColor;

    LineRenderer Line;
    void Start()
    {
        Route = new List<Vector3>(); Route.Add(new Vector3(-1, -1)); Route.Add(new Vector3(-1, -1));
        Line = GetComponent<LineRenderer>();
        Line.positionCount = 0;
        Line.enabled = false;
        Line.material.color = LineColor;
        Line.startWidth = 5;
        Line.endWidth = 5;
    }

    public void Selected()
    {
        Line.enabled = true;
    }

    public void DeSelected()
    {
        Line.enabled = false;
    }

    public void AddLine(Vector3 NewPos)
    {
        if (Route[Route.Count-2] == NewPos)
        {
            Route.RemoveAt(Route.Count-1);
            Line.positionCount--;
            Info.HP++;
        }
        else if (Route[Route.Count-1] != NewPos)
        {
            Route.Add(NewPos);
            Line.positionCount++;
            Line.SetPosition(Line.positionCount - 1, NewPos - new Vector3(0, 0, 1));
            Info.HP--;
        }
    }

    public bool DrawAble(int Type)
    {
        if (Info.IsCar)
        {
            if (Type == 1) return false;
            else if (Info.HP > 0) { return true; }
            else { Debug.Log("HP End!"); return false; }
        }
        else
        {
            if (Type != 1 && Type != 5) return false;
            else if (Info.HP > 0) { return true; }
            else { Debug.Log("HP End!"); return false;}
        }
    }
}
