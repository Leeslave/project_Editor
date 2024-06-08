using System;
using System.Collections.Generic;
using UnityEngine;

public class AttatchFile_N : MonoBehaviour
{
    // �巡�� ������
    [NonSerialized] public bool IsDragged = false;
    // ���� �巡�� ���� Icon
    [NonSerialized] public GameObject CurDragged;

    [NonSerialized] public int AttatchType = -1;

    public delegate void ExecuteAttatch();
    public ExecuteAttatch Attatch = null;

    [NonSerialized] public string IconName = null;

    private void OnDisable()
    {
        AttatchType = -1;
        Attatch = null;
        IsDragged = false;
        CurDragged = null;
        IconName = null;
    }
}
