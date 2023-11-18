using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Razer : MonoBehaviour
{
    /*
        size : (3.8, 0.1 ~ 1.4<- 가변)
        pos : down (2.6,-4)  up (2.6, 1.4)
    */
    private void Start()
    {
        StartCoroutine(BigRazer());
    }

    // 레이저의 느낌을 살리기 위한 연출
    IEnumerator BigRazer()  // 9.5초 지속
    {
        // 레이저가 생성 된 지 첫 2초간은 닿아도 GameOver되지 않으며 충돌하지도 않는 상태
        yield return new WaitForSeconds(2f);
        gameObject.tag = "Bullet";
        gameObject.layer = 7;

        // 1.4초동안 레이저의 크기를 키움
        for (; transform.localScale.y < 1.4f; transform.localScale = new Vector2(transform.localScale.x, transform.localScale.y + 0.1f))
            yield return new WaitForSeconds(0.01f);
        yield return new WaitForSeconds(5.72f);
        // 1.4초동안 레이저의 크기를 줄임
        for (; transform.localScale.y >= 0.1f; transform.localScale = new Vector2(transform.localScale.x, transform.localScale.y - 0.1f))
            yield return new WaitForSeconds(0.01f);

        Destroy(gameObject);
    }
}
