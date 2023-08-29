using System;
using UnityEngine;

public class AttatchFile_N : MonoBehaviour
{
    [SerializeField] public AttatchFile_F AF;
    [SerializeField] public DumpedIconManager DI;
    // �޽��� ���� ÷�� �ʵ忡 �巡�� �Ǿ� �ִ��� ����
    [NonSerialized] public bool IsAttatched = false;
    // �巡�� ������
    [NonSerialized] public bool IsDragged = false;
    // ���� �巡�� ���� Icon
    [NonSerialized] public GameObject CurDragged;
    // �������뿡 �巡�� �Ǿ� �ִ���
    [NonSerialized] public bool IsDumped = false;

    private void OnDisable()
    {
        IsAttatched = false;
        IsDragged = false;
        IsDumped = false;
        CurDragged = null;
    }
}
