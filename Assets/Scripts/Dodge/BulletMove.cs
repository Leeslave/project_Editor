using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletMove: MonoBehaviour
{
    public float move_speed;
    Vector3 vec;
    private void Start()
    {
        vec = new Vector3(move_speed,0,0);
    }
    void Update()
    {
        transform.Translate(vec);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Border")
        {
            gameObject.SetActive(false);
        }
    }
}
