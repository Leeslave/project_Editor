using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HanoiManager : MonoBehaviour 
{
    // ���� �ڽ��� ���� ��������
    public bool IsPick = false;
    // ���� ���� �ڽ�
    public GameObject PickedBox = null;
    // ���� ����ϰ� �ִ� Container�� Index��. -1�̸� �ƹ��͵� �ƴ� ����
    public int CurCon = -1;
    // ���� Try Ƚ���� ��Ÿ���� Text
    public TMP_Text Try;
    // ���� Try Ƚ��
    public int TryCount = 0;
    // �ڷΰ��� ��ɿ� ��� �� Stack. (���� ���� Container�� Index, ���� ���� Container�� Index)�� �����ȴ�.
    public Stack<Tuple<int, int>> Back = new Stack<Tuple<int, int>>();
    // �ٽ� ���� ��ɿ� ��� �� Stack. (���� ���� Container�� Index, ���� ���� Container�� Index)�� �����ȴ�.
    public Stack<Tuple<int, int>> Go = new Stack<Tuple<int, int>>();
    // Container���� ���� �Լ��� Container�� ��� List
    public List<Container> Containers = new List<Container>();
    // �ٽ� ����, �ǵ����� ��ư
    public GoBack GoB;
    public GoBack BackB;
    // Error
    public GameObject Error;
    public TMP_Text ErrorMessage;
    // Judge
    public bool TouchAble = true;

    // 3�� �����̳ʿ� ������ �߰� �Ǿ�� �ϴ� Box
    public int NextBox = 5;
    int LastCon = 0;

    private void Awake()
    {
        _Init();
    }

    // ���ڸ� ���� ������ ���, ���� ���ڸ� ���콺�� ��ġ�� �̵���Ŵ
    private void Update()
    {
        if (IsPick)
        {
            Vector3 MousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition); MousePos.z = 1;
            PickedBox.transform.position = MousePos;
        }
    }

    // Manager�� ���� ����(�ʱ�ȭ)
    // ���� ����, ���� Container �ʱ�ȭ �� TryȽ�� ����.
    public void _Init()
    {
        PickedBox = null;
        CurCon = -1;
        IsPick = false;
        Try.text = $"{TryCount} Try";
    }

    // Container���� ���ڰ� �̵��ϴ� ��Ȳ���� ȣ��Ǵ� �Լ�.
    // ���� ������ ���԰� �̵� �� Container �ֻ���� ���Ժ��� �����ٸ�, �ش� Container ���� �װ� Try Ƚ���� �ø���, �ش� ��ȭ�� Back ���ÿ� �߰��ϰ�
    // �� ���̰ų�, ������ Container���� �̵��̶�� ������ ��ġ�� �ٲ��� �ʴ´�.
    // Input : �̵� �� Container�� Index
    public void AddEvent(int a)
    {
        LastCon = a;
        IsPick = false;
        // ���� Container���� �̵��̶�� �ٽ� ���ڸ� ���� �����̳ʿ� �ִ´�.
        if (a == CurCon) Containers[a].AddTop(PickedBox);
        else
        {
            // ���� ������ ���԰� ���õ� Container �ֻ���� �ڽ����� �����ٸ�, �ش� Container�� �ִ´�.
            if (Containers[a].CompareTop(PickedBox))
            {
                Containers[a].AddTop(PickedBox);
                // ���� Back List�� ũ�Ⱑ 0�̶�� �ǵ����� ��ư�� Ȱ��ȭ ��Ų��(SetActive �ƴ�)
                if (Back.Count == 0)
                {
                    BackB.IsActive = true;
                    BackB.OutPoint(null);
                }
                // �ش� ���� ������ Back ���ÿ� �ְ�, Try Ƚ���� �����Ѵ�.
                Back.Push(new Tuple<int, int>(CurCon, a));

                if (Go.Count != 0)
                {
                    Go.Clear();
                    GoB.IsActive = false;
                    GoB.gameObject.GetComponent<Image>().color = new Color(0.4f, 0.4f, 0.4f, 1);
                }
                PickedBox.GetComponent<Box>().DuraChange(-1);
                Try.text = $"{++TryCount} Try";
            }
            // ���� ������ ���԰� ���õ� Container �ֻ���� �ڽ����� ���̴ٸ�, ���� Container�� �ǵ�����.
            else Containers[CurCon].AddTop(PickedBox);
        }
    }

    // �ǵ����� ��ư�� Ŭ������ �� ȣ��Ǵ� �Լ�
    // Back ������ ũ�Ⱑ 0�� �ƴ϶��, ���������� �ߴ� ������ �ǵ�����.
    public void BackEvent()
    {
        if(Back.Count > 0)
        {
            Try.text = $"{--TryCount} Try";
            var cnt = Back.Pop();
            Containers[cnt.Item1].AddTop(Containers[cnt.Item2].ReturnTop(1));

            // �ǵ��� ������ Go ���ÿ� ������, Go ������ ũ�Ⱑ 0�̾��ٸ� �ٽ� ���� ��ư�� Ȱ��ȭ�Ѵ�(SetActive�ƴ�)
            if(Go.Count == 0)
            {
                GoB.IsActive = true;
                GoB.OutPoint(null);
            }
            Go.Push(new Tuple<int, int>(cnt.Item2,cnt.Item1));
            // ������ �ǵ��� �� Back ������ ũ�Ⱑ 0�̶�� �ǵ����� ��ư�� ��Ȱ��ȭ �Ѵ�.
            if(Back.Count == 0)
            {
                BackB.IsActive = false;
                BackB.gameObject.GetComponent<Image>().color = new Color(0.4f,0.4f,0.4f,1);
            }
        }
    }
    // �ٽ� ���� ��ư�� Ŭ������ �� ȣ��Ǵ� �Լ�
    // Go ������ ũ�Ⱑ 0�� �ƴ϶��, ���������� �ǵ��ȴ� ������ �ٽ� �����Ѵ�.
    public void GoEvent()
    {
        if (Go.Count > 0)
        {
            Try.text = $"{++TryCount} Try";
            var cnt = Go.Pop();
            Containers[cnt.Item1].AddTop(Containers[cnt.Item2].ReturnTop(-1));

            // �ٽ� ������ ������ Back ���ÿ� ������, Back ������ ũ�Ⱑ 0�̾��ٸ� �ǵ����� ��ư�� Ȱ��ȭ�Ѵ�.
            if (Back.Count == 0)
            {
                BackB.IsActive = true;
                BackB.OutPoint(null);
            }
            Back.Push(new Tuple<int, int>(cnt.Item2, cnt.Item1));
            // ������ �ٽ� ������ �� Go ������ ũ�Ⱑ 0�̶�� �ٽ� ���� ��ư�� ��Ȱ��ȭ �Ѵ�.
            if (Go.Count == 0)
            {
                GoB.IsActive = false;
                GoB.gameObject.GetComponent<Image>().color = new Color(0.4f, 0.4f, 0.4f, 1);
            }
        }
    }

    // ���� �ڽ��� 3�� Container�� �̵��� ���� �ƴϸ�, ���� �̵� ������ �ƴ� ��� ���� ���
    // ex) 0 (2,3) (4)�̸� 3�� �ڽ��� �������� 1�� ���¿��� 1������ �ű�� ���
    // ���� ���� ��Ȳ���� 2�� �ڽ��� �������� 1�� ���¿��� 3������ �ű�� ���
    public void ErrorEvent(int num)
    {
        if (num == NextBox && LastCon == 2)
        {
            NextBox--;
            if (NextBox == 0) ClearEvent(); 
            return;
        }
        Error.SetActive(true);
        ErrorMessage.text = $"{num}�� Box��\n�������� 0�Դϴ�.";
        TouchAble = false;
    }

    public void ClearEvent()
    {

    }

    public void TouchAbleChange()
    {
        TouchAble = !TouchAble;
    }
}
