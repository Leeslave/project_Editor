using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;
using System.IO;
using System;
using TMPro;
using Random = UnityEngine.Random;
using System.Threading;
using System.Linq;
using System.Globalization;
using System.Net;
using Unity.VisualScripting;

// Main Manager
public class GameManager: MonoBehaviour
{
    [SerializeField]
    public Timer Timer;             // Timer Prefab
    public BulletManager BM;        // Bullet Create Manager Prefab
    public PatternManager PM;       // For UpgradePattern Script
    public GlitchEffect MC;         // Making Glitch Prefab
    public Player Pl;               // Player Prefab

    //Prefabs
    public GameObject GameOver;     // GameOver Prefab
    public GameObject GameEnd;      // GameEnd Prefab

    public GameObject PlatU;        // PlatForm_Up
    public GameObject PlatD;        // PlatForm_Down

    //Public variable
    public bool GameType = false;   // Now Playing Type(Normal/Hard : false/true)
    [Range(5, 30)]
    public float TimeToSurvive;     // How Long Player Have To Survie

    //Private variable
    IEnumerator CurPlayPattern;     // Now Playing Patter(Normal/Hard)

    private void Start()
    {
        Timer.MaxTime = TimeToSurvive;
    }

    void TimeFlow()         // Timer Work, Pattern Work
    {
        Timer.IsTimeFlow = true;
        StartCoroutine(CurPlayPattern);
    }

    public void GameResult()
    {
        BM.DelBul();
        Timer.IsTimeFlow = false;
        if (!GameType)      // When Normal
        {
            if (Timer.time >= TimeToSurvive)        // Endure Time
            {
                GameEnd.SetActive(true);
            }
            else            // GameOver
            {
                GameOver.SetActive(true);
                StopAllCoroutines();
                Timer.time = 0;
            }
        }
        else              // When Hard
        {
            /*UpValue.EndPT();*/
            GameEnd.SetActive(true);
            if (Timer.time >= TimeToSurvive * 2)
            {
                GameEnd.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = "Hidden";
            }
            else           // GameOver
            {
                Timer.time = 0;
                Timer.MaxTime = TimeToSurvive;
            }
        }
    }

    public void MakeGlitch(float a, float b, float c)
    {
        MC.intensity = a; MC.flipIntensity = b; MC.colorIntensity = c;
    }



}
