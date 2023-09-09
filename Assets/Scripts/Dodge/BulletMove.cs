using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class BulletMove: MonoBehaviour
{
    Vector3 MyRotate = Vector3.forward;
    SpriteRenderer Mys;
    Color j = new Color(0, 0, 0, 0.02f);
    bool s = false;

    private void Awake()
    {
        Mys = GetComponent<SpriteRenderer>();
    }
    private void OnEnable()
    {
        if (Random.Range(0, 2) == 0) MyRotate = Vector3.back;
        else MyRotate = Vector3.forward;
    }
    private void FixedUpdate()
    {
        transform.Rotate(MyRotate * Time.deltaTime * 250);
        if (Mys.color.a <= 0.7 || Mys.color.a >= 1.0) s = s == false;

        if (s) Mys.color -= j;
        else Mys.color += j;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Border"))
        {
            gameObject.SetActive(false);
        }
    }
}
