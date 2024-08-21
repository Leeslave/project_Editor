using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using System.IO;

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
    public MazeTimer Timer;
    [NonSerialized] public List<List<GameObject>> Fogs = new List<List<GameObject>>();
    [NonSerialized] public List<List<bool>> IsFog = new List<List<bool>>();

    [NonSerialized] public bool IsCalcFog = false;

    public float Move_X;
    public float Move_Y;
    public int Difficulty;
    [NonSerialized] public int Col;
    [NonSerialized] public int Row;
    [NonSerialized] public int KeyNum;
    [NonSerialized] public int KeyWeight;

    [SerializeField] string[] Paths;

    [SerializeField] bool IsGoNextStage = false;

    [SerializeField] List<StageData> Stages;

    [SerializeField] Transform Walls;
    [SerializeField] Transform Objects;
    [SerializeField] Transform FogP;

    [System.Serializable]
    public class StageData
    {
        public int Col;
        public int Row;
        public int KeyNum;
        public int KeyWeight;
        public int Sight;
        public bool IsCalcFog;
        public bool IsCalcTime;
        public int Time;
    }

    void Awake()
    {
        try
        {
            Difficulty = GameSystem.Instance.GetTask("Maze");
        }
        catch
        {
            //Difficulty = 0;
        }

        // 클리어시 다음으로 넘어가게 임시로 해둔 거
        if (IsGoNextStage) Difficulty = PlayerPrefs.GetInt("MazeStage");
        else PlayerPrefs.DeleteKey("MazeStage");

        // Document Secret 폴더 오픈되게 임시로 해둔거
        if (PlayerPrefs.GetInt("DocumentTest") == 1) Difficulty = 10;

        if (Difficulty > 0) GetDifficulty();
        else MakeTutorial();

        

        Player.transform.position = new Vector3(Maze_Inf.Player_X * Move_X + 5, Maze_Inf.Player_Y * Move_Y + 5f, 0);
        
    }

    void MakeTutorial()
    {
        Player.GetComponent<PlayerMove>().Sight = 5;
        IsCalcFog = true;
        Timer.NowTime = 999;
        Timer.gameObject.SetActive(true);
        
        Maze_Inf = new MazeMap();
        Col = Row = 15;
        KeyNum = 1;
        Maze_Inf.MazeMaking(Col, Row,true);
        MakeItems(true);

        CreateLevel();
        
    }

    // �ӽ�(���� �̱����� � ������ �� �� ��������)
    void GetDifficulty()
    {
        Difficulty--;
        Col = Stages[Difficulty].Col; Row = Stages[Difficulty].Row;
        KeyNum = Stages[Difficulty].KeyNum; KeyWeight = Stages[Difficulty].KeyWeight;
        Player.GetComponent<PlayerMove>().Sight = Stages[Difficulty].Sight;
        IsCalcFog = Stages[Difficulty].IsCalcFog;
        Timer.gameObject.SetActive(Stages[Difficulty].IsCalcTime);
        Timer.NowTime = Stages[Difficulty].Time;

        Maze_Inf = new MazeMap();
        Maze_Inf.MazeMaking(Col, Row);
        CreateLevel();
        MakeItems();
    }


    void CreateLevel()
    {
        // �̷��� �ٱ� �κ��� �ѷ��δ�  �Ȱ��� ����.
        GameObject outsT = Instantiate(Fog, new Vector3((-3) * Move_X + 5, (Col + 5) * Move_Y + 10), new Quaternion(0, 0, 0, 0),FogP);
        outsT.transform.localScale = new Vector2(2 * Row * Move_X + 120 ,12 * Move_Y);
        GameObject outsL = Instantiate(Fog, new Vector3(-60, -60), new Quaternion(0, 0, 0, 0), FogP);
        outsL.transform.localScale = new Vector2(12 * Move_X, 2 * Col * Move_Y + 120);
        GameObject outsB = Instantiate(Fog, new Vector3(-60, -60), new Quaternion(0, 0, 0, 0), FogP);
        outsB.transform.localScale = new Vector2(2 * Row * Move_X + 120, 12 * Move_Y);
        GameObject outsR = Instantiate(Fog, new Vector3((Row + 5) * Move_X + 10, (-3) * Move_Y + 5), new Quaternion(0, 0, 0, 0), FogP);
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
                // �Ȱ� ���� ������ �����ϴ� �迭�� ����
                // �̷��� �� ĭ�� �� 4���� �Ȱ��� ����
                if(IsCalcFog)
                for(int a = 0; a < 2; a++)for(int b = 0; b<2;b++)
                    {
                        Fogs[Y * 2 + a].Add(Instantiate(Fog, new Vector3(x * Move_X + 2.5f + 5 * b, Y * Move_Y + 2.5f + 5 * a), new Quaternion(0, 0, 0, 0),FogP));
                        IsFog[Y * 2 + a].Add(false);
                    }
                if (!Maze_Inf.Maze[x, Y].Left)     // ���� �� ���� ����
                {
                    GameObject cnt = Instantiate(CWall,new Vector3(x * 10,y * 10 + 5,0),transform.rotation,Walls);
                    if (Maze_Inf.Maze[x, Y].Exit == Vector3.left) { Clear = cnt; cnt.GetComponent<SpriteRenderer>().color = new Color(1, 0, 0, 1f); cnt.tag = "ExitWall"; cnt.layer = 7; }
                }
                if (!Maze_Inf.Maze[x, Y].Right)     // ������ �� ���� ����
                {
                    GameObject cnt = Instantiate(CWall, new Vector3(x * 10 +10, y* 10 + 5, 0), new Quaternion(0,0,90,0), Walls);
                    if (Maze_Inf.Maze[x, Y].Exit == Vector3.right) { Clear = cnt; cnt.GetComponent<SpriteRenderer>().color = new Color(1, 0, 0, 1f); cnt.tag = "ExitWall"; cnt.layer = 7; }
                }
                if (!Maze_Inf.Maze[x, Y].Down)     // �Ʒ��� �� ���� ����
                {
                    GameObject cnt = Instantiate(RWall, new Vector3(x * 10 + 5, y * 10, 0), transform.rotation, Walls);
                    if (Maze_Inf.Maze[x, Y].Exit == Vector3.down) { Clear = cnt; cnt.GetComponent<SpriteRenderer>().color = new Color(1, 0, 0, 1f); cnt.tag = "ExitWall"; cnt.layer = 7; }
                }
                if (!Maze_Inf.Maze[x, Y].Up)     // ���� �� ���� ����
                {
                    GameObject cnt = Instantiate(RWall, new Vector3(x * 10 + 5, y * 10+10, 0), transform.rotation, Walls);
                    if (Maze_Inf.Maze[x, Y].Exit == Vector3.up) { Clear = cnt; cnt.GetComponent<SpriteRenderer>().color = new Color(1, 0, 0, 1f); cnt.tag = "ExitWall"; cnt.layer = 7; }
                }
                if (Maze_Inf.Maze[x,y].Test)
                    Instantiate(Key).transform.position = new Vector3(x * Move_X + 5, y * Move_Y + 5, 0);
            }
        }
    }

    // Key ����
    // ��� Key�� ��ġ�� ��ġ�� ������, �÷��̾�� Ư�� �Ÿ� �̻��� ��ġ���� �����ǵ��� ��.

    void MakeItems(bool IsTu = false)
    {
        if (IsTu)
        {
            float x = Maze_Inf.Player_X-2, y = Maze_Inf.Player_Y;
            var cnt = Instantiate(Key,Objects);
            cnt.transform.position = new Vector3(x-- * Move_X + 5,y * Move_Y + 5 , 0);
            SpriteRenderer s = cnt.GetComponent<SpriteRenderer>(); s.color = Color.green;
            Player.GetComponent<PlayerMove>().KeysTrans.Add(cnt.transform);
        }
        else
        {
            List<Tuple<int, int>> Cnt = new List<Tuple<int, int>>() { };
            Vector3 CCnt = new Vector3(Maze_Inf.Player_X, Maze_Inf.Player_Y);
            Tuple<int, int> a;
            double z = 0;
            for (int i = 0; i < KeyNum + 2; i++)
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
            for (int i = 0; i < KeyNum; i++)
            {
                GameObject cnt = Instantiate(Key,Objects);
                Player.GetComponent<PlayerMove>().KeysTrans.Add(cnt.transform);
                cnt.transform.position = new Vector3(Cnt[i].Item1 * Move_X + 5, Cnt[i].Item2 * Move_Y + 5, 0);
                SpriteRenderer s = cnt.GetComponent<SpriteRenderer>();
                if (i == 0) s.color = Color.green;
                else if (i == 1) s.color = Color.blue;
                else s.color = Color.red;
            }

        }
    }
}
