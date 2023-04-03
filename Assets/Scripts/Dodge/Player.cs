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
    public GameObject Up;
    public GameObject Down;
    public GameObject Particle;
    public GameObject GameOver;
    public GameObject EndG;
    public Image[] HPS;
    public Sprite HPOn;
    public Sprite HPOff;

    public int speed;
    public int CurHp;
    public float MaxSpeed_Y;
    public bool IsGameOver;
    public bool MovePing;
    public bool MoveAble = true;
    bool RayAble = true;
    List<GameObject> PtL = new List<GameObject>();

    private void Awake()
    {
        IsGameOver = false;
        rigid = GetComponent<Rigidbody2D>();
        rigid.velocity = new Vector2(0, -speed);
        for (int i = 0; i < 9; i++) { PtL.Add(Instantiate(Particle)); PtL[i].SetActive(false); }
    }
    private void FixedUpdate()
    {
        if (!MoveAble) return;
        if (MovePing) PingPong();
        else NormalMove();
    }

    void PingPong()
    {
        float x = Input.GetAxisRaw("Horizontal"); transform.position += new Vector3(x, 0, 0) * speed * Time.deltaTime;
        Vector2 RayCnt = Vector2.zero;
        if (rigid.velocity.y < 0) RayCnt = Vector2.down;
        else RayCnt = Vector2.up;
        RaycastHit2D RayHit = Physics2D.Raycast(rigid.position, RayCnt, 0.4f, LayerMask.GetMask("Plat"));
        if (RayHit.collider != null && RayAble)
        {
            rigid.velocity = new Vector2(rigid.velocity.x, rigid.velocity.y * (-1));
            StartCoroutine(ChangeColor(RayHit.collider.gameObject));
            RayHit.collider.gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.4f);
            RayAble = false;
            Invoke("CanRay", 0.2f);
        }
    }

    void CanRay()
    {
        RayAble = true;
    }

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

    IEnumerator ChangeColor(GameObject a)
    {
        yield return new WaitForSeconds(0.3f);
        a.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
        yield break;
    }
    IEnumerator RealEnd()
    {
        ChangeType("End");
        rigid.velocity = new Vector2(0, 0);
        MoveAble = false;
        EndG.SetActive(true);
        EndG.GetComponent<RealEnd>().Ending("Game Over", "마음이 꺾였다...");
        yield return new WaitForSeconds(1);
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

    private void OnEnable()
    {
        if (CurHp != 3)
        {
            PM.EndPT(false);
            PM.StartPT(0);
            PM.TM.IsTimeFlow = true;
            PM.TM.time = 0;
            PM.TM.MaxTime = PM.TM.TimeToSurvive;
        }
        foreach (var a in PtL) a.SetActive(false);
        gameObject.transform.position = new Vector2(0, 0);
        ChangeType("Ping");
        speed = 10;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Bullet")
        {
            PM.EndPT(true);
            PM.TM.IsTimeFlow = false;
            if (CurHp > 1)
            {
                for (int i = 0; i < 9; i++)
                {
                    if (i == 4) continue;
                    PtL[i].transform.position = transform.position;
                    PtL[i].SetActive(true);
                    PtL[i].GetComponent<Rigidbody2D>().AddForce((PM.DE[i][0] + PM.DE[i][1]) * Random.Range(1, 4), ForceMode2D.Impulse);
                }
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
}
