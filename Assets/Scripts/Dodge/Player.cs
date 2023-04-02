using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class Player : MonoBehaviour
{
    Rigidbody2D rigid;
    public GameManager GM;
    public GameObject Up;
    public GameObject Down;
    public int speed;
    public float MaxSpeed_Y;
    public bool IsGameOver;
    public bool MovePing;
    public bool MoveAble = true;
    bool RayAble = true;

    private void Awake()
    {
        IsGameOver = false;
        rigid = GetComponent<Rigidbody2D>();
        rigid.velocity = new Vector2(0, -speed);
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
        else
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
    }

    private void OnEnable()
    {
        gameObject.transform.position = new Vector2(3, -2);
        speed = 10;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Bullet")
        {
            gameObject.SetActive(false);
        }
    }
}
