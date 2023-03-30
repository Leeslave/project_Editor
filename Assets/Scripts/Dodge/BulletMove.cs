using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class BulletMove: MonoBehaviour
{

    public void BigBullet(BulletManager BM)
    {
        StartCoroutine(BigBulletPattern(BM));
    }

    IEnumerator BigBulletPattern(BulletManager BM)
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            Vector2 DX = Vector2.zero;
            Vector2 DY = Vector2.up;
            for (int y = 0; y < 3; y++)
            {
                if (y == 1) DY = Vector2.zero;
                else if (y == 2) DY = Vector2.down;
                for (int x = 0; x < 3; x++)
                {
                    if (y == 1 && x == 1) continue;
                    if (x == 0) DX = Vector2.left;
                    else if (x == 1) DX = Vector2.zero;
                    else DX = Vector2.right;
                    BM.MakeSmallBul(DX * 8, DY * 8).transform.position = transform.position;
                }
            }
            
        }
    }


    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Border")
        {
            gameObject.SetActive(false);
        }
    }
}
