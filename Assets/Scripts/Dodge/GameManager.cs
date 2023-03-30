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

    private void Awake()
    {
        PTL = new List<Pattern>() { new Pattern() };
        ReadPattern();
    }
    private void Start()
    {
        StartCoroutine(MakeNormalPattern());
    }

    IEnumerator MakeNormalPattern()
    {
        Pattern CurPattern = PTL[Random.Range(0, PatternNum)];
        for(int CurRepeat = 0; CurRepeat < CurPattern.repeat; CurRepeat++)
        {
            for(int x = 0; x < CurPattern.PT[0].Length; x++)
            {
                for(int y = 0; y < CurPattern.PT.Count; y++)
                {
                    if (CurPattern.PT[y][x] == 1)
                    {
                        if (CurRepeat % 2 == 0) BM.MakeSmallBul(Vector2.left * 10, Vector2.zero).transform.position = SPR[y].position;
                        else BM.MakeSmallBul(Vector2.right * 10, Vector2.zero).transform.position = SPL[y].position;
                    }
                }
                yield return new WaitForSeconds(BulletInterv);
            }
            yield return new WaitForSeconds(RepeatInterv);
        }
        yield break;
    }


    
    void ReadPattern()  // Read Pattern In Resources Folder. Name of The Pattern File Must be Pattern_X ( ex) Pattern_1, Pattern_2)
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
}
