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
    public Player Pl;               // Player Prefab

    //Prefabs
    public GameObject GameOverP;     // GameOver Prefab
    public GameObject GameEnd;      // GameEnd Prefab

    public GameObject PlatU;        // PlatForm_Up
    public GameObject PlatD;        // PlatForm_Down

    //Public variable
    public bool GameType = false;   // Now Playing Type(Normal/Hard : false/true)
    [Range(5, 30)]
    public float TimeToSurvive;     // How Long Player Have To Survie

    //Private variable
    IEnumerator CurPlayPattern;     // Now Playing Patter(Normal/Hard)

    void TimeFlow(bool tp)         // Timer Work, Pattern Work
    {
        Timer.IsTimeFlow = tp;
    }
}
