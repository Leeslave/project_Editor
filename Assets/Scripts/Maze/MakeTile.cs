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
    public int Col;
    public int Row;
    public int KeyNum;
    public int KeyWeight = 5;

    void Awake()
    {
        Maze_Inf = new MazeMap();
        Maze_Inf.MazeMaking(Col, Row);
        Player.transform.position = new Vector3(Maze_Inf.Player_X * Move_X + 5, Maze_Inf.Player_Y * Move_Y + 5f, 0);
        CreateLevel();
        MakeKey();
    }


    void CreateLevel()
    {
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
                for(int a = 0; a < 2; a++)for(int b = 0; b<2;b++)
                    {
                        Fogs[Y * 2 + a].Add(Instantiate(Fog, new Vector3(x * Move_X + 2.5f + 5 * b, Y * Move_Y + 2.5f + 5 * a), new Quaternion(0, 0, 0, 0)));
                        IsFog[Y * 2 + a].Add(false);
                    }
                if (!Maze_Inf.Maze[x, Y].Left)     // 哭率 寒 积己 咯何
                {
                    GameObject cnt = Instantiate(CWall,new Vector3(x * 10,y * 10 + 5,0),transform.rotation);
                    if (Maze_Inf.Maze[x, Y].Exit == Vector3.left) { Clear = cnt; cnt.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0.5f); cnt.tag = "ExitWall"; }
                }
                if (!Maze_Inf.Maze[x, Y].Right)     // 坷弗率 寒 积己 咯何
                {
                    GameObject cnt = Instantiate(CWall, new Vector3(x * 10 +10, y* 10 + 5, 0), new Quaternion(0,0,90,0));
                    if (Maze_Inf.Maze[x, Y].Exit == Vector3.right) { Clear = cnt; cnt.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0.5f); cnt.tag = "ExitWall"; }
                }
                if (!Maze_Inf.Maze[x, Y].Down)     // 酒贰率 寒 积己 咯何
                {
                    GameObject cnt = Instantiate(RWall, new Vector3(x * 10 + 5, y * 10, 0), transform.rotation);
                    if (Maze_Inf.Maze[x, Y].Exit == Vector3.down) { Clear = cnt; cnt.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0.5f); cnt.tag = "ExitWall"; }
                }
                if (!Maze_Inf.Maze[x, Y].Up)     // 困率 寒 积己 咯何
                {
                    GameObject cnt = Instantiate(RWall, new Vector3(x * 10 + 5, y * 10+10, 0), transform.rotation);
                    if (Maze_Inf.Maze[x, Y].Exit == Vector3.up) { Clear = cnt; cnt.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0.5f); cnt.tag = "ExitWall"; }
                }
            }
        }
    }
    void MakeKey()
    {
        List<Tuple<int, int>> Cnt = new List<Tuple<int, int>>() { };
        Cnt.Add(new Tuple<int,int>(Maze_Inf.Player_X,Maze_Inf.Player_Y));
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
            while (Cnt.Contains(a) && z >= KeyWeight * 10);
            Cnt.Add(a);
        }
        for(int i = 1; i <= KeyNum; i++)
        {
            Instantiate(Key).transform.position = new Vector3(Cnt[i].Item1 * Move_X + 5, Cnt[i].Item2 * Move_Y + 5, 0);
        }
    }
}
