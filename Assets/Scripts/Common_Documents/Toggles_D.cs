using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.UI;

public class Toggles_D : MonoBehaviour
{
    public int Ind;
    Toggle TG;
    public delegate void Interact(int Ind, bool IsOn);
    public Interact interact = null;

    private void Awake()
    {
        TG = GetComponent<Toggle>();
        TG.onValueChanged.AddListener((x) => interact.Invoke(Ind,x));
        if (TG.isOn) TG.enabled = false;
    }
    public void AddAction(Action<int,bool> Act)
    {
        interact = new Interact(Act);
    }
}
