using System;
using UnityEngine;
using TMPro;
using UnityEngine.Scripting;
using System.Collections.Generic;
using System.Collections;

public class PlayerMove : MonoBehaviour
{
    [SerializeField]
    public MakeTile MT;
    public float Move_X;
    public float Move_Y;
    public bool MoveAble;
    public float InputDelay;
    public int Sight;
    public GameObject maincam;
    public GameObject Clear;

    List<GameObject> KeyTrain = new List<GameObject>();
    Vector3 LastTrans;

    RaycastHit2D rayHit;
    Rigidbody2D rigid;
    float Bf_X;
    float Bf_Y;
    float NX;
    float NY;
    bool IsEnd = false;
    bool MS = true;
    Vector3 Dir;

    void Awake()
    {
        //Size_P = GetComponent<SpriteRenderer>().sprite.bounds.size.x;
        //Size_T = 0.04f;
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
    }

    void Update()
    {
        if (MoveAble && MS)
        {
            NX = 0;
            NY = 0;
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
            if (NX != 0 || NY != 0)   // 움직임을 감지
            {
                bool IsMoveNext = true;
                rayHit = Physics2D.Raycast(transform.position, Dir, 10, LayerMask.GetMask("Plat"));

                if (rayHit.collider != null)
                {
                    if(rayHit.collider.tag == "Wall")
                    {
                        IsMoveNext = false;
                    }
                    else if(rayHit.collider.tag == "ExitWall")
                    {
                        if (!IsEnd)
                        {
                            if (KeyTrain.Count - 1 == MT.KeyNum) { StartCoroutine(ClearGame()); }
                            IsMoveNext = false;
                        }
                        else
                        {
                            Clear.SetActive(true);
                            Destroy(gameObject);
                        }
                    }
                }
                rayHit = Physics2D.Raycast(transform.position, Dir, 10, LayerMask.GetMask("Default"));
                if(rayHit.collider != null && IsMoveNext) if (rayHit.collider.tag == "Key")
                    {
                        KeyTrain.Add(rayHit.collider.gameObject);
                        rayHit.collider.tag = "Untagged";
                        rayHit.collider.gameObject.layer = 6;
                        rayHit.collider.gameObject.transform.position = LastTrans;
                    }


                if (IsMoveNext)
                {
                    LastTrans = KeyTrain[KeyTrain.Count-1].transform.position;
                    for(int i = KeyTrain.Count - 1; i > 0; i--) KeyTrain[i].transform.position = KeyTrain[i - 1].transform.position;
                    Bf_X = transform.position.x;
                    Bf_Y = transform.position.y;
                    transform.Translate(new Vector3(NX, NY, 0));
                    maincam.transform.position = new Vector3(transform.position.x, transform.position.y, -10);
                    CalcFog();
                }
                MoveAble = false;
                Invoke("AbleMove", InputDelay);
            }
        }
    }

    IEnumerator ClearGame()
    {
        MS = false;
        for (int i = KeyTrain.Count - 1; i > 0; i--)
        {
            for (int x = 0; x < 50; x++)
            {
                KeyTrain[i].transform.Translate(0,0.1f,0);
                yield return new WaitForSeconds(0.01f);
            }
            yield return new WaitForSeconds(1);
            Vector3 cnt = (MT.Clear.transform.position - KeyTrain[i].transform.position).normalized;
            while (Vector3.Magnitude(MT.Clear.transform.position - KeyTrain[i].transform.position) > 1)
            {
                KeyTrain[i].transform.Translate(cnt/2);
                yield return new WaitForSeconds(0.01f);
            }
            yield return new WaitForSeconds(0.25f);
            KeyTrain[i].SetActive(false);
            yield return new WaitForSeconds(1);
        }
        MT.Clear.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0,0);
        MS = true; IsEnd = true;
    }

    void CalcFog()
    {
        int CurY = (int)((transform.position.y - 5) / Move_X);
        int CurX = (int)((transform.position.x - 5) / Move_X);

        for (int y = CurY - Sight + 1; y <= CurY + Sight - 1; y++)
        {
            if (y >= MT.Col || y < 0) continue;
            for(int x = CurX - Sight + 1; x <= CurX + Sight - 1; x++)
            {
                if (x >= MT.Row || x < 0) continue;
                for (int a = 0; a < 2; a++) for (int b = 0; b < 2; b++)
                    {
                        int dx = x * 2 + b;
                        int dy = y * 2 + a;
                        Vector3 RayVec = new Vector3(x * Move_X + 5 - 2.5f + 5 * b, y * Move_Y + 5 - 2.5f + 5 * a, 0) - transform.position;
                        float S = Vector3.Magnitude(RayVec) / Move_X;
                        if (S > Sight) continue;
                        rayHit = Physics2D.Raycast(transform.position, RayVec, S * Move_X, LayerMask.GetMask("Plat"));

                        
                        if (rayHit.collider == null)
                        {
                            MT.Fogs[dy][dx].GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0);
                            MT.IsFog[dy][dx] = true;
                        }
                        else
                        {
                            if (MT.IsFog[dy][dx] == true) MT.Fogs[dy][dx].GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0.5f);
                        }
                    }
            }
        }
        // 추가로 이동 방향 반대의 거리 + 1의 안개에 대해 이미 밝혀진 길이면 연하게 표시.
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
}
