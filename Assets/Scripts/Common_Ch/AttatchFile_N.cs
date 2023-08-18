using System;
using UnityEngine;

public class AttatchFile_N : MonoBehaviour
{
    [SerializeField] public AttatchFile_F AF;
    [NonSerialized] public bool IsAttatched = false;
    [NonSerialized] public bool IsDragged = false;
    [NonSerialized] public GameObject CurDragged;

    private void OnDisable()
    {
        IsAttatched = false;
        IsDragged = false;
        CurDragged = null;
    }
}
