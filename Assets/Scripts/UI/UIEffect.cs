using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIEffect : MonoBehaviour
{
    [SerializeField] private float[] initial; // gameObject의 첫 scale 값
    [SerializeField] private float[] target;  // gameObject의 활성화 된 scale 값
    private Vector3 initialScale;   // Lerp함수 인자용 첫 scale의 vector값
    private Vector3 targetScale;    // Lerp함수 인자용 활성화 된 scale의 vector값
    private float duration = 1f;    // 활성화 되는 시간

    void Start()
    {
        // gameObject의 초기화
        transform.localScale = Vector3.zero;
        initialScale = new Vector3(initial[0], initial[1], initial[2]);
        targetScale = new Vector3(target[0], target[1], target[2]);
    }

    public void ButtonClick() // Debug용 UI버튼 클릭시 활성화 되는 함수
    {
        StartCoroutine(ScaleUp());
    }

    /// <summary>
    /// gameObject의 scale값 변경
    /// 이후, manager를 통해서 순서대로 활성화,
    /// 버튼 클릭 시 다음 설명으로 이동 코드 추가
    /// </summary>
    /// <returns></returns>
    IEnumerator ScaleUp()
    {
        float currentTime = 0f;
        while (currentTime < duration) // 처음에 설정한 시간동안 실행
        {
            currentTime += Time.deltaTime;
            float t = currentTime / duration; // t는 보간을 의미하는 변수
            transform.localScale = Vector3.Lerp(initialScale, targetScale, t); // 우리가 설정한 값으로 서서히 사이즈 변경
            yield return null;
        }

        transform.localScale = targetScale; // 최종적으로는 우리가 설정한 값으로 변경
    }
}
