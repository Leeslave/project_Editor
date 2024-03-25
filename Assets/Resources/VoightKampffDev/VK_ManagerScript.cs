using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class VK_ManagerScript : MonoBehaviour
{
    private GameObject EyeViewer { get; set; }
    private GameObject PupilBone { get; set; }
    private Vector2 PupilOriginPos { get; set; } = new Vector2(1.7f, 0f);
    private Coroutine PupilSequenceCoroutine { get; set; }

    private enum TurnStatus
    {
        비활성, 활성
    }
    private TurnStatus CurrentTurnStatus { get; set; } = TurnStatus.비활성;
    
    private GameObject ArrowSpawnPos { get; set; }
    private GameObject Arrow { get; set; }
    private Queue<GameObject> ArrowQueue { get; set; } = new();
    
    private TextMeshPro TurnTimer { get; set; }


    [SerializeField] private AnimationCurve curve;
    
    
    
    private GameObject AnswerScreen { get; set; }
    
    
    private void Awake()
    {

        PupilBone = GameObject.Find("bone_5");
        ArrowSpawnPos = GameObject.Find("ArrowSpawnPos");
        Arrow = Resources.Load<GameObject>("VoightKampffDev/Arrow");
        TurnTimer = GameObject.Find("TurnTimer").GetComponent<TextMeshPro>();
        
        EyeViewer = GameObject.Find("EyeViewer");        
        AnswerScreen = GameObject.Find("AnswerScreen");
    }

    private void Update()
    {
        ClickKeyCodeArrow();
    }

    #region 턴 관련 함수
    
    public void StartTurn(float wait, float duration, int num, int arrowNum)
    {
        StartCoroutine(StartSequence(wait, duration, num, arrowNum));        
    }
    
    private IEnumerator StartSequence(float wait, float duration, int num, int arrowNum)
    {
        //랜덤한 개수의 화살표를 생성한다
        SpawnArrows(arrowNum);
        
        //활성 상태가 아니면 화살표를 입력할 수 없다
        CurrentTurnStatus = TurnStatus.활성;
        
        //답지가 출력되는 스크린을 상승시킨다
        LJWConverter.Instance.PositionTransform(false, wait / 2, wait / 2, new Vector3(3.4f, -7f, 0f), AnswerScreen.transform, curve);
        
        //눈동자가 출력되는 뷰어를 상승시킨다
        LJWConverter.Instance.PositionTransform(false, 0f, wait / 2, new Vector3(0.2f, -5.4f, 0f), EyeViewer.transform, curve);
        
        //타이머를 시작시킨다
        LJWConverter.Instance.SetIntTimerTMP(false, wait, duration, TurnTimer);

        yield return new WaitForSeconds(wait);
        
        Debug.Log("새로운 턴이 시작되었습니다!");
        
        //시퀀스마다 난수를 생성하여 눈동자가 다른 방향으로 움직이게 한다
        for (var i = 0; i < num; i++)
        {
            var x = Random.Range(0, 2) == 0 ? Random.Range(-3f, -1f) : Random.Range(1f, 3f);
            var y = Random.Range(0, 2) == 0 ? Random.Range(-3f, -1f) : Random.Range(1f, 3f);
            var random = new Vector2(x, y);
            yield return PupilSequenceCoroutine = StartCoroutine(MovePupil2Random(duration / num, Time.time, random));
        }
        
        //모든 시퀀스를 소화했으므로 턴을 종료한다
        yield return StartCoroutine(EndTurn());
        
        Debug.Log("타이머가 다 되어 턴이 종료되었습니다!");
    }
    
    private IEnumerator EndTurn()
    {
        const float duration = 3f;
        
        //답지가 출력되는 스크린을 하강시킨다
        LJWConverter.Instance.PositionTransform(false, 0f, duration / 2, new Vector3(3.4f, -11f, 0f), AnswerScreen.transform, curve);
        
        //눈동자가 출력되는 뷰어를 하강시킨다
        LJWConverter.Instance.PositionTransform(false, duration / 2, duration / 2, new Vector3(0.2f, -14.5f, 0f), EyeViewer.transform, curve);
        
        //타이머를 종료시킨다
        LJWConverter.Instance.EndIntTimerTMP(false, 0f, duration, TurnTimer);
        
        //활성 상태가 아니면 화살표를 입력할 수 없다
        CurrentTurnStatus = TurnStatus.비활성;
        
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
        LJWConverter.Instance.PositionTransform(false, 0f, duration, PupilOriginPos, PupilBone.transform, curve);
        
        yield return new WaitForSeconds(duration);
    }
    
    private IEnumerator MovePupil2Random(float duration, float startTime, Vector2 random)
    {
        Debug.Log($"{random} 방향으로 이동!");
        while (Time.time - startTime < duration)
        {
            var x = Input.GetAxis("Horizontal") * 3.5f;
            var y = Input.GetAxis("Vertical") * 3.5f;
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
        yield return StartCoroutine(EndTurn());
        Debug.Log("눈동자를 제어하지 못해서 턴이 종료되었습니다!");
    }
    
    #endregion

    #region 화살표 관련 함수

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
            StartCoroutine(EndTurn());
        }

        if (ArrowQueue.Count == 0)
        {
            Debug.Log("모든 화살표를 소화하였으므로 턴을 종료합니다!");
            StartCoroutine(EndTurn());
        }
    }
    private void SpawnArrows(int num)
    {
        Debug.Log($"총 {num}개의 화살표를 생성했습니다!");
        var startPos = num % 2 == 0 ? new Vector3(-1f * num / 2 + 0.5f, 0f, 0f) :  new Vector3(-1f * (num - 1) / 2, 0f, 0f);
        for (var i = 0; i < num; i++)
        {
            var random = Random.Range(0, 4);
            var arrow = Instantiate(Arrow, ArrowSpawnPos.transform);
            arrow.transform.localPosition = startPos;
            arrow.transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, -90f * random));
            startPos += new Vector3(1f,0f, 0f);
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
    
}
