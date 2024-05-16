using System.Collections;
using System.Collections.Generic;
using System.Net;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
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

    public Image[] Unzips;

    public Sprite Unzip;
    public Sprite Zip;

    public int InitHP = 2;

    int HPForPattern = 2;    // �Ϲ� ���Ͽ� HP

    public int speed;               // �÷��̾��� �� �� �̵� �ӵ�
    public bool MoveAble = true;    // �÷��̾� ���� ���� ���¸� ��Ÿ��
    bool RayAble = true;            // true�� ��쿡�� ��, �Ϸ� �����ɽ�Ʈ�� ����
    List<GameObject> PtL = new List<GameObject>();  // ����� ������ Particle Object�� List

    bool OnStart = true;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        rigid.velocity = new Vector2(0, -speed);
        AS.Stop();
        HPForPattern = InitHP;
        // Particle Object Pooling
        for (int i = 0; i < 9; i++) { PtL.Add(Instantiate(Particle)); PtL[i].SetActive(false); }
        for (int i = 0; i < 3 - HPForPattern; i++) Unzips[i].gameObject.SetActive(false);
    }
    private void Update()
    {
        if (!MoveAble) return;
        PingPong();
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
            AS.Play();
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
        AS.Stop();
        RayAble = true;
    }

    // ������ ���
    void RealEnd()
    {
        
        EndG.GetComponent<RealEnd>().Ending(PM.CurPattern>=1);
    }

    // ��Ȱ ���� �� �ʱ� ���·� ����
    private void OnEnable()
    {
        // ������ ���� �����Ǿ��� �� �۵����� �ʵ��� �ϱ� ����(�ش� ������ ���� �ʿ� ���� ����)
        if (!OnStart)
        {
            // PatternManager �� Timer ����
            PM.EndPT(false);
            PM.NextPattern(ref HPForPattern,0);
            PM.ErrorObject.SetActive(false);
        }
        
        OnStart = false;
        // �÷��̾� ��� ���⿡ ���� ������Ʈ�� ��Ȱ��ȭ
        foreach (var a in PtL) a.SetActive(false);
        gameObject.transform.position = new Vector2(0, 0);
        // �� ���� ���� ��� ��, ���� ����� ä�� �����Ǳ� ������ �̸� ����
        Up.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
        Down.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
        MoveAble = true;
        speed = 10;
        rigid.velocity = new Vector2(0, -speed);
    }

    [SerializeField] AudioSource AS;
    // ź���� �浹���� ��
    // PM�� TM������ PatternManager �� Timer ����
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Bullet"))
        {
            PM.AD.MusicOff(true);
            MoveAble = false;
            if (PM.IsEnd) PM.Clear();
            else
            {
                // �÷��̾ ������ ����
                for (int i = 0; i < 9; i++)
                {
                    if (i == 4) continue;
                    PtL[i].transform.position = transform.position;
                    PtL[i].SetActive(true);
                    PtL[i].GetComponent<Rigidbody2D>().AddForce((PM.DE[i][0] + PM.DE[i][1]) * Random.Range(1, 4), ForceMode2D.Impulse);
                }
                GameOver.SetActive(true);
                gameObject.SetActive(false);
            }
            foreach (Image s in Unzips) s.sprite = Zip;
            if(HPForPattern > 0) HPForPattern = InitHP;
        }
        else if (collision.CompareTag("Trace"))
        {
            if (PM.CurPattern == 2) GameClear();
            else
            {
                collision.gameObject.SetActive(false);
                if (HPForPattern == 1) { foreach (Image s in Unzips) s.gameObject.SetActive(false); }
                else if (HPForPattern > 1) Unzips[3 - HPForPattern].sprite = Unzip;
                PM.NextPattern(ref HPForPattern);
            }
        }
    }

    public void GameClear()
    {
        if (GameSystem.Instance != null)
        {
            GameSystem.Instance.ClearTask("Dodge");
            LoadTestTrash.Instance.LoadScene = "Screen";
            SceneManager.LoadScene("LoadT");
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
