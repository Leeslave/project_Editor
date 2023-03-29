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
    public GameObject PlatU;
    public GameObject PlatL;

    //Public variable
    public float RepeatInterv;      // Interval within the Pattern
    public float BulletInterv;      // Interval between Bullets
    public float PatternInterv;     // Interval between Patterns (When Pattern End)
    public int PatternNum;          // Num of Pattern
    public int RepeatNum;           // Num of Repeatition of Pattern
    public int TimeToSurvive;        // How Long Player Have To Survie
    //Private variable
    private float time = 0;         // Current Time (For Timer)
    private int CurIndex;           // Current Index (For Pattern List)
    private int CurRepeat;          // Current Repeatition
    private int CurPattern;         // Current Pattern
    private bool TimeOn = true;

    private void Awake()
    {
        PTL = new List<Pattern>() { new Pattern() };
        ReadPattern();
    }
    private void Start()
    {
        /*StartCoroutine(GetComponent<UpgradePattern>().Pattern2());*/
        StartCoroutine(GetComponent<UpgradePattern>().Pattern1());
    }

    private void OnEnable()
    {
        /*Init();*/
    }

    IEnumerator MakePattern()
    {
        yield return new WaitForSeconds(PatternInterv / 2);

        for (CurRepeat = 0; CurRepeat < RepeatNum; CurRepeat++)
        {
            Transform[] cnt;
            Vector2 Dir;
            if (CurRepeat % 2 == 0) { cnt = SPR; Dir = Vector2.left; }
            else { cnt = SPL; Dir = Vector2.right; }

            for (CurIndex = 0; CurIndex < PTL[CurPattern].PT[0].Length; CurIndex++)
            {
                for (int i = 0; i < PTL[CurPattern].PT.Count; i++)
                {
                    if (PTL[CurPattern].PT[i][CurIndex] == 1)
                    {
                        GameObject tmp = BM.MakeSmallBul(Dir * 10, Vector2.zero);
                        tmp.transform.position = cnt[i].position;
                    }
                }
                yield return new WaitForSeconds(BulletInterv);
            }
            yield return new WaitForSeconds(RepeatInterv);
        }

        yield return new WaitForSeconds(PatternInterv / 2);
        Init();
    }

    IEnumerator TImeUpdate()
    {
        for (time = 0; time <= TimeToSurvive; time += 0.01f)
        {
            yield return new WaitForSeconds(0.01f);
            Timer.text = string.Format("{0:0.00}", time);
        }
        Timer.text = string.Format("{0:0.00}", time);
        StopAllCoroutines();
        BM.DelBul();
        EndPattern();
    }

    void EndPattern()
    {
        for (int i = 0; i < SPR.Length; i++)
        {
            GameObject Left = BM.MakeSmallBul(Vector2.right * 10, Vector2.zero); Left.transform.position = SPL[i].position;
            GameObject Right = BM.MakeSmallBul(Vector2.left * 10, Vector2.zero); Right.transform.position = SPR[i].position;
        }
    }

    void Init()
    {
        CurPattern = Random.Range(1, PatternNum + 1);
        CurIndex = 0;
        CurRepeat = 0;
        if (TimeOn) { time = 0; StartCoroutine(TImeUpdate()); TimeOn = false; }
        StartCoroutine(MakePattern());
    }

    // Read Pattern In Resources Folder. Name of The Pattern File Must be Pattern_X ( ex) Pattern_1, Pattern_2)
    void ReadPattern()
    {
        for (int i = 1; i < PatternNum+1; i++)
        {
            string tmp = "Text/Dodge/Pattern_" + i.ToString();
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

    void GameOverFunc()
    {
        StopAllCoroutines();
        GameEnd.SetActive(true);
        Pl.End_Player();
        BM.EndBul();
    }
}
