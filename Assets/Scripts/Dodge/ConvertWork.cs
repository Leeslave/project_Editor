using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConvertWork : MonoBehaviour
{
    public static ConvertWork Instance { get; private set; }
    
    private bool preStepClear;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start() { preStepClear = false; }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    public bool PreStepClearCheck() { return preStepClear; }

    public void PreStepClear()
    {
        if (!preStepClear)
        {
            preStepClear = true;
            return;
        }
    }

    public void DeleteGameObject() { Destroy(gameObject); }

}
