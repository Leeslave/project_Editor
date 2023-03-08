using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class Player : MonoBehaviour
{
    Rigidbody2D rigid;
    public GameObject Up;
    public GameObject Down;
    public float MaxSpeed_Y;
    public float MaxSpeed_X;
    public bool IsGameOver;
    bool UpAble = true;
    bool DownAble = true;

    private void Awake()
    {
        IsGameOver = false;
        rigid = GetComponent<Rigidbody2D>();
    }
    private void FixedUpdate()
    {
        float h = Input.GetAxisRaw("Horizontal");
        rigid.AddForce(Vector2.right * h, ForceMode2D.Impulse);
        if (rigid.velocity.x > MaxSpeed_X) rigid.velocity = new Vector2(MaxSpeed_X, rigid.velocity.y);
        else if (rigid.velocity.x < MaxSpeed_X * (-1)) rigid.velocity = new Vector2(MaxSpeed_X * (-1), rigid.velocity.y);
        if (rigid.velocity.y > MaxSpeed_Y) rigid.velocity = new Vector2(rigid.velocity.x, MaxSpeed_Y);
        else if (rigid.velocity.y < MaxSpeed_Y * (-1)) rigid.velocity = new Vector2(rigid.velocity.x, MaxSpeed_Y * (-1));
        RaycastHit2D RayHit_Down = Physics2D.Raycast(rigid.position, Vector3.down, 0.35f,LayerMask.GetMask("Plat"));
        RaycastHit2D RayHit_Up = Physics2D.Raycast(rigid.position, Vector3.up, 0.35f, LayerMask.GetMask("Plat"));
        if (RayHit_Down.collider != null)
        {
            if (RayHit_Down.collider.name == "Plat_Down" && DownAble)
            {
                rigid.velocity = new Vector2(rigid.velocity.x, rigid.velocity.y * (-0.5f));
                DownAble = false;
                UpAble = true;
                rigid.gravityScale *= -1;
                Down.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.4f);
                Invoke("ChangeColor_Down", 0.5f);
            }
        }
        if (RayHit_Up.collider != null)
        {
            if (RayHit_Up.collider.name == "Plat_Up" && UpAble)
            {
                rigid.velocity = new Vector2(rigid.velocity.x, rigid.velocity.y * (-0.5f));
                UpAble = false;
                DownAble = true;
                rigid.gravityScale *= -1;
                Up.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.4f);
                Invoke("ChangeColor_Up", 0.5f);
            }
        }
    }
    void ChangeColor_Up()
    {
        Up.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
    }
    void ChangeColor_Down()
    {
        Down.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
    }

    public void make_Player()
    {
        gameObject.SetActive(true);
        gameObject.transform.position = new Vector2(3,-2);
    }
    public void End_Player()
    {
        gameObject.SetActive(false);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Bullet")
        {
            gameObject.SetActive(false);
            IsGameOver = true;
        }
    }
}
