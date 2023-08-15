using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttatchFile_N : MonoBehaviour
{
    [SerializeField] public AttatchFile_F AF;
    [NonSerialized] public bool IsAttatched = false;
    [NonSerialized] public bool IsDragged = false;
}
