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
        }
    }
}
