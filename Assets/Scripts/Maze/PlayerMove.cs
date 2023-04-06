using System;
using UnityEngine;
using TMPro;
using UnityEngine.Scripting;
using System.Collections.Generic;
using System.Collections;

public class PlayerMove : MonoBehaviour
{
    [SerializeField]
    public GameObject Trace;
    public MakeTile MT;
    public float Move_X;
    public float Move_Y;
    public bool MoveAble;
    public float InputDelay;
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
                    else if(rayHit.collider.tag == "Key")
                    {
                        KeyTrain.Add(rayHit.collider.gameObject);
                        rayHit.collider.tag = "Untagged";
                        rayHit.collider.gameObject.layer = 0;
                        rayHit.collider.gameObject.transform.position = LastTrans;
                    }
                }
                if (IsMoveNext)
                {
                    LastTrans = KeyTrain[KeyTrain.Count-1].transform.position;
                    for(int i = KeyTrain.Count - 1; i > 0; i--) KeyTrain[i].transform.position = KeyTrain[i - 1].transform.position;
                    Bf_X = transform.position.x;
                    Bf_Y = transform.position.y;
                    transform.Translate(new Vector3(NX, NY, 0));
                    maincam.transform.position = new Vector3(transform.position.x, transform.position.y, -10);
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

    void AbleMove()
    {
        MoveAble = true;
    }
}
