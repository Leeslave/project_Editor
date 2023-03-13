using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class Eye : MonoBehaviour
{
    private GameManager_VoightKampff GameManager;

    private enum AnimState
    {
        h0, h1, h2, h3, h4, h5, h6, h7, h8, h9, h10, h11, h12, h13, h14, h15,
        idle_h0, idle_h1, idle_h2, idle_h3, idle_h4, idle_h5, idle_h6, idle_h7,
        idle_h8, idle_h9, idle_h10, idle_h11, idle_h12, idle_h13, idle_h14, idle_h15,
        middle, middle_idle,
        Surprise, Shaking, 
    }                               
    private string currentAnimation;                        //현재 애니메이션 이름
    [Header("애니메이션 스켈레톤")]
    public SkeletonAnimation skeletonAnimation;
    [Header("애니메이션 에셋배열")]
    public AnimationReferenceAsset[] animClip;
    private Coroutine animationCoroutine;

    private void Start()
    {
        GameManager = GameObject.Find("GameManager").GetComponent<GameManager_VoightKampff>();
        SetCurrentAnimation(AnimState.middle_idle, true, 1.0f);
    }

    private void Update()
    {
        float HorizontalInput = Input.GetAxisRaw("Horizontal");
        float VerticalInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(KeyCode.Alpha4))
            RotatePupil(4, 12, true);
        if (Input.GetKeyDown(KeyCode.Alpha8))
            RotatePupil(8, 12, false);
        if (Input.GetKeyDown(KeyCode.C))
            DrawCSign();
        if (Input.GetKeyDown(KeyCode.S))
            DrawStar();
        if (Input.GetKeyDown(KeyCode.V))
            DrawVSign();
        if (Input.GetKeyDown(KeyCode.F))
            DrawFSign();
        if (Input.GetKeyDown(KeyCode.N))
            DrawNSign();
        if (Input.GetKeyDown(KeyCode.Z))
            DrawZSign();
        if (Input.GetKeyDown(KeyCode.Q))
            Surprise();
    }

    private bool IsEqualFloat(float a, float b)
    {
        return Mathf.Abs(a - b) < 0.05f;
    }

    public void RotatePupil(int rotateStartingPos, int rotateEndingPos, bool clockwise)//시작 각도에서 종료 각도까지 지정 방향으로 눈동자 회전
    {
        Debug.Log("Eye : RotatePupil " + rotateStartingPos + " to " + rotateEndingPos + " by clockwise" + clockwise);
        if (animationCoroutine != null)
            StopCoroutine(animationCoroutine);
        animationCoroutine = StartCoroutine(RotatePupil_IE(rotateStartingPos, rotateEndingPos, clockwise));
    }
    private IEnumerator RotatePupil_IE(int rotateStartingPos, int rotateEndingPos, bool clockwise)//시작 각도에서 종료 각도까지 지정 방향으로 눈동자 회전 IEnumerator
    {
        int dir = clockwise ? 1 : -1;
        int currentPos = 0;
        while(true)
        {
            AsyncAnimation(animClip[(16 + rotateStartingPos + currentPos) % 16], true, 1f);
            yield return new WaitForSeconds(0.105f);

            if (rotateEndingPos == (16 + rotateStartingPos + currentPos) % 16)
                break;

            currentPos += dir;
        }
        SetCurrentAnimation(AnimState.middle_idle, true, 1.0f);
    }

    public void Surprise()//놀람 애니메이션 재생
    {
        Debug.Log("Eye : Surprise");
        StartCoroutine(Surprise_IE());
    }
    private IEnumerator Surprise_IE()//놀람 애니메이션 재생 IEnumerator
    {
        SetCurrentAnimation(AnimState.Surprise, false, 1.0f);
        yield return new WaitForSeconds(4.0f);
        SetCurrentAnimation(AnimState.middle_idle, true, 1.0f);
    }

    public void DrawStar()//별을 그립니다
    {
        Debug.Log("Eye : DrawStar");
        if (animationCoroutine != null)
            StopCoroutine(animationCoroutine);
        animationCoroutine = StartCoroutine(DrawStar_IE());
    }
    private IEnumerator DrawStar_IE()//별을 그립니다 IEnumerator
    {
        SetCurrentAnimation(AnimState.h0, false, 1.0f);
        yield return new WaitForSeconds(0.2f);
        SetCurrentAnimation(AnimState.h10, false, 1.0f);
        yield return new WaitForSeconds(0.2f);
        SetCurrentAnimation(AnimState.h3, false, 1.0f);
        yield return new WaitForSeconds(0.2f);
        SetCurrentAnimation(AnimState.h13, false, 1.0f);
        yield return new WaitForSeconds(0.2f);
        SetCurrentAnimation(AnimState.h6, false, 1.0f);
        yield return new WaitForSeconds(0.2f);
        SetCurrentAnimation(AnimState.h0, false, 1.0f);
        yield return new WaitForSeconds(0.2f);
        SetCurrentAnimation(AnimState.middle_idle, true, 1.0f);
    }

    public void DrawCSign()//C사인을 그립니다
    {
        Debug.Log("Eye : DrawCSign");
        if (animationCoroutine != null)
            StopCoroutine(animationCoroutine);
        animationCoroutine = StartCoroutine(DrawCSign_IE());
    }
    private IEnumerator DrawCSign_IE()//C사인을 그립니다 IEnumerator
    {
        SetCurrentAnimation(AnimState.h2, false, 1.0f);
        yield return new WaitForSeconds(0.2f);
        SetCurrentAnimation(AnimState.h1, false, 1.0f);
        yield return new WaitForSeconds(0.105f);
        SetCurrentAnimation(AnimState.h0, false, 1.0f);
        yield return new WaitForSeconds(0.105f);
        SetCurrentAnimation(AnimState.h15, false, 1.0f);
        yield return new WaitForSeconds(0.105f);
        SetCurrentAnimation(AnimState.h14, false, 1.0f);
        yield return new WaitForSeconds(0.105f);
        SetCurrentAnimation(AnimState.h13, false, 1.0f);
        yield return new WaitForSeconds(0.105f);
        SetCurrentAnimation(AnimState.h12, false, 1.0f);
        yield return new WaitForSeconds(0.105f);
        SetCurrentAnimation(AnimState.h11, false, 1.0f);
        yield return new WaitForSeconds(0.105f);
        SetCurrentAnimation(AnimState.h10, false, 1.0f);
        yield return new WaitForSeconds(0.105f);
        SetCurrentAnimation(AnimState.h9, false, 1.0f);
        yield return new WaitForSeconds(0.105f);
        SetCurrentAnimation(AnimState.h8, false, 1.0f);
        yield return new WaitForSeconds(0.105f);
        SetCurrentAnimation(AnimState.h7, false, 1.0f);
        yield return new WaitForSeconds(0.105f);
        SetCurrentAnimation(AnimState.h6, false, 1.0f);
        yield return new WaitForSeconds(0.2f);
        SetCurrentAnimation(AnimState.middle_idle, true, 1.0f);
    }

    public void DrawVSign()//V사인을 그립니다
    {
        Debug.Log("Eye : DrawVSign");
        if (animationCoroutine != null)
            StopCoroutine(animationCoroutine);
        animationCoroutine = StartCoroutine(DrawVSign_IE());
    }
    private IEnumerator DrawVSign_IE()//V사인을 그립니다 IEnumerator
    {
        SetCurrentAnimation(AnimState.h14, false, 1.0f);
        yield return new WaitForSeconds(0.2f);
        SetCurrentAnimation(AnimState.h8, false, 1.0f);
        yield return new WaitForSeconds(0.2f);
        SetCurrentAnimation(AnimState.h2, false, 1.0f);
        yield return new WaitForSeconds(0.2f);
        SetCurrentAnimation(AnimState.middle_idle, true, 1.0f);
    }

    public void DrawFSign()//F사인을 그립니다
    {
        Debug.Log("Eye : DrawFSign");
        if (animationCoroutine != null)
            StopCoroutine(animationCoroutine);
        animationCoroutine = StartCoroutine(DrawFSign_IE());
    }
    private IEnumerator DrawFSign_IE()//F사인을 그립니다 IEnumerator
    {
        SetCurrentAnimation(AnimState.h2, false, 1.0f);
        yield return new WaitForSeconds(0.105f);
        SetCurrentAnimation(AnimState.h1, false, 1.0f);
        yield return new WaitForSeconds(0.105f);
        SetCurrentAnimation(AnimState.h0, false, 1.0f);
        yield return new WaitForSeconds(0.2f);
        SetCurrentAnimation(AnimState.h8, false, 1.0f);
        yield return new WaitForSeconds(0.105f);
        SetCurrentAnimation(AnimState.h9, false, 1.0f);
        yield return new WaitForSeconds(0.105f);
        SetCurrentAnimation(AnimState.h10, false, 1.0f);
        yield return new WaitForSeconds(0.2f);
        SetCurrentAnimation(AnimState.middle_idle, true, 1.0f);
    }

    public void DrawNSign()//N사인을 그립니다
    {
        Debug.Log("Eye : DrawNSign");
        if (animationCoroutine != null)
            StopCoroutine(animationCoroutine);
        animationCoroutine = StartCoroutine(DrawNSign_IE());
    }
    private IEnumerator DrawNSign_IE()//N사인을 그립니다 IEnumerator
    {
        SetCurrentAnimation(AnimState.h10, false, 1.0f);
        yield return new WaitForSeconds(0.2f);
        SetCurrentAnimation(AnimState.h14, false, 1.0f);
        yield return new WaitForSeconds(0.105f);
        SetCurrentAnimation(AnimState.h15, false, 1.0f);
        yield return new WaitForSeconds(0.105f);
        SetCurrentAnimation(AnimState.h0, false, 1.0f);
        yield return new WaitForSeconds(0.105f);
        SetCurrentAnimation(AnimState.h1, false, 1.0f);
        yield return new WaitForSeconds(0.105f);
        SetCurrentAnimation(AnimState.h2, false, 1.0f);
        yield return new WaitForSeconds(0.2f);
        SetCurrentAnimation(AnimState.h6, false, 1.0f);
        yield return new WaitForSeconds(0.2f);
        SetCurrentAnimation(AnimState.middle_idle, true, 1.0f);
    }

    public void DrawZSign()//Z사인을 그립니다
    {
        Debug.Log("Eye : DrawZSign");
        if (animationCoroutine != null)
            StopCoroutine(animationCoroutine);
        animationCoroutine = StartCoroutine(DrawZSign_IE());
    }
    private IEnumerator DrawZSign_IE()//Z사인을 그립니다
    {
        SetCurrentAnimation(AnimState.h14, true, 1.0f);
        yield return new WaitForSeconds(0.2f);
        SetCurrentAnimation(AnimState.h2, true, 1.0f);
        yield return new WaitForSeconds(0.2f);
        SetCurrentAnimation(AnimState.h10, true, 1.0f);
        yield return new WaitForSeconds(0.2f);
        SetCurrentAnimation(AnimState.h6, true, 1.0f);
        yield return new WaitForSeconds(0.2f);
        SetCurrentAnimation(AnimState.middle_idle, true, 1.0f);
    }

    private void SetCurrentAnimation(AnimState state, bool loop, float timeScale)//주어진 애니메이션 상태 상태에 따라 애니메이션 재생
    {
        AsyncAnimation(animClip[(int)state], loop, timeScale);
    }

    private void AsyncAnimation(AnimationReferenceAsset animClip, bool loop, float timeScale)//주어진 조건에 맞춰 애니메이션 동기화
    {
        if (animClip.name.Equals(currentAnimation))
            return;
        skeletonAnimation.state.SetAnimation(0, animClip, loop).TimeScale = timeScale;
        skeletonAnimation.loop = loop;
        skeletonAnimation.timeScale = timeScale;
        currentAnimation = animClip.name;
    }
}
