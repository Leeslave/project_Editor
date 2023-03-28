using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BulletMove: MonoBehaviour
{
    public float move_speed;
    Vector3 vec;
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Border")
        {

            gameObject.SetActive(false);
        }
    }
}
