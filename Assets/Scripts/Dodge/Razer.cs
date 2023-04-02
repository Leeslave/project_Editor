using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Razer : MonoBehaviour
{
    /*
        size : (3.8, 0.1 ~ 1.4<- 가변)

        0.5 + 0.65 1.15 4.85
        pos : down (2.6,-4)  up (2.6, 1.4)
    */
    private void Start()
    {
        StartCoroutine(BigRazer());
    }

    IEnumerator BigRazer()  // 9.5초 지속
    {
        yield return new WaitForSeconds(2f);
        gameObject.tag = "Bullet";
        gameObject.layer = 7;
        for (; transform.localScale.y < 1.4f; transform.localScale = new Vector2(transform.localScale.x, transform.localScale.y + 0.1f))
            yield return new WaitForSeconds(0.01f);
        yield return new WaitForSeconds(6.22f);
        for (; transform.localScale.y >= 0.1f; transform.localScale = new Vector2(transform.localScale.x, transform.localScale.y - 0.1f))
            yield return new WaitForSeconds(0.01f);
        Destroy(gameObject);
    }
}
