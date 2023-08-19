using System;
using UnityEngine;

public class AttatchFile_N : MonoBehaviour
{
    [SerializeField] public AttatchFile_F AF;
    [SerializeField] public DumpedIconManager DI;
    [NonSerialized] public bool IsAttatched = false;
    [NonSerialized] public bool IsDragged = false;
    [NonSerialized] public GameObject CurDragged;
    [NonSerialized] public bool IsDumped = false;

    private void OnDisable()
    {
        IsAttatched = false;
        IsDragged = false;
        IsDumped = false;
        CurDragged = null;
    }
}
