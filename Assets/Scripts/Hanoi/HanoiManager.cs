using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HanoiManager : MonoBehaviour 
{
    public bool IsPick = false;
    public GameObject PickedBox = null;
    public int CurCon = -1;
    public TMP_Text Try;
    public int TryCount = 0;
    public Stack<Tuple<int, int>> Back = new Stack<Tuple<int, int>>();
    public Stack<Tuple<int, int>> Go = new Stack<Tuple<int, int>>();
    public List<Container> Containers = new List<Container>();
    public GoBack GoB;
    public GoBack BackB;

    private void Awake()
    {
        _Init();
    }

    private void Update()
    {
        if (IsPick)
        {
            Vector3 MousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition); MousePos.z = 1;
            PickedBox.transform.position = MousePos;
        }
    }

    public void _Init()
    {
        PickedBox = null;
        CurCon = -1;
        IsPick = false;
        Try.text = $"{TryCount} Try";
    }
    public void AddEvent(int a)
    {
        if (a == CurCon) Containers[a].AddTop(PickedBox);
        else
        {
            if (Containers[a].CompareTop(PickedBox))
            {
                Containers[a].AddTop(PickedBox);
                if (Back.Count == 0)
                {
                    BackB.IsActive = true;
                    BackB.OutPoint(null);
                }
                Back.Push(new Tuple<int, int>(CurCon, a));
                Try.text = $"{++TryCount} Try";
            }
            else Containers[CurCon].AddTop(PickedBox);
        }
    }
    public void BackEvent()
    {
        if(Back.Count > 0)
        {
            Try.text = $"{--TryCount} Try";
            var cnt = Back.Pop();
            Containers[cnt.Item1].AddTop(Containers[cnt.Item2].ReturnTop());
            if(Go.Count == 0)
            {
                GoB.IsActive = true;
                GoB.OutPoint(null);
            }
            Go.Push(new Tuple<int, int>(cnt.Item2,cnt.Item1));
            if(Back.Count == 0)
            {
                BackB.IsActive = false;
                BackB.gameObject.GetComponent<Image>().color = new Color(0.4f,0.4f,0.4f,1);
            }
        }
    }
    public void GoEvent()
    {
        if (Go.Count > 0)
        {
            Try.text = $"{++TryCount} Try";
            var cnt = Go.Pop();
            Containers[cnt.Item1].AddTop(Containers[cnt.Item2].ReturnTop());
            if (Back.Count == 0)
            {
                BackB.IsActive = true;
                BackB.OutPoint(null);
            }
            Back.Push(new Tuple<int, int>(cnt.Item2, cnt.Item1));
            if (Go.Count == 0)
            {
                GoB.IsActive = false;
                GoB.gameObject.GetComponent<Image>().color = new Color(0.4f, 0.4f, 0.4f, 1);
            }
        }
    }
}
