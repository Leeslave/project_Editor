using System;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [SerializeField]
    public GameObject Trace;
    public float Move_X;
    public float Move_Y;
    public bool MoveAble;
    public float InputDelay;
    public GameObject maincam;
    RaycastHit2D rayHit;
    Rigidbody2D rigid;
    float Bf_X;
    float Bf_Y;
    float NX;
    float NY;
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
    }

    void Update()
    {
        Debug.DrawRay(transform.position, Dir, new Color(1, 1, 0));
        if (MoveAble)
        {
            NX = 0;
            NY = 0;
            if (Input.GetButton("Horizontal"))
            {
                NX = Input.GetAxisRaw("Horizontal");
                Dir = NX < 0 ? Vector3.left : Vector3.right;
            }
            else if (Input.GetButton("Vertical"))
            {
                NY = Input.GetAxisRaw("Vertical");
                Dir = NY < 0 ? Vector3.down : Vector3.up;
            }
            if (NX != 0 || NY != 0)   // 움직임을 감지
            {
                bool IsMoveNext = true;
                rayHit = Physics2D.Raycast(transform.position, Dir, 1, LayerMask.GetMask("Plat"));

                if (rayHit.collider != null)
                {
                    if(rayHit.collider.tag == "Wall")
                    {
                        IsMoveNext = false;
                    }
                }
                if (IsMoveNext)
                {
                    Bf_X = transform.position.x;
                    Bf_Y = transform.position.y;
                    transform.Translate(new Vector3(NX, NY, 0));
                }
                MoveAble = false;
                Invoke("AbleMove", InputDelay);
            }

        }
    }

    void AbleMove()
    {
        MoveAble = true;
    }
}
