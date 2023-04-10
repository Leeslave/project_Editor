using System.Collections;
using System.Collections.Generic;
using System.Net;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    Rigidbody2D rigid;
    public PatternManager PM;
    // 위, 아래 플랫폼( 색 변화를 위해 사용 )
    public GameObject Up;
    public GameObject Down;
    // 사망 시 나오는 연출에 사용 되는 Object
    public GameObject Particle;
    // R버튼으로 재시작이 가능한 GameOver 화면
    public GameObject GameOver;
    // 엔딩 Object
    public GameObject EndG;
    // 현재 HP 상태를 표시해주는 Image
    public Image[] HPS;
    // HP 상태 표시에 사용되는 Sprite
    public Sprite HPOn;     // HP가 존재함을 나타냄
    public Sprite HPOff;    // HP가 없음을 나타냄

    public int speed;               // 플레이어의 좌 우 이동 속도
    public int CurHp;               // 현재 HP
    public bool MovePing;           // 현재 어떠한 방식으로 플레이어가 이동하는지에 대해 나타냄
    public bool MoveAble = true;    // 플레이어 조작 가능 상태를 나타냄
    bool RayAble = true;            // true일 경우에만 상, 하로 레이케스트를 날림
    List<GameObject> PtL = new List<GameObject>();  // 사망시 나오는 Particle Object의 List

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        rigid.velocity = new Vector2(0, -speed);
        // Particle Object Pooling
        for (int i = 0; i < 9; i++) { PtL.Add(Instantiate(Particle)); PtL[i].SetActive(false); }
    }
    private void Update()
    {
        if (!MoveAble) return;
        if (MovePing) PingPong();
        else NormalMove();
    }

    // 위 아래로 튕기면서 이동하는 상태
    void PingPong()
    {
        // 좌, 우 이동을 입력받을 경우, Speed값에 기반해 플레이어의 위치를 이동시킴.
        float x = Input.GetAxisRaw("Horizontal"); transform.position += new Vector3(x, 0, 0) * speed * Time.deltaTime;
        Vector2 RayCnt = Vector2.zero;
        // 아래로 이동중에는 아래로 레이캐스트를, 위로 이동중에는 위로 레이캐스트를 날림
        if (rigid.velocity.y < 0) RayCnt = Vector2.down;
        else RayCnt = Vector2.up;
        RaycastHit2D RayHit = Physics2D.Raycast(rigid.position, RayCnt, 0.4f, LayerMask.GetMask("Plat"));
        // 레이캐스트에 Plat이 닿았을 때 플레이어의 속도 백터의 y축의 방향을 반대로 바꿔 튕기는 효과를 주며 충돌한 Plat의 색을 변경
        if (RayHit.collider != null && RayAble)
        {
            rigid.velocity = new Vector2(rigid.velocity.x, rigid.velocity.y * (-1));
            RayHit.collider.gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.4f);
            // Plat의 색을 다시 원래대로 돌림.
            StartCoroutine(ChangeColor(RayHit.collider.gameObject));
            // 중복으로 충돌을 탐지하는 것을 막기 위해 한번 충돌 된 이후 0.2초 이후에 충돌 판정이 가능하게 함.
            RayAble = false;
            Invoke("CanRay", 0.2f);
        }
    }
    IEnumerator ChangeColor(GameObject a)
    {
        yield return new WaitForSeconds(0.3f);
        a.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
        yield break;
    }

    void CanRay()
    {
        RayAble = true;
    }

    // 플레이어의 이동 방식을 정함.
    // Input : "Ping"일 경우 튕기는 방식으로, "Normal"일 경우 상하좌우로 직접 움직일 수 있음.
    // 3페이즈 삭제에 따라 항상 "Ping"상태로 고정됨
    public void ChangeType(string type)
    {
        if(type == "Ping")
        {
            rigid.velocity = new Vector2(0, -10);
            MovePing = true;
        }
        else if(type == "Normal")
        {
            rigid.velocity = new Vector2(0, 0);
            MovePing = false;
        }
    }

    // 엔딩을 출력
    IEnumerator RealEnd()
    {
        // RealEnd.Cs 참조.
        EndG.SetActive(true);
        EndG.GetComponent<RealEnd>().Ending("Game Over", "마음이 꺾였다...");

        yield return new WaitForSeconds(1);
        // 플레이어가 터지는 듯한 연출을 위해 사용
        foreach(var a in PtL)
        {
            a.SetActive(true);
            a.transform.position = transform.position;
            a.GetComponent<Rigidbody2D>().gravityScale = 1;
            a.GetComponent<Rigidbody2D>().AddForce((Vector2.up * Random.Range(5,15) + Vector2.left * Random.Range(-4,5)),ForceMode2D.Impulse);
        }
        gameObject.SetActive(false);
        yield break;
    }

    // 부활 했을 때 초기 상태로 돌림
    private void OnEnable()
    {
        // 조건은 씬이 생성되었을 때 작동되지 않도록 하기 위함(해당 시점에 굳이 필요 없는 연산)
        if (CurHp != 3)
        {
            // PatternManager 및 Timer 참조
            PM.EndPT(false);
            PM.StartPT(0);
            PM.TM.IsTimeFlow = true;
            PM.TM.time = 0;
            PM.TM.MaxTime = PM.TM.TimeToSurvive;
        }
        // 플레이어 사망 연출에 사용된 오브젝트들 비활성화
        foreach (var a in PtL) a.SetActive(false);
        gameObject.transform.position = new Vector2(0, 0);
        // 색 변경 도중 사망 시, 색이 변경된 채로 유지되기 때문에 이리 유지
        Up.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
        Down.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
        MoveAble = true;
        ChangeType("Ping");
        speed = 10;
    }

    // 탄막과 충돌했을 때
    // PM과 TM관련은 PatternManager 및 Timer 참조
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Bullet")
        {
            rigid.velocity = new Vector2(0, 0);
            MoveAble = false;
            PM.EndPT(true);
            PM.TM.IsTimeFlow = false;
            if (CurHp > 1)
            {
                // 플레이어가 터지는 연출
                for (int i = 0; i < 9; i++)
                {
                    if (i == 4) continue;
                    PtL[i].transform.position = transform.position;
                    PtL[i].SetActive(true);
                    PtL[i].GetComponent<Rigidbody2D>().AddForce((PM.DE[i][0] + PM.DE[i][1]) * Random.Range(1, 4), ForceMode2D.Impulse);
                }
                // 체력을 한칸 깍고, 1초 뒤 Game Over Object 생성
                HPS[--CurHp].sprite = HPOff;
                gameObject.SetActive(false);
                Invoke("OnGameOver", 1);
            }
            else
            {
                PM.EndPT(false);
                StartCoroutine(RealEnd());
            }
        }
    }

    void OnGameOver()
    {
        GameOver.SetActive(true);
    }

    // 3페이즈에 사용되는 이동 방식임으로 주석을 적지 않음
    void NormalMove()
    {
        MoveSpeed();
        DirMove();
    }

    void DirMove()
    {
        float y = Input.GetAxisRaw("Vertical");
        float x = Input.GetAxisRaw("Horizontal");
        Vector3 nextPos = new Vector3(x, y, 0) * speed * Time.deltaTime;
        transform.position += nextPos;
    }
    void MoveSpeed()
    {
        if (Input.GetButton("SlowMove")) speed = 5;
        else speed = 10;
    }
}
