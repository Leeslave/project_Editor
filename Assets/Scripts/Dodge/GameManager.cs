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

public class GameManager: MonoBehaviour
{
    [SerializeField]
    //Prefabs
    public BulletManager BM;        // Bullet Create Manager Prefab
    public Player Pl;               // Player Prefab
    public GameObject GameOver;     // GameOver Prefab
    public GameObject GameEnd;      // GameEnd Prefab
    public Transform[] SPR;         // Bullet Spawn Position(Transform) List_Right
    public Transform[] SPL;         // Bullet Spawn Position(Transform) List_Left
    public List<Pattern> PTL;       // List of Patterns
    public TMP_Text Timer;          // Timer Prefab
    public GameObject PlatU;        // PlatForm_Up
    public GameObject PlatD;        // PlatForm_Down

    //Public variable
    public float RepeatInterv;      // Interval within the Pattern
    public float BulletInterv;      // Interval between Bullets
    public float PatternInterv;     // Interval between Patterns (When Pattern End)
    public int PatternNum;          // Num of Pattern
    public int RepeatNum;           // Num of Repeatition of Pattern
    public float TimeToSurvive;     // How Long Player Have To Survie
    //Private variable
    private IEnumerator TimerCor;   // For Timmer Coroutine
    private bool TimerWork = false; // For Check TImerCor Is On Work;
    private IEnumerator PatternCor; // For Pattern Update Coroutine
    private bool PatternOn = false; // For Check PatternCor Is On Work;
    private IEnumerator EndCor;     // For End Coroutine
    private bool EndWork = false;   // For Check EndCor Is On Work;
    private UpgradePattern UP;      // Upgrade Pattern
    private float NormalTime;       // For Rollback Time To Survive
    private float time = 0;         // Current Time (For Timer)
    private int CurIndex;           // Current Index (For Pattern List)
    private int CurRepeat;          // Current Repeatition
    private int CurPattern;         // Current Pattern
    private bool AllEnd = false;    // Whether player lasted for the Time For Survive
    private bool TimeOn = true;     // Is Timer On

    private void Awake()
    {
        PTL = new List<Pattern>() { new Pattern() };
        ReadPattern();
        NormalTime = TimeToSurvive;
    }
    private void Start()
    {
        UP = GetComponent<UpgradePattern>();
    }
}
