using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class VK_ManagerScript : MonoBehaviour
{
    private GameObject EyeViewer { get; set; }
    private GameObject PupilBone { get; set; }
    private Vector2 PupilOriginPos { get; set; } = new Vector2(1.746719f, 0.5157298f);
    private Coroutine PupilSequenceCoroutine { get; set; }

    private enum TurnStatus
    {
        비활성, 활성
    }
    private TurnStatus CurrentTurnStatus { get; set; } = TurnStatus.비활성;
    
    private SpriteRenderer ArrowDisplay { get; set; }
    private GameObject Arrow { get; set; }
    private Queue<GameObject> ArrowQueue { get; set; } = new Queue<GameObject>();
    
    private SpriteRenderer TimerSlider { get; set; }
    
    private void Awake()
    {
        EyeViewer = GameObject.Find("EyeViewer");
        PupilBone = GameObject.Find("bone_10");
        ArrowDisplay = GameObject.Find("ArrowDisplay").GetComponent<SpriteRenderer>();
        Arrow = Resources.Load<GameObject>("VoightKampffDev/Arrow");
        TimerSlider = GameObject.Find("TimerSlider").GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        ClickKeyCodeArrow();
    }

    #region 턴 관련 함수
    
    public void StartTurn(float wait, float duration, int num)
    {
        StartCoroutine(StartSequence(wait, duration, num));        
    }
    private IEnumerator StartSequence(float wait, float duration, int num)
    {
        LJWConverter.Instance.GradientSpriteRendererColor(false, 0f, wait, new Color(0f, 0f, 0f, 0.8f), ArrowDisplay);
        
        //타이머 슬라이더 초기화
        ResetTimerSlider(wait);
        
        yield return new WaitForSeconds(wait);
        
        //타이머 슬라이더 최소화
        MinimizeTimerSlider(duration);
        
        Debug.Log("새로운 턴이 시작되었습니다!");
        
        //랜덤한 개수의 화살표를 생성한다
        SpawnArrows(Random.Range(4, 8));
        
        //활성 상태가 아니면 화살표를 입력할 수 없다
        CurrentTurnStatus = TurnStatus.활성;
        
        //시퀀스마다 난수를 생성하여 눈동자가 다른 방향으로 움직이게 한다
        for (var i = 0; i < num; i++)
        {
            var x = Random.Range(0, 2) == 0 ? Random.Range(-7, -3) : Random.Range(4, 8);
            var y = Random.Range(0, 2) == 0 ? Random.Range(-7, -3) : Random.Range(4, 8);
            var random = new Vector2(x, y);
            yield return PupilSequenceCoroutine = StartCoroutine(MovePupil2Random(duration / num, Time.time, random));
        }
        
        //모든 시퀀스를 소화했으므로 턴을 종료한다
        yield return StartCoroutine(EndTurn(3f));
        
        Debug.Log("타이머가 다 되어 턴이 종료되었습니다!");
    }
    private IEnumerator EndTurn(float duration)
    {        
        CurrentTurnStatus = TurnStatus.비활성;
        
        //타이머 슬라이더 최소화
        MinimizeTimerSlider(duration);
        
        //활성화되어있던 시퀀스를 종료한다
        if(ReferenceEquals(PupilSequenceCoroutine, null) == false)
            StopCoroutine(PupilSequenceCoroutine);
        
        //남아있는 화살표를 전부 파괴
        var count = ArrowQueue.Count;
        for (var i = 0; i < count; i++)
        {
            Destroy(ArrowQueue.Dequeue());
        }
        
        //눈동자 원위치
        LJWConverter.Instance.GradientSpriteRendererColor(false, 0f, duration, new Color(0f, 0f, 0f, 0f), ArrowDisplay);
        LJWConverter.Instance.PositionTransform(false, 0f, duration, PupilOriginPos, PupilBone.transform);
        
        yield return new WaitForSeconds(duration);
    }
    private IEnumerator MovePupil2Random(float duration, float startTime, Vector2 random)
    {
        Debug.Log($"{random} 방향으로 이동!");
        while (Time.time - startTime < duration)
        {
            var x = Input.GetAxis("Horizontal") * 10f;
            var y = Input.GetAxis("Vertical") * 10f;
            var input = new Vector2(x, y);
            var result = random + input;
            PupilBone.transform.Translate(result * Time.deltaTime);
            yield return new WaitForSeconds(0.02f);
        }
    }

    #endregion

    #region 눈동자 엑시트 이벤트
    
    public void OnPupilExit()
    {
        StartCoroutine(OnPupilExit_IE());
    }
    private IEnumerator OnPupilExit_IE()
    {
        yield return StartCoroutine(EndTurn(3f));
        Debug.Log("눈동자를 제어하지 못해서 턴이 종료되었습니다!");
    }
    
    #endregion

    #region 화살표 입력 함수

    private void ClickKeyCodeArrow()
    {
        if (CurrentTurnStatus == TurnStatus.비활성)
            return;
        
        if (Input.GetKeyDown(KeyCode.UpArrow))
            CheckInputResult("UpArrow");
        else if (Input.GetKeyDown(KeyCode.RightArrow))
            CheckInputResult("RightArrow");
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            CheckInputResult("DownArrow");
        else if(Input.GetKeyDown(KeyCode.LeftArrow))
            CheckInputResult("LeftArrow");
    }
    private void CheckInputResult(string keyCode)
    {
        if (ArrowQueue.Peek().name == keyCode)
        {
            Debug.Log("정확한 키 입력!");
            Destroy(ArrowQueue.Dequeue());
        }
        else
        {
            Debug.Log("부정확한 키 입력!");
        }

        if (ArrowQueue.Count == 0)
        {
            StartCoroutine(EndTurn(3f));
            Debug.Log("모든 화살표를 소화하였으므로 턴을 종료합니다!");
        }
    }
    private void SpawnArrows(int num)
    {
        Debug.Log($"총 {num}개의 화살표를 생성했습니다!");
        var startPos = num % 2 == 0 ? new Vector3(-1.5f * num / 2 + 0.75f, -3f, 0f) :  new Vector3(-1.5f * (num - 1) / 2, -3f, 0f);
        for (var i = 0; i < num; i++)
        {
            var random = Random.Range(0, 4);
            var arrow = Instantiate(Arrow, startPos, Quaternion.Euler(new Vector3(0f, 0f, -90f * random)));
            startPos += new Vector3(1.5f, 0f, 0f);
            arrow.name = random switch
            {
                0 => "UpArrow",
                1 => "RightArrow",
                2 => "DownArrow",
                3 => "LeftArrow",
                _ => arrow.name
            };
            ArrowQueue.Enqueue(arrow);
        }
    }

    #endregion

    #region 타이머 관련 함수

    private void ResetTimerSlider(float wait)
    {
        LJWConverter.Instance.GradientSpriteRendererColor(false, 0f, wait, new Color(0.17f, 1f, 0.17f, 1f), TimerSlider);
        LJWConverter.Instance.ConvertSpriteRendererSize(false, 0f, wait, new Vector2(11f, 1.6f), TimerSlider);
    }

    private void MinimizeTimerSlider(float duration)
    {
        LJWConverter.Instance.GradientSpriteRendererColor(false, 0f, duration, new Color(1f, 0.17f, 0.17f, 1f), TimerSlider);
        LJWConverter.Instance.ConvertSpriteRendererSize(false, 0f, duration, new Vector2(0f, 1.6f), TimerSlider);
    }

    #endregion
    
}
