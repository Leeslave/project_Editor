using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class MakeTile : MonoBehaviour
{
    [SerializeField]
    public GameObject CWall;
    public GameObject RWall;
    public GameObject Player;
    public GameObject Key;
    public MazeMap Maze_Inf;
    public GameObject Clear;
    public GameObject Fog;
    public List<List<GameObject>> Fogs = new List<List<GameObject>>();
    public List<List<bool>> IsFog = new List<List<bool>>();

    public float Move_X;
    public float Move_Y;
    [System.NonSerialized]
    public int Col;
    public int Row;
    public int KeyNum;
    public int KeyWeight;

    void Awake()
    {
        GetDifficulty();
        Maze_Inf = new MazeMap();
        Maze_Inf.MazeMaking(Col, Row);
        Player.transform.position = new Vector3(Maze_Inf.Player_X * Move_X + 5, Maze_Inf.Player_Y * Move_Y + 5f, 0);
        CreateLevel();
        MakeKey();
    }

    void GetDifficulty()
    {
        if (PlayerPrefs.HasKey("Difficulty"))
        {
            switch (PlayerPrefs.GetString("Difficulty"))
            {
                case "1":
                    Col = 10; Row = 10; KeyNum = 1; KeyWeight = 1;
                    break;
                case "2":
                    Col = 15; Row = 15; KeyNum = 2; KeyWeight = 3;
                    break;
                case "3":
                    Col = 20; Row = 20; KeyNum = 3; KeyWeight = 5;
                    break;
            }
        }
    }


    void CreateLevel()
    {
        // 미로의 바깥 부분을 둘러싸는  안개를 생성.
        GameObject outsT = Instantiate(Fog, new Vector3((-3) * Move_X + 5, (Col + 5) * Move_Y + 10), new Quaternion(0, 0, 0, 0));
        outsT.transform.localScale = new Vector2(2 * Row * Move_X + 120 ,12 * Move_Y);
        GameObject outsL = Instantiate(Fog, new Vector3(-60, -60), new Quaternion(0, 0, 0, 0));
        outsL.transform.localScale = new Vector2(12 * Move_X, 2 * Col * Move_Y + 120);
        GameObject outsB = Instantiate(Fog, new Vector3(-60, -60), new Quaternion(0, 0, 0, 0));
        outsB.transform.localScale = new Vector2(2 * Row * Move_X + 120, 12 * Move_Y);
        GameObject outsR = Instantiate(Fog, new Vector3((Row + 5) * Move_X + 10, (-3) * Move_Y + 5), new Quaternion(0, 0, 0, 0));
        outsR.transform.localScale = new Vector2(12 * Move_X, 2 * Col * Move_Y + 120);

        for(int y = 0; y < Col * 2; y++)
        {
            Fogs.Add(new List<GameObject>());
            IsFog.Add(new List<bool>());
        }

        for (int Y = 0; Y < Col; Y++)
        {
            int y = Col - 1 - Y;
            for (int x = 0; x < Row; x++)
            {
                // 안개 관련 정보를 저장하는 배열에 저장
                // 미로의 한 칸에 총 4개의 안개가 존재
                for(int a = 0; a < 2; a++)for(int b = 0; b<2;b++)
                    {
                        Fogs[Y * 2 + a].Add(Instantiate(Fog, new Vector3(x * Move_X + 2.5f + 5 * b, Y * Move_Y + 2.5f + 5 * a), new Quaternion(0, 0, 0, 0)));
                        IsFog[Y * 2 + a].Add(false);
                    }
                if (!Maze_Inf.Maze[x, Y].Left)     // 왼쪽 벽 생성 여부
                {
                    GameObject cnt = Instantiate(CWall,new Vector3(x * 10,y * 10 + 5,0),transform.rotation);
                    if (Maze_Inf.Maze[x, Y].Exit == Vector3.left) { Clear = cnt; cnt.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0.5f); cnt.tag = "ExitWall"; }
                }
                if (!Maze_Inf.Maze[x, Y].Right)     // 오른쪽 벽 생성 여부
                {
                    GameObject cnt = Instantiate(CWall, new Vector3(x * 10 +10, y* 10 + 5, 0), new Quaternion(0,0,90,0));
                    if (Maze_Inf.Maze[x, Y].Exit == Vector3.right) { Clear = cnt; cnt.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0.5f); cnt.tag = "ExitWall"; }
                }
                if (!Maze_Inf.Maze[x, Y].Down)     // 아래쪽 벽 생성 여부
                {
                    GameObject cnt = Instantiate(RWall, new Vector3(x * 10 + 5, y * 10, 0), transform.rotation);
                    if (Maze_Inf.Maze[x, Y].Exit == Vector3.down) { Clear = cnt; cnt.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0.5f); cnt.tag = "ExitWall"; }
                }
                if (!Maze_Inf.Maze[x, Y].Up)     // 위쪽 벽 생성 여부
                {
                    GameObject cnt = Instantiate(RWall, new Vector3(x * 10 + 5, y * 10+10, 0), transform.rotation);
                    if (Maze_Inf.Maze[x, Y].Exit == Vector3.up) { Clear = cnt; cnt.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0.5f); cnt.tag = "ExitWall"; }
                }
            }
        }
    }

    // Key 생성
    // 모든 Key의 위치가 겹치지 않으며, 플레이어와 특정 거리 이상의 위치에서 생성되도록 함.

    void MakeKey()
    {
        List<Tuple<int, int>> Cnt = new List<Tuple<int, int>>() { };
        Vector3 CCnt = new Vector3(Maze_Inf.Player_X, Maze_Inf.Player_Y);
        Tuple<int,int> a;
        double z = 0;
        for (int i = 0; i < KeyNum; i++)
        {
            do
            {
                int x = Random.Range(0, Row);
                int y = Random.Range(0, Col);
                a = new Tuple<int, int>(Random.Range(0, Row), Random.Range(0, Col));
                z = Vector3.Magnitude(CCnt - new Vector3(x, y, 0));
            }
            while (Cnt.Contains(a) && z >= (KeyWeight * 10));
            Cnt.Add(a);
        }
        for(int i = 0; i < KeyNum; i++)
        {
            Instantiate(Key).transform.position = new Vector3(Cnt[i].Item1 * Move_X + 5, Cnt[i].Item2 * Move_Y + 5, 0);
        }
    }
}
