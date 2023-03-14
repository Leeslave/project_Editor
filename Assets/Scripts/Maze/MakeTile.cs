using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakeTile : MonoBehaviour
{
    [SerializeField]
    public GameObject CWall;
    public GameObject RWall;
    public GameObject Player;
    public Map Maze_Inf;
    public float Move_X;
    public float Move_Y;
    public int Col;
    public int Row;

    void Awake()
    {
        Maze_Inf = new Map();
        Maze_Inf.MazeMaking(Col, Row);
        CreateLevel();
        GameObject cnt = Instantiate(Player, new Vector3(Maze_Inf.Player_X * Move_X + 0.5f, Maze_Inf.Player_Y * Move_Y + 0.5f, 0), transform.rotation);
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
                    GameObject cnt = Instantiate(CWall,new Vector3(x,y + 0.5f,0),transform.rotation);
                }
                if (!Maze_Inf.Maze[x, Y].Right)     // 坷弗率 寒 积己 咯何
                {
                    GameObject cnt = Instantiate(CWall, new Vector3(x+1, y + 0.5f, 0), new Quaternion(0,0,90,0));
                }
                if (!Maze_Inf.Maze[x, Y].Down)     // 酒贰率 寒 积己 咯何
                {
                    GameObject cnt = Instantiate(RWall, new Vector3(x + 0.5f, y, 0), transform.rotation);
                }
                if (!Maze_Inf.Maze[x, Y].Up)     // 困率 寒 积己 咯何
                {
                    GameObject cnt = Instantiate(RWall, new Vector3(x + 0.5f, y+1, 0), transform.rotation);
                }
            }
        }
    }
}
