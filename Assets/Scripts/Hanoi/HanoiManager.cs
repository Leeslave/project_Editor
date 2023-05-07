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
    // 현재 박스를 집은 상태인지
    public bool IsPick = false;
    // 현재 집은 박스
    public GameObject PickedBox = null;
    // 현재 사용하고 있는 Container의 Index값. -1이면 아무것도 아닌 상태
    public int CurCon = -1;
    // 현재 Try 횟수를 나타내는 Text
    public TMP_Text Try;
    // 현재 Try 횟수
    public int TryCount = 0;
    // 뒤로가기 기능에 사용 될 Stack. (변경 전의 Container의 Index, 변경 후의 Container의 Index)로 구성된다.
    public Stack<Tuple<int, int>> Back = new Stack<Tuple<int, int>>();
    // 다시 실행 기능에 사용 될 Stack. (변경 전의 Container의 Index, 변경 후의 Container의 Index)로 구성된다.
    public Stack<Tuple<int, int>> Go = new Stack<Tuple<int, int>>();
    // Container들의 내장 함수인 Container를 담는 List
    public List<Container> Containers = new List<Container>();
    // 다시 실행, 되돌리기 버튼
    public GoBack GoB;
    public GoBack BackB;

    private void Awake()
    {
        _Init();
    }

    // 상자를 집은 상태일 경우, 집은 상자를 마우스의 위치로 이동시킴
    private void Update()
    {
        if (IsPick)
        {
            Vector3 MousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition); MousePos.z = 1;
            PickedBox.transform.position = MousePos;
        }
    }

    // Manager의 상태 갱신(초기화)
    // 현재 상자, 현재 Container 초기화 및 Try횟수 갱신.
    public void _Init()
    {
        PickedBox = null;
        CurCon = -1;
        IsPick = false;
        Try.text = $"{TryCount} Try";
    }

    // Container간의 상자가 이동하는 상황에서 호출되는 함수.
    // 현재 상자의 무게가 이동 후 Container 최상단의 무게보다 가볍다면, 해당 Container 위에 쌓고 Try 횟수를 늘리며, 해당 변화를 Back 스택에 추가하고
    // 더 무겁거나, 동일한 Container간의 이동이라면 상자의 위치를 바꾸지 않는다.
    // Input : 이동 전 Container의 Index
    public void AddEvent(int a)
    {
        // 동일 Container간의 이동이라면 다시 상자를 원래 콘테이너에 넣는다.
        if (a == CurCon) Containers[a].AddTop(PickedBox);
        else
        {
            // 현재 상자의 무게가 선택된 Container 최상단의 박스보다 가볍다면, 해당 Container에 넣는다.
            if (Containers[a].CompareTop(PickedBox))
            {
                Containers[a].AddTop(PickedBox);
                // 현재 Back List의 크기가 0이라면 되돌리기 버튼을 활성화 시킨다(SetActive 아님)
                if (Back.Count == 0)
                {
                    BackB.IsActive = true;
                    BackB.OutPoint(null);
                }
                // 해당 변경 사항을 Back 스택에 넣고, Try 횟수를 갱신한다.
                Back.Push(new Tuple<int, int>(CurCon, a));
                PickedBox.GetComponent<Box>().DuraChange(-1);
                Try.text = $"{++TryCount} Try";
            }
            // 현재 상자의 무게가 선택된 Container 최상단의 박스보다 무겁다면, 원래 Container로 되돌린다.
            else Containers[CurCon].AddTop(PickedBox);
        }
    }

    // 되돌리기 버튼을 클릭했을 때 호출되는 함수
    // Back 스택의 크기가 0이 아니라면, 마지막으로 했던 명령을 되돌린다.
    public void BackEvent()
    {
        if(Back.Count > 0)
        {
            Try.text = $"{--TryCount} Try";
            var cnt = Back.Pop();
            Containers[cnt.Item1].AddTop(Containers[cnt.Item2].ReturnTop(true));

            // 되돌린 명령을 Go 스택에 넣으며, Go 스택의 크기가 0이었다면 다시 실행 버튼을 활성화한다(SetActive아님)
            if(Go.Count == 0)
            {
                GoB.IsActive = true;
                GoB.OutPoint(null);
            }
            Go.Push(new Tuple<int, int>(cnt.Item2,cnt.Item1));
            // 명령을 되돌린 후 Back 스택의 크기가 0이라면 되돌리기 버튼을 비활성화 한다.
            if(Back.Count == 0)
            {
                BackB.IsActive = false;
                BackB.gameObject.GetComponent<Image>().color = new Color(0.4f,0.4f,0.4f,1);
            }
        }
    }
    // 다시 실행 버튼을 클릭했을 떄 호출되는 함수
    // Go 스택의 크기가 0이 아니라면, 마지막으로 되돌렸던 명령을 다시 실행한다.
    public void GoEvent()
    {
        if (Go.Count > 0)
        {
            Try.text = $"{++TryCount} Try";
            var cnt = Go.Pop();
            Containers[cnt.Item1].AddTop(Containers[cnt.Item2].ReturnTop(false));

            // 다시 실행한 명령을 Back 스택에 넣으며, Back 스택의 크기가 0이었다면 되돌리기 버튼을 활성화한다.
            if (Back.Count == 0)
            {
                BackB.IsActive = true;
                BackB.OutPoint(null);
            }
            Back.Push(new Tuple<int, int>(cnt.Item2, cnt.Item1));
            // 명령을 다시 실행한 후 Go 스택의 크기가 0이라면 다시 실행 버튼을 비활성화 한다.
            if (Go.Count == 0)
            {
                GoB.IsActive = false;
                GoB.gameObject.GetComponent<Image>().color = new Color(0.4f, 0.4f, 0.4f, 1);
            }
        }
    }

    public void ErrorEvent()
    {

    }

    public void ClearEvent()
    {

    }
}
