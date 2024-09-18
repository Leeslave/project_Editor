using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plat_DG : MonoBehaviour
{
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Border"))
        {
            gameObject.SetActive(false);
            Rigidbody2D cnt = GetComponent<Rigidbody2D>();
            if (cnt.velocity.x == 0) gameObject.SetActive(false);
        }
    }
}
