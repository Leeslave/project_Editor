using System;
using UnityEngine;
using TMPro;
using UnityEngine.Scripting;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.SceneManagement;

public class PlayerMove : MonoBehaviour
{
    [SerializeField]
    public MakeTile MT;
    // X, Y�� �̵� �ӵ�
    public float Move_X;
    public float Move_Y;
    // Player�� �̵� ���� ����
    public bool MoveAble;
    // �Է��� �߻��� ���� ���� �Է±����� ������ ����
    public float InputDelay;
    // �÷��̾��� �þ�
    public int Sight;
    // MainCamera
    public GameObject maincam;
    // KeyText
    public TMP_Text KeyText;
    // Clear �� ��� ����
    public GameObject Clear;
    // Timer
    public GameObject Timer;
    // ���� ��ܿ� ��� Icon(��ħ��)
    public GameObject DirIcon;
    // �ⱸ ��ġ�� ��Ÿ���ִ� ȭ��ǥ
    public GameObject DirMark;
    // ���� ��ܿ� ��� Icon(Ƚ��)
    public GameObject FireIcon;
    // �׵��� Key���� Object�� �����ϴ� List
    // �� �� ���� ��� ���� ���Ǹ� ���� Player Object�� 0���� �����Ѵ�.
    List<GameObject> KeyTrain = new List<GameObject>();
    // KeyTrain�� ������ Object�� �̵� �� ������ ��ġ. Key �׵� �� �ش� Key�� ��ġ ������ ���� ���
    Vector3 LastTrans;

    // RayHit�� �ӽ� ����
    RaycastHit2D rayHit;
    // Player�� RigidBody Component�� ���� ����
    Rigidbody2D rigid;
    // �÷��̾��� �̵� ��꿡 ����.
    // Bf�� �̵� �� ��ġ, N�� �̵� ���� ��ġ
    float Bf_X;
    float Bf_Y;
    float NX;
    float NY;
    // ������ Ŭ���� ���⿡ ���(�� ����)
    bool IsEnd = false;
    bool MS = true;
    // �÷��̾��� �̵� ������ ���� �ӽ� ����
    Vector3 Dir;

    Vector3 VCnt;

    // ���� �������� �Ծ����� ����
    bool IsFire = false;

    bool IsCom = false;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        Bf_X = transform.position.x;
        Bf_Y = transform.position.y;
        MoveAble = true;
        Dir = Vector3.up;
        KeyTrain.Add(gameObject);
        LastTrans = transform.position;
        maincam.transform.position = new Vector3(transform.position.x, transform.position.y, -10);
    }
    private void Start()
    {
        CalcFog();
        KeyText.text = $"{KeyTrain.Count - 1}/{MT.KeyNum}";
    }

    void FixedUpdate()
    {
        if (MoveAble && MS)
        {
            NX = 0;
            NY = 0;
            // �̵� ����
            if (Input.GetButton("Horizontal"))
            {
                NX = Input.GetAxisRaw("Horizontal") * 10;
                Dir = NX < 0 ? Vector3.left : Vector3.right;
            }
            else if (Input.GetButton("Vertical"))
            {
                NY = Input.GetAxisRaw("Vertical") * 10;
                Dir = NY < 0 ? Vector3.down : Vector3.up;
            }

            if (NX != 0 || NY != 0)   // �������� ����
            {
                bool IsMoveNext = true;     // ���� �̵� ��, ������ �������� ���� �ⱸ, ���� �ε����ٸ� �̵����� ������ ����. 

                rayHit = Physics2D.Raycast(transform.position, Dir, 10, LayerMask.GetMask("Plat"));

                if (rayHit.collider != null)
                {
                    if(rayHit.collider.tag == "Wall")
                    {
                        IsMoveNext = false;
                    }
                    else if(rayHit.collider.tag == "ExitWall")
                    {
                        if (!IsEnd)     // ��� Key�� ������� �ⱸ�� �����ϸ�, �׷��� ������ �̵� �Ұ�.
                        {
                            if (KeyTrain.Count - 1 == MT.KeyNum) { StartCoroutine(ClearGame()); }
                            IsMoveNext = false;
                        }
                        else           // Ŭ����
                        {
                            Clear.SetActive(true);
                            PlayerPrefs.SetString("Clear", "Y");
                            SceneManager.LoadScene("TestT");
                            Destroy(gameObject);
                        }
                    }
                }
                // RayCast�� ������ ����
                rayHit = Physics2D.Raycast(transform.position, Dir, 10, LayerMask.GetMask("Default"));
                
                if (rayHit.collider != null && IsMoveNext)
                {
                    switch (rayHit.collider.name)
                    {
                        case "Key(Clone)":
                            // �÷��̾��� �ڸ� ������� Key�� Ư�� ��, �÷��̾�� �浹�� �� ��������, �ش� ���꿡 ������ ���� �ʴ� tag �� layer�� ����.
                            KeyTrain.Add(rayHit.collider.gameObject);
                            rayHit.collider.tag = "Untagged";
                            rayHit.collider.gameObject.layer = 6;
                            // ���� �� ���� Object�� ���� �̵� ��ġ�� Key�� ��ġ�� ����
                            rayHit.collider.gameObject.transform.position = LastTrans;
                            KeyText.text = $"{KeyTrain.Count - 1}/{MT.KeyNum}";
                            break;
                        // ��ħ�� �������� �Ծ��� ���, �ⱸ�� ��ġ�� ǥ���ϴ� ȭ��ǥ ����
                        case "Compass(Clone)":
                            Destroy(rayHit.collider.gameObject);
                            IsCom = true;
                            DirMark.SetActive(true);
                            DirIcon.SetActive(true);
                            break;
                        // Ƚ�� �������� �Ծ��� ���, 30�ʰ� ������ �þ� ���� ���� ��� �Ȱ��� �Ⱦ��.
                        case "Fire(Clone)":
                            Destroy(rayHit.collider.gameObject);
                            IsFire = true;
                            FireIcon.SetActive(true);
                            Invoke("FireOff", 30);
                            break;
                    }
                }
                if (IsMoveNext)
                {
                    LastTrans = KeyTrain[KeyTrain.Count-1].transform.position;
                    // KeyTrain�� ��� ��� Key���� ��ġ�� �ڽ� �ٷ� ���� Object�� ��ġ�� ������.(0�� Player������ ��� ����)
                    for(int i = KeyTrain.Count - 1; i > 0; i--) KeyTrain[i].transform.position = KeyTrain[i - 1].transform.position;
                    Bf_X = transform.position.x;
                    Bf_Y = transform.position.y;
                    transform.Translate(new Vector3(NX, NY, 0));
                    maincam.transform.position = new Vector3(transform.position.x, transform.position.y, -10);
                    CalcFog();
                    if (IsCom)
                    {
                        VCnt = (MT.Clear.transform.position - transform.position).normalized;
                        DirMark.transform.position = transform.position + (VCnt) * 7;
                        DirMark.transform.rotation = Quaternion.Euler(0,0,Mathf.Atan2(VCnt.y,VCnt.x) * Mathf.Rad2Deg);
                    }
                }
                // �÷��̾��� ���� Ȥ�� ���� �̵�Ű�� �Է� �Ǿ��� ��� InputDelay ������ ���� �Է��� ���� �� ���� ��
                MoveAble = false;
                Invoke("AbleMove", InputDelay);
            }
        }
    }

    // ���� Ŭ���� ����.
    // �� ���� Key���� 0.5�ʵ��� y������ 5��ŭ �̵���Ű�� �� �� �ش� ��ġ���� ������ �̵���Ŵ(�� �ð��� Key�� ��ġ�� ���� �ٸ�)
    // ��� Key�� ������ ���� ����Ǵ� �ִϸ��̼��� ���(���� �ش� �ִϸ��̼��� ���⿡ ���� ����)
    // �� �� ���� ���� ������ Ŭ���� ��.
    IEnumerator ClearGame()
    {
        Timer.SetActive(false);
        Time.timeScale = 2;
        MS = false;
        for (int i = KeyTrain.Count - 1; i > 0; i--)
        {
            for (int x = 0; x < 50; x++)
            {
                KeyTrain[i].transform.Translate(0,0.1f,0);
                yield return new WaitForSeconds(0.01f);
            }
            yield return new WaitForSeconds(0.5f);
            Vector3 cnt = (MT.Clear.transform.position - KeyTrain[i].transform.position).normalized;
            while (Vector3.Magnitude(MT.Clear.transform.position - KeyTrain[i].transform.position) > 1)
            {
                KeyTrain[i].transform.Translate(cnt/2);
                yield return new WaitForSeconds(0.01f);
            }
            yield return new WaitForSeconds(0.25f);
            KeyTrain[i].SetActive(false);
            yield return new WaitForSeconds(0.5f);
        }
        MT.Clear.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0,0);
        MS = true; IsEnd = true;
        Time.timeScale = 1;
    }

    // �÷��̾��� �þ� ������ ����� ( Instantiate, SetActive�� �ƴ� ���� ������ �����Ѵ�.)
    // �÷��̾ �߽����� �ϴ� ���̰� Sight�� ���簢���� �����Ѵ�.
    // �ش� ���簢���� �̷��� ��ǥ�� �����Ͽ�, Ư�� �κ��� �̷��� � Col,Row�� �ش��ϴ��� ������ ��
    // �÷��̾ �߽����� �ش� ��ǥ�� LayCast�� ����, ���� �ε����� �ʰ� �ش� ��ǥ���� �����Ѵٸ�, �ش� �κ��� �Ȱ��� �ȴ´�(������ 0���� ����)
    // �ѹ� �þ߰� ��Ҵ� �κ���, ���� ������ �ξ��� List�� �湮 ���θ� �����Ͽ� ���� �Ȱ��� �����ϴ� ������ �� �� ���� �帰 �Ȱ��� �ٲ۴�.(������ 0.5�� ����)
    // ���� �ε��� ��� �ش� �κп� �湮�� ���� �־����� �帰 �Ȱ��� ����.
    // ���� �÷��̾ �̵��� ������ �ݴ��̸� �Ÿ��� Sight + 1�� �κп� ���� �Ȱ� ���� ���� ������ �����Ѵ�.

    void CalcFog()
    {
        int CurY = (int)((transform.position.y - 5) / Move_X);
        int CurX = (int)((transform.position.x - 5) / Move_X);

        for (int y = CurY - Sight + 1; y <= CurY + Sight - 1; y++)
        {
            // �ش� ��ǥ�� �̷��� ���� ���̸� �������� �ʴ´�.
            if (y >= MT.Col || y < 0) continue;
            for(int x = CurX - Sight + 1; x <= CurX + Sight - 1; x++)
            {
                if (x >= MT.Row || x < 0) continue;
                for (int a = 0; a < 2; a++) for (int b = 0; b < 2; b++)
                    {
                        // �� ĭ�� �� 4���� �Ȱ��� ����������, �ش� �κ��� ������ ���� ��
                        int dx = x * 2 + b;
                        int dy = y * 2 + a;
                        // �����ɽ�Ʈ�� ���� ���Ϳ� �ش� ������ �� ���
                        Vector3 RayVec = new Vector3(x * Move_X + 5 - 2.5f + 5 * b, y * Move_Y + 5 - 2.5f + 5 * a, 0) - transform.position;
                        float S = Vector3.Magnitude(RayVec) / Move_X;
                        if (S > Sight) continue;

                        if (IsFire)
                        {
                            MT.Fogs[dy][dx].GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0);
                            MT.IsFog[dy][dx] = true;
                        }
                        else
                        {
                            rayHit = Physics2D.Raycast(transform.position, RayVec, S * Move_X, LayerMask.GetMask("Plat"));

                            // �浹�� �Ȱ� ����
                            if (rayHit.collider == null)
                            {
                                MT.Fogs[dy][dx].GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0);
                                MT.IsFog[dy][dx] = true;
                            }
                            // �̵��ߴ� �κ� �ݴ����� ���, �ѹ� �Ȱ��� ���� ���� ��������, �帰 �Ȱ��� ǥ��.
                            else
                            {
                                if (MT.IsFog[dy][dx] == true) MT.Fogs[dy][dx].GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0.5f);
                            }
                        }
                    }
            }
        }
        // �߰��� �̵� ���� �ݴ� ���⿡ ����, �Ÿ� + 1�� �Ȱ��� ���� �̹� ������ ���̸� �帰 �Ȱ��� ����.
        if (Dir == Vector3.up)
        {
            if (CurY - Sight < 0) return;
            for (int x = CurX - Sight + 1; x <= CurX + Sight - 1; x++)
            {
                if (x >= MT.Row || x < 0) continue;
                for (int a = 0; a < 2; a++) for (int b = 0; b < 2; b++)
                    {
                        int dx = x * 2 + b;
                        int dy = (CurY-Sight) * 2 + a;
                        if (MT.IsFog[dy][dx] == true) MT.Fogs[dy][dx].GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0.5f);
                    }
            }
        }
        else if (Dir == Vector3.down)
        {
            if (CurY + Sight >= MT.Col) return;
            for (int x = CurX - Sight + 1; x <= CurX + Sight - 1; x++)
            {
                if (x >= MT.Row || x < 0) continue;
                for (int a = 0; a < 2; a++) for (int b = 0; b < 2; b++)
                    {
                        int dx = x * 2 + b;
                        int dy = (CurY + Sight) * 2 + a;
                        if (MT.IsFog[dy][dx] == true) MT.Fogs[dy][dx].GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0.5f);
                    }
            }
        }
        else if (Dir == Vector3.left)
        {
            if (CurX + Sight >= MT.Row) return;
            for (int x = CurY - Sight + 1; x <= CurY + Sight - 1; x++)
            {
                if (x >= MT.Col || x < 0) continue;
                for (int a = 0; a < 2; a++) for (int b = 0; b < 2; b++)
                    {
                        int dx = (CurX + Sight) * 2 + b;
                        int dy = x * 2 + a;
                        if (MT.IsFog[dy][dx] == true) MT.Fogs[dy][dx].GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0.5f);
                    }
            }
        }
        else if (Dir == Vector3.right)
        {
            if (CurX - Sight < 0) return;
            for (int x = CurY - Sight + 1; x <= CurY + Sight - 1; x++)
            {
                if (x >= MT.Col || x < 0) continue;
                for (int a = 0; a < 2; a++) for (int b = 0; b < 2; b++)
                    {
                        int dx = (CurX - Sight) * 2 + b;
                        int dy = x * 2 + a;
                        if (MT.IsFog[dy][dx] == true) MT.Fogs[dy][dx].GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0.5f);
                    }
            }
        }
    }

    void AbleMove()
    {
        MoveAble = true;
    }

    void FireOff()
    {
        IsFire = false;
        FireIcon.SetActive(false);
    }
}
