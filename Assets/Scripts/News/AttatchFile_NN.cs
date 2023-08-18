using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttatchFile_NN : MonoBehaviour
{
    [SerializeField] public AttatchFile_FN AF;
    [NonSerialized] public bool IsAttatched = false;
    [NonSerialized] public bool IsDragged = false;
}
