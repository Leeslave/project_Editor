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
    //Public variable
    public float MPPM;              // Interval within the Pattern
    public float MBPM;              // Interval between Bullets
    public float PTIV;              // Interval between Patterns (When Pattern End)
    public int PatternNum;          // Num of Pattern
    public int RepeatNum;           // Num of Repeatition of Pattern
    public int TimeToSurvie;        // How Long Player Have To Survie
    //Private variable
    private float CPPM;             // Current Interval constant within the Pattern
    private float time = 0;         // Current Time (For Timer)
    private int CurIndex;           // Current Index (For Pattern List)
    private int CurRepeat;          // Current Repeatition
    private int CurPattern;         // Current Pattern
    private bool PTIVJ = true;      // Don't Call MakeBullet Function betweens interval
    private bool IsPTEnd = true;    // Judgment whether Pattern is End

    private void Awake()
    {
        PTL = new List<Pattern>();
        PTL.Add(new Pattern());
        ReadPattern();
        Init();
    }


    // Read Pattern In Resources Folder. Name of The Pattern File Must be PatternX ( ex) Pattern1, Pattern2)
    void ReadPattern()
    {
        for (int i = 1; i < PatternNum+1; i++)
        {
            string tmp = "Pattern" + i.ToString();
            TextAsset textFile = Resources.Load(tmp) as TextAsset;
            if (textFile == null)
            {
                return;
            }
            StringReader stringReader = new StringReader(textFile.text);
            Pattern Data = new Pattern();
            Data.PT = new List<int[]>();
            Data.repeat = RepeatNum;

            while (stringReader != null)
            {
                string line = stringReader.ReadLine();
                if (line == null) break;
                int[] cnt = Array.ConvertAll(line.Split(' '), int.Parse);
                Data.PT.Add(cnt);
            }
            PTL.Add(Data);
            stringReader.Close();
        }
    }
    void Update()
    {
        if (!Pl.IsGameOver)
        {
            Timer.text = string.Format("{0:N2}",time);
            time += Time.deltaTime;
            if(time >= TimeToSurvie)
            {
                gameObject.SetActive(false);
            }
            if (CPPM >= MPPM)
            {
                IsPTEnd = false;
                CurIndex = 0;
                CurRepeat = 0;
                InvokeRepeating("MakeBullet", 1, MBPM);
                CPPM = 0;
            }
            if (IsPTEnd)
            {
                if (CPPM == 0) CurPattern = Random.Range(1,PatternNum+1);
                CPPM += Time.deltaTime;
            }
        }
        else
        {
            if (!GameOver.activeSelf)
            {
                GameOver.SetActive(true);
                CancelInvoke("MakeBullet");
                BM.DelBul();
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                Init();
            }
        }
    }
    private void OnEnable()
    {
        Init();
    }
    private void OnDisable()
    {
        GameEnd.SetActive(true);
        Pl.End_Player();
        CancelInvoke("MakeBullet");
        BM.EndBul();
    }

    void Pause()
    {

    }
    void Init()
    {
        time = 0;
        CPPM = 5;
        CurIndex = 0;
        CurRepeat = 0;
        IsPTEnd = true;
        PTIVJ = true;
        CurPattern = Random.Range(1, PatternNum + 1);
        GameOver.SetActive(false);
        Pl.IsGameOver = false;
        Pl.make_Player();
    }
    void MakeBullet()
    {
        if (PTIVJ)
        {
            if (CurIndex < PTL[CurPattern].PT[0].Length)
            {
                for (int y = 0; y < PTL[CurPattern].PT.Count; y++)
                {
                    if (CurRepeat % 2 == 0)
                    {
                        if (PTL[CurPattern].PT[y][CurIndex] == 1)
                        {
                            GameObject bullet = BM.MakeBul("L");
                            bullet.transform.position = SPL[y].position;
                        }
                    }
                    else
                    {
                        if (PTL[CurPattern].PT[PTL[CurPattern].PT.Count - y - 1][CurIndex] == 1)
                        {
                            GameObject bullet = BM.MakeBul("R");
                            bullet.transform.position = SPR[y].position;
                        }
                    }
                }
                CurIndex += 1;
            }
            if (CurIndex >= PTL[CurPattern].PT[0].Length)
            {
                CurIndex = 0;
                CurRepeat++;
                PTIVJ = false;
                Invoke("MakeBulletSub", PTIV);
            }
            if (CurRepeat == PTL[CurPattern].repeat)
            {
                CancelInvoke("MakeBullet");
                IsPTEnd = true;
            }
        }
    }
    void MakeBulletSub()
    {
        PTIVJ = true;
    }
}
