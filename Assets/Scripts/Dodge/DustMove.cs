using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class DustMove: MonoBehaviour
{
    Rigidbody2D Rigid;
    float s;
    Vector2 R = Vector2.right;
    Vector2 L = Vector2.left;
    private void Awake()
    {
        Rigid = GetComponent<Rigidbody2D>();
        s = Random.Range(0.1f, 1f);
        R *= s; L *= s;
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Border"))
        {

            transform.position = new Vector2(-transform.position.x * 0.9f,-transform.position.y);
            if (transform.position.x < 0) Rigid.AddForce(R,ForceMode2D.Impulse);
            else Rigid.AddForce(L,ForceMode2D.Impulse);
        }
    }
}
