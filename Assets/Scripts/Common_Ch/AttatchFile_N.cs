using System;
using UnityEngine;

public class AttatchFile_N : MonoBehaviour
{
    [SerializeField] public AttatchFile_F AF;
    [SerializeField] public DumpedIconManager DI;
    // 메신저 상의 첨부 필드에 드래그 되어 있는지 여부
    [NonSerialized] public bool IsAttatched = false;
    // 드래그 중인지
    [NonSerialized] public bool IsDragged = false;
    // 현재 드래그 중인 Icon
    [NonSerialized] public GameObject CurDragged;
    // 쓰레기통에 드래그 되어 있는지
    [NonSerialized] public bool IsDumped = false;

    private void OnDisable()
    {
        IsAttatched = false;
        IsDragged = false;
        IsDumped = false;
        CurDragged = null;
    }
}
