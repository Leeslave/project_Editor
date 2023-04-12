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
    // ��, �Ʒ� �÷���( �� ��ȭ�� ���� ��� )
    public GameObject Up;
    public GameObject Down;
    // ��� �� ������ ���⿡ ��� �Ǵ� Object
    public GameObject Particle;
    // R��ư���� ������� ������ GameOver ȭ��
    public GameObject GameOver;
    // ���� Object
    public GameObject EndG;
    // ���� HP ���¸� ǥ�����ִ� Image
    public Image[] HPS;
    // HP ���� ǥ�ÿ� ���Ǵ� Sprite
    public Sprite HPOn;     // HP�� �������� ��Ÿ��
    public Sprite HPOff;    // HP�� ������ ��Ÿ��

    public int speed;               // �÷��̾��� �� �� �̵� �ӵ�
    public int CurHp;               // ���� HP
    public bool MovePing;           // ���� ��� ������� �÷��̾ �̵��ϴ����� ���� ��Ÿ��
    public bool MoveAble = true;    // �÷��̾� ���� ���� ���¸� ��Ÿ��
    bool RayAble = true;            // true�� ��쿡�� ��, �Ϸ� �����ɽ�Ʈ�� ����
    List<GameObject> PtL = new List<GameObject>();  // ����� ������ Particle Object�� List

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

    // �� �Ʒ��� ƨ��鼭 �̵��ϴ� ����
    void PingPong()
    {
        // ��, �� �̵��� �Է¹��� ���, Speed���� ����� �÷��̾��� ��ġ�� �̵���Ŵ.
        float x = Input.GetAxisRaw("Horizontal"); transform.position += new Vector3(x, 0, 0) * speed * Time.deltaTime;
        Vector2 RayCnt = Vector2.zero;
        // �Ʒ��� �̵��߿��� �Ʒ��� ����ĳ��Ʈ��, ���� �̵��߿��� ���� ����ĳ��Ʈ�� ����
        if (rigid.velocity.y < 0) RayCnt = Vector2.down;
        else RayCnt = Vector2.up;
        RaycastHit2D RayHit = Physics2D.Raycast(rigid.position, RayCnt, 0.4f, LayerMask.GetMask("Plat"));
        // ����ĳ��Ʈ�� Plat�� ����� �� �÷��̾��� �ӵ� ������ y���� ������ �ݴ�� �ٲ� ƨ��� ȿ���� �ָ� �浹�� Plat�� ���� ����
        if (RayHit.collider != null && RayAble)
        {
            rigid.velocity = new Vector2(rigid.velocity.x, rigid.velocity.y * (-1));
            RayHit.collider.gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.4f);
            // Plat�� ���� �ٽ� ������� ����.
            StartCoroutine(ChangeColor(RayHit.collider.gameObject));
            // �ߺ����� �浹�� Ž���ϴ� ���� ���� ���� �ѹ� �浹 �� ���� 0.2�� ���Ŀ� �浹 ������ �����ϰ� ��.
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

    // �÷��̾��� �̵� ����� ����.
    // Input : "Ping"�� ��� ƨ��� �������, "Normal"�� ��� �����¿�� ���� ������ �� ����.
    // 3������ ������ ���� �׻� "Ping"���·� ������
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

    // ������ ���
    IEnumerator RealEnd()
    {
        // RealEnd.Cs ����.
        EndG.SetActive(true);
        EndG.GetComponent<RealEnd>().Ending("Game Over", "������ ������...");

        yield return new WaitForSeconds(1);
        // �÷��̾ ������ ���� ������ ���� ���
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

    // ��Ȱ ���� �� �ʱ� ���·� ����
    private void OnEnable()
    {
        // ������ ���� �����Ǿ��� �� �۵����� �ʵ��� �ϱ� ����(�ش� ������ ���� �ʿ� ���� ����)
        if (CurHp != 3)
        {
            // PatternManager �� Timer ����
            PM.EndPT(false);
            PM.StartPT(0);
            PM.TM.IsTimeFlow = true;
            PM.TM.time = 0;
            PM.TM.MaxTime = PM.TM.TimeToSurvive;
        }
        // �÷��̾� ��� ���⿡ ���� ������Ʈ�� ��Ȱ��ȭ
        foreach (var a in PtL) a.SetActive(false);
        gameObject.transform.position = new Vector2(0, 0);
        // �� ���� ���� ��� ��, ���� ����� ä�� �����Ǳ� ������ �̸� ����
        Up.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
        Down.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
        MoveAble = true;
        ChangeType("Ping");
        speed = 10;
    }

    // ź���� �浹���� ��
    // PM�� TM������ PatternManager �� Timer ����
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
                // �÷��̾ ������ ����
                for (int i = 0; i < 9; i++)
                {
                    if (i == 4) continue;
                    PtL[i].transform.position = transform.position;
                    PtL[i].SetActive(true);
                    PtL[i].GetComponent<Rigidbody2D>().AddForce((PM.DE[i][0] + PM.DE[i][1]) * Random.Range(1, 4), ForceMode2D.Impulse);
                }
                // ü���� ��ĭ ���, 1�� �� Game Over Object ����
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

    // 3����� ���Ǵ� �̵� ��������� �ּ��� ���� ����
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
