using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class PacObj : MonoBehaviour
{
    [SerializeField] LineRenderer Line;
    MazeMap maze;

    [SerializeField] int CurType = 1; // P, F
    Vector2Int TargetGrid = new Vector2Int(-1,-1);

    [SerializeField] Vector2Int InitGrid;

    List<Vector2Int> Path = new List<Vector2Int>();
    bool[,] visit;
    int[,] cost;
    Vector2Int[,] from;

    Rigidbody2D rigid;

    // A* Base
    public void CalcPos()
    {
        Path.Clear();

        for (int y = 0; y < col; y++) for (int x = 0; x < row; x++)
            {
                visit[x, y] = false; cost[x, y] = int.MaxValue;
            }

        PriorityQueue pq = new PriorityQueue();
        pq.push(new ARoute(CurGrid, 0, 0));
        cost[pq.top().x, pq.top().y] = 0;
        if (CurType == 1)
        {
            int rdx = 1, rdy = 1;
            do
            {
                do { rdx = Random.Range(0, 5); rdy = Random.Range(0, 5); } while (rdx == 2 && rdy == 2);
                TargetGrid = new Vector2Int(Mathf.Clamp(InitGrid.x - 2 + rdx, 0, row - 1), Mathf.Clamp(InitGrid.y - 2 + rdy, 0, col - 1));
            } while (TargetGrid == CurGrid);
        }
        int i = 0;

        while (!pq.isEmpty)
        {
            ARoute k = pq.top(); pq.pop();

            if (visit[k.x, k.y]) continue;

            if (k.x == TargetGrid.x && k.y == TargetGrid.y) break;


            visit[k.x, k.y] = true;

            int nx, ny, score_goal, score_self;
            // r
            if (maze.Maze[k.x, k.y].Right)
            {
                nx = k.x + 1; ny = k.y;
                score_goal = (Mathf.Abs(TargetGrid.x - nx) + Mathf.Abs(TargetGrid.y - ny));
                score_self = 1 + k.cost;
                if (cost[nx, ny] > score_self)
                {
                    cost[nx, ny] = score_self;
                    from[nx, ny] = new Vector2Int(k.x, k.y);
                    pq.push(new ARoute(nx, ny, score_self, score_self + score_goal));
                }
            }
            // l
            if (maze.Maze[k.x, k.y].Left)
            {
                nx = k.x - 1; ny = k.y;
                score_goal = 1 * (Mathf.Abs(TargetGrid.x - nx) + Mathf.Abs(TargetGrid.y - ny));
                score_self = 1 + k.cost;
                if (cost[nx, ny] > score_self)
                {
                    cost[nx, ny] = score_self;
                    from[nx, ny] = new Vector2Int(k.x, k.y);
                    pq.push(new ARoute(nx, ny, score_self, score_self + score_goal));
                }
            }
            // u
            if (maze.Maze[k.x, k.y].Up)
            {
                nx = k.x; ny = k.y - 1;
                score_goal = 1 * (Mathf.Abs(TargetGrid.x - nx) + Mathf.Abs(TargetGrid.y - ny));
                score_self = 1 + k.cost;
                if (cost[nx, ny] > score_self)
                {
                    cost[nx, ny] = score_self;
                    from[nx, ny] = new Vector2Int(k.x, k.y);
                    pq.push(new ARoute(nx, ny, score_self, score_self + score_goal));
                }
            }
            // d
            if (maze.Maze[k.x, k.y].Down)
            {
                nx = k.x; ny = k.y + 1;
                score_goal = 1 * (Mathf.Abs(TargetGrid.x - nx) + Mathf.Abs(TargetGrid.y - ny));
                score_self = 1 + k.cost;
                if (cost[nx, ny] > score_self)
                {
                    cost[nx, ny] = score_self;
                    from[nx, ny] = new Vector2Int(k.x, k.y);
                    pq.push(new ARoute(nx, ny, score_self, score_self + score_goal));
                }
            }
        }

        Stack<Vector2Int> sub = new Stack<Vector2Int>(); sub.Push(TargetGrid);

        i = 0;
        while (sub.Peek() != CurGrid) { sub.Push(from[sub.Peek().x, sub.Peek().y]); }
        Line.positionCount = sub.Count + 1;
        Line.SetPosition(0, transform.position);
        while (sub.Count > 0) { var s = sub.Pop(); Path.Add(s); Line.SetPosition(++i, new Vector3(s.x * 10 + 5, (col - 1 - s.y) * 10 + 5)); }

        CurPath = 1;
        NextPos = new Vector2(Path[CurPath].x * 10 + 5, (col - Path[CurPath].y - 1) * 10 + 5);
        Dir = new Vector2(Path[CurPath].x - CurGrid.x, CurGrid.y - Path[CurPath].y);
        SightCol.offset = Dir * SightRange * 0.5f;
        
        OnThink = false; if(ThinkCor != null) StopCoroutine(ThinkCor); ThinkCor = null;
    }

    int col, row;

    int hear = 3;
    int sight = 3;

    /// 주변 소음 발생시 해당 위치로 쫓아가는 Logic. 넣고싶으면 밑에 Event 등록 해제하면 됨.
    void NoiseCheck(Vector2Int PGrid)
    {
        if (Mathf.Abs(PGrid.x - CurGrid.x) + Mathf.Abs(PGrid.y - CurGrid.y) > hear || CurType == 3) return;
        CurType = 2;
        TargetGrid = PGrid;
        rigid.position = NextPos; CurGrid = Path[CurPath]; OnThink = true;
        CalcPos();
    }

    private void Start()
    {
        maze = MakeTile.ExMaze;
        col = MakeTile.ins.Col; row = MakeTile.ins.Row;
        visit = new bool[row, col]; cost = new int[row, col]; from = new Vector2Int[row, col];
        CurGrid = InitGrid = new Vector2Int(Mathf.FloorToInt(transform.position.x * 0.1f), col - 1 - Mathf.FloorToInt(transform.position.y * 0.1f));
        rigid = GetComponent<Rigidbody2D>();
        mesh = new Mesh(); SightObj.GetComponent<MeshFilter>().mesh = mesh;
        SightRend = SightObj.GetComponent<MeshRenderer>(); SightRend.material = new Material(SightRend.material); SightRend.material.SetVector("_Color", Color.yellow);
        SightCol = SightObj.GetComponent<BoxCollider2D>();
        Line = Instantiate(Line.gameObject).GetComponent<LineRenderer>(); Line.endColor = new Color(Random.Range(0,1f), Random.Range(0, 1f), Random.Range(0, 1f));
        int t = 0;
        for(int i = 1; i < 61; i++)
        {
            Tris[t++] = 0; Tris[t++] = i; Tris[t++] = i + 1;
        }
        StartCoroutine(SetMesh());
        //PlayerMove.Noise += NoiseCheck;
        CalcPos();
    }

    [SerializeField] Vector2Int CurGrid;
    [SerializeField] Vector2 NextPos = Vector2.zero;
    [SerializeField] Vector2 Dir = Vector2.zero;
    [SerializeField] bool OnThink = true;
    [SerializeField] bool OnChase = false;
    [SerializeField] int CurPath = 0;

    [SerializeField] GameObject SightObj;
    [SerializeField] int _SightRange = 20;
    public int SightRange
    { 
        get => _SightRange; 
        set 
        {
            if (SightCol == null || _SightRange == value) return;
            _SightRange = value;
            SightCol.size = new Vector2(_SightRange, _SightRange);
            SightCol.offset = SightCol.offset = Dir * _SightRange * 0.5f;
            SightRend.material.SetFloat("_SightVar", 0.05f / _SightRange);
        } 
    }
    MeshRenderer SightRend;
    BoxCollider2D SightCol;
    Vector3[] Points = new Vector3[62];
    int[] Tris = new int[60*3];
    float agstep = Mathf.Deg2Rad;
    Mesh mesh;

    
    WaitForSeconds wfs = new WaitForSeconds(0.1f);
    // 앞에 시야용 Mesh
    IEnumerator SetMesh()
    {
        
        while (true)
        {
            yield return wfs;
            if (Dir == Vector2.zero) continue;
            float ang = Mathf.Atan2(Dir.y, Dir.x);
            Points[0] = Vector2.zero;
            for (int i = -30; i <= 30; i++)
            {
                var angDir = new Vector2(Mathf.Cos(ang + agstep * i), Mathf.Sin(ang + agstep * i));
                var hit = Physics2D.Raycast(rigid.position, angDir, SightRange, 1 << 4);
                if (hit) Points[i + 31] = hit.point - rigid.position;
                else Points[i + 31] = angDir * SightRange;
            }

            mesh.Clear();
            mesh.vertices = Points;
            mesh.triangles = Tris;
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
        }
    }

    Coroutine Sighter = null;

    [SerializeField] Vector3 LastSeen = Vector3.zero;
    float NextSub = 1, distSub = 1;

    private void FixedUpdate()
    {
        if (OnThink || Dir == Vector2.zero) return;
        if (Dir.y == 0) rigid.position = new Vector2(rigid.position.x, Mathf.Lerp(rigid.position.y, NextPos.y, Mathf.Clamp01(Time.fixedDeltaTime * 12)));
        else if (Dir.x == 0) rigid.position = new Vector2(Mathf.Lerp(rigid.position.x, NextPos.x, Mathf.Clamp01(Time.fixedDeltaTime * 12)), rigid.position.y);
        distSub = (rigid.position - NextPos).magnitude;

        NextSub = Mathf.Min((CurType == 1 ? 5f : 15f) * Time.fixedDeltaTime, distSub);
        rigid.MovePosition(rigid.position + Dir * NextSub);
        if (distSub <= 0.1f)
        {
            rigid.position = NextPos;
            CurGrid = Path[CurPath++];
            if (Path.Count > CurPath)
            {
                NextPos = new Vector2(Path[CurPath].x * 10 + 5, (col - Path[CurPath].y - 1) * 10 + 5);
                Dir = new Vector2(Path[CurPath].x - CurGrid.x, CurGrid.y - Path[CurPath].y); SightCol.offset = Dir * SightRange * 0.5f;
            }
            else
            {
                OnThink = true;
                if (ThinkCor == null) ThinkCor = StartCoroutine(OnThinking());
            }
        }
        
        
    }

    IEnumerator SightCheck()
    {
        
        while (true)
        {
            yield return wfs;
            var OTP = MakeTile.ins.Player.transform.position - transform.position;
            float ang = Vector2.Angle(OTP, Dir);
            if (Mathf.Abs(ang) > 30) continue;
            var hit = Physics2D.Raycast(rigid.position, OTP.normalized, SightRange, 1 << 4);
            if (!OnChase)
            {
                if (!hit) // Patrol 중 최초 발견
                {
                    SightRend.material.SetVector("_Color", Color.red); SightRange = 40;
                    CurType = 3; TargetGrid = MakeTile.ins.CurPlayerPos; CalcPos();
                    OnChase = true;
                }
            }
            else
            {
                TargetGrid = MakeTile.ins.CurPlayerPos; LastSeen = MakeTile.ins.Player.transform.position; CalcPos();
                // 추격 도중 놓침 -> 다음 이동 Grid 예측
                if (hit) 
                {
                    // Grid 중앙 대비 이동한 위치 계산
                    float dx = LastSeen.x - MakeTile.ins.CurPlayerPos.x * 10f - 5; 
                    float dy = LastSeen.y - MakeTile.ins.CurPlayerPos.y * 10f - 5;
                    TargetGrid = MakeTile.ins.CurPlayerPos;
                    if (Mathf.Abs(dx) > Mathf.Abs(dy)) Mathf.Clamp(TargetGrid.x = TargetGrid.x + (dx > 0 ? 1 : -1),0,row-1);
                    else TargetGrid.y = Mathf.Clamp(TargetGrid.y + (dy > 0 ? -1 : 1),0,col-1); // y는 반전해서 저장하니까 이 순서 맞음
                    OnChase = false; 
                    break; 
                }
            }
        }
        Sighter = null;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (Sighter == null) { Sighter = StartCoroutine(SightCheck()); }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (Sighter != null && !OnChase) { StopCoroutine(Sighter); Sighter = null;}
    }

    Coroutine ThinkCor = null;
    IEnumerator OnThinking() { yield return new WaitForSeconds(3); OnThink = false; CurType = 1; SightRend.material.SetVector("_Color", Color.yellow); SightRange = 20; CalcPos(); }
}
