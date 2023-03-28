using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MakeTile : MonoBehaviour
{
    [SerializeField]
    public GameObject CWall;
    public GameObject RWall;
    public GameObject Player;
    public MazeMap Maze_Inf;
    public GameObject Clear;

    public float Move_X;
    public float Move_Y;
    public int Col;
    public int Row;

    void Awake()
    {
        Maze_Inf = new MazeMap();
        Maze_Inf.MazeMaking(Col, Row);
        CreateLevel();
        Player.transform.position = new Vector3(Maze_Inf.Player_X * Move_X + 5, Maze_Inf.Player_Y * Move_Y + 5f, 0);
    }


    void CreateLevel()
    {
        for (int Y = 0; Y < Col; Y++)
        {
            int y = Col - 1 - Y;
            for (int x = 0; x < Row; x++)
            {
                if (!Maze_Inf.Maze[x, Y].Left)     // 哭率 寒 积己 咯何
                {
                    GameObject cnt = Instantiate(CWall,new Vector3(x * 10,y * 10 + 5,0),transform.rotation);
                    if (Maze_Inf.Maze[x, Y].Exit == Vector3.left) { cnt.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0); cnt.tag = "ExitWall"; }
                }
                if (!Maze_Inf.Maze[x, Y].Right)     // 坷弗率 寒 积己 咯何
                {
                    GameObject cnt = Instantiate(CWall, new Vector3(x * 10 +10, y* 10 + 5, 0), new Quaternion(0,0,90,0));
                    if (Maze_Inf.Maze[x, Y].Exit == Vector3.right) { cnt.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0); cnt.tag = "ExitWall"; }
                }
                if (!Maze_Inf.Maze[x, Y].Down)     // 酒贰率 寒 积己 咯何
                {
                    GameObject cnt = Instantiate(RWall, new Vector3(x * 10 + 5, y * 10, 0), transform.rotation);
                    if (Maze_Inf.Maze[x, Y].Exit == Vector3.down) { cnt.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0); cnt.tag = "ExitWall"; }
                }
                if (!Maze_Inf.Maze[x, Y].Up)     // 困率 寒 积己 咯何
                {
                    GameObject cnt = Instantiate(RWall, new Vector3(x * 10 + 5, y * 10+10, 0), transform.rotation);
                    if (Maze_Inf.Maze[x, Y].Exit == Vector3.up) { cnt.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0); cnt.tag = "ExitWall"; }
                }
            }
        }
    }
}
