using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletMove: MonoBehaviour
{
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Border")
        {
            gameObject.SetActive(false);
        }
    }
}
