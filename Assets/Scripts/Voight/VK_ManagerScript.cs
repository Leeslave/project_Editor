using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class VK_ManagerScript : MonoBehaviour
{
    public VoightTutorialManager TutorialManager { get; set; }

    public Coroutine Turn { get; set; }
    
    public GameObject EyeDisplay { get; set; }
    private TMP_Text EyeDisplayTimer { get; set; }
    private Rigidbody2D PupilBone { get; set; }
    private Vector2 PupilOriginPos { get; set; } = new(1.7f, 0f);
    [SerializeField] private float pupilSpeed;
    
    private GameObject ArrowSpawnPos { get; set; }
    private GameObject Arrow { get; set; }
    private int ArrowNum { get; set; }
    private Queue<GameObject> ArrowQueue { get; set; } = new();
    
    public GameObject AnswerScreen { get; set; }
    private TMP_Text AnswerScreenTimer { get; set; }
    
    private SpriteRenderer WalterBlur { get; set; }

    private enum TurnStatus
    {
        비활성, 활성
    }
    private TurnStatus CurrentTurnStatus { get; set; } = TurnStatus.비활성;
    
    [SerializeField] public AnimationCurve curve;
    [SerializeField] private bool startTutorial;
    
    private void Awake()
    {
        TutorialManager = FindObjectOfType<VoightTutorialManager>();

        PupilBone = GameObject.Find("bone_5").GetComponent<Rigidbody2D>();
        ArrowSpawnPos = GameObject.Find("ArrowSpawnPos");
        Arrow = Resources.Load<GameObject>("Voight/Arrow");
        EyeDisplay = GameObject.Find("EyeDisplay");        
        EyeDisplayTimer = GameObject.Find("EyeDisplayTimer").GetComponent<TMP_Text>();
        AnswerScreen = GameObject.Find("AnswerScreen");
        AnswerScreenTimer = GameObject.Find("AnswerScreenTimer").GetComponent<TMP_Text>();
        WalterBlur = GameObject.Find("WalterBlur").GetComponent<SpriteRenderer>();
    }
    private void Start()
    {
        if(startTutorial)
            TutorialManager.StartVoightTutorial();
    }
    private void Update()
    {
        ClickKeyCodeArrow();
    }

    #region 턴 관련 함수
    
    public void StartComplexTurn(float wait, float duration, int evade, int arrow) => Turn = StartCoroutine(ComplexSequence(wait, duration, evade, arrow));
    private IEnumerator ComplexSequence(float wait, float duration, int evade, int arrow)
    {
        //화살표 생성
        SpawnArrows(arrow);
        
        //활성 상태가 아니면 화살표를 입력할 수 없다
        CurrentTurnStatus = TurnStatus.활성;
        
        //월터 블러를 활성화
        LJWConverter.Instance.GradientSpriteRendererColor(false, 0f, wait, new Color(1f, 1f, 1f, 1f), WalterBlur);
        
        //답지가 출력되는 스크린을 상승시킨다
        LJWConverter.Instance.PositionTransform(false, wait / 2, wait / 2, new Vector3(3.4f, -7f, 0f), AnswerScreen.transform, curve);
        //타이머 시작
        LJWConverter.Instance.SetIntTimerTMP(false, wait, duration, AnswerScreenTimer);
        
        //눈동자가 출력되는 뷰어를 상승시킨다
        LJWConverter.Instance.PositionTransform(false, 0f, wait / 2, new Vector3(0.2f, -5.4f, 0f), EyeDisplay.transform, curve);
        //타이머를 시작
        LJWConverter.Instance.SetIntTimerTMP(false, wait, duration, EyeDisplayTimer);

        yield return new WaitForSeconds(wait);
        
        Debug.Log("새로운 턴이 시작되었습니다!");

        //시퀀스마다 난수를 생성하여 눈동자가 다른 방향으로 움직이게 한다
        for (var i = 0; i < evade; i++)
        {
            var x = Random.Range(0, 2) == 0 ? Random.Range(-2f, -1f) : Random.Range(1f, 2f);
            var y = Random.Range(0, 2) == 0 ? Random.Range(-2f, -1f) : Random.Range(1f, 2f);
            var random = new Vector2(x, y);
            var startTime = Time.time;
            {
                while (Time.time - startTime < duration / evade)
                {
                    var input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")) * pupilSpeed;
                    PupilBone.AddForce(random);
                    PupilBone.AddForce(input);
                    yield return new WaitForSeconds(0.02f);
                }
            }
        }
        
        //모든 시퀀스를 소화했으므로 턴을 종료한다
        yield return StartCoroutine(EndTurn());
    }

    public void StartEyeTurn(float wait, float duration, int evade) => Turn = StartCoroutine(EyeSequence(wait, duration, evade));
    private IEnumerator EyeSequence(float wait, float duration, int evade)
    {
        //월터 블러를 활성화
        LJWConverter.Instance.GradientSpriteRendererColor(false, 0f, wait, new Color(1f, 1f, 1f, 1f), WalterBlur);
        
        //눈동자가 출력되는 뷰어를 상승시킨다
        LJWConverter.Instance.PositionTransform(false, 0f, wait, new Vector3(0.2f, -5.4f, 0f), EyeDisplay.transform, curve);
        //타이머를 시작
        LJWConverter.Instance.SetIntTimerTMP(false, wait, duration, EyeDisplayTimer);

        yield return new WaitForSeconds(wait);
        
        Debug.Log("새로운 턴이 시작되었습니다!");
        
        //시퀀스마다 난수를 생성하여 눈동자가 다른 방향으로 움직이게 한다
        for (var i = 0; i < evade; i++)
        {
            var x = Random.Range(0, 2) == 0 ? Random.Range(-2f, -1f) : Random.Range(1f, 2f);
            var y = Random.Range(0, 2) == 0 ? Random.Range(-2f, -1f) : Random.Range(1f, 2f);
            var random = new Vector2(x, y);
            var startTime = Time.time;
            {
                while (Time.time - startTime < duration / evade)
                {
                    var input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")) * pupilSpeed;
                    PupilBone.AddForce(random);
                    PupilBone.AddForce(input);
                    yield return new WaitForSeconds(0.02f);
                }
            }
        }
        
        //모든 시퀀스를 소화했으므로 턴을 종료한다
        yield return StartCoroutine(EndTurn());
    }

    public void StartArrowTurn(float wait, float duration, int arrowNum) => Turn = StartCoroutine(ArrowSequence(wait, duration, arrowNum));
    private IEnumerator ArrowSequence(float wait, float duration, int arrowNum)
    {
        //이번 턴 화살표의 개수
        ArrowNum = arrowNum;
        
        //월터 블러를 활성화
        LJWConverter.Instance.GradientSpriteRendererColor(false, 0f, wait, new Color(1f, 1f, 1f, 1f), WalterBlur);
        
        //답지가 출력되는 스크린을 상승시킨다
        LJWConverter.Instance.PositionTransform(false, 0f, wait, new Vector3(3.4f, -7f, 0f), AnswerScreen.transform, curve);
        
        //타이머 시작
        LJWConverter.Instance.SetIntTimerTMP(false, wait, duration, AnswerScreenTimer);
        
        yield return new WaitForSeconds(wait);
        
        //화살표 생성
        SpawnArrows(arrowNum);
        
        //활성 상태가 아니면 화살표를 입력할 수 없다
        CurrentTurnStatus = TurnStatus.활성;
        
        Debug.Log("새로운 턴이 시작되었습니다!");

        yield return new WaitForSeconds(duration);
        
        //모든 시퀀스를 소화했으므로 턴을 종료한다
        yield return StartCoroutine(EndTurn());
    }
    
    private IEnumerator EndTurn()
    {
        //턴을 진행시키던 코루틴을 종료시킨다
        if(Turn != null)
            StopCoroutine(Turn);
        
        const float duration = 3f;
        
        //답지가 출력되는 스크린을 하강시킨다
        LJWConverter.Instance.PositionTransform(false, 0f, duration / 2, new Vector3(3.4f, -11f, 0f), AnswerScreen.transform, curve);
        //타이머 종료
        LJWConverter.Instance.EndIntTimerTMP(false, 0f, duration, AnswerScreenTimer);
        
        //눈동자가 출력되는 뷰어를 하강시킨다x
        PupilBone.velocity = new Vector2(0f, 0f);
        LJWConverter.Instance.PositionTransform(false, duration / 2, duration / 2, new Vector3(0.2f, -14.5f, 0f), EyeDisplay.transform, curve);
        //타이머 종료
        LJWConverter.Instance.EndIntTimerTMP(false, 0f, duration, EyeDisplayTimer);
        
        //월터 블러를 비활성화
        LJWConverter.Instance.GradientSpriteRendererColor(false, 0f, duration, new Color(1f, 1f, 1f, 0f), WalterBlur);
        
        //활성 상태가 아니면 화살표를 입력할 수 없다
        CurrentTurnStatus = TurnStatus.비활성;

        //화살표 삭제
        DespawnArrows();
        
        //눈동자 원위치
        LJWConverter.Instance.PositionTransform(false, 0f, duration, PupilOriginPos, PupilBone.transform, curve);
        
        yield return new WaitForSeconds(duration);
    }

    #endregion

    #region 눈동자 관련 함수
    
    public void OnPupilExit() => StartCoroutine(OnPupilExit_IE());
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
            DespawnArrows();
            SpawnArrows(ArrowNum);
        }

        if (ArrowQueue.Count == 0)
        {
            Debug.Log("모든 화살표를 소화하였으므로 턴을 종료합니다!");
            StartCoroutine(EndTurn());
        }
    }
    public void SpawnArrows(int arrowNum)
    {
        Debug.Log($"총 {arrowNum}개의 화살표를 생성했습니다!");
        var startPos = arrowNum % 2 == 0 ? new Vector3(-0.8f * arrowNum / 2 + 0.5f, 0f, 0f) :  new Vector3(-0.8f * (arrowNum - 1) / 2, 0f, 0f);
        for (var i = 0; i < arrowNum; i++)
        {
            var random = Random.Range(0, 4);
            var arrow = Instantiate(Arrow, ArrowSpawnPos.transform);
            arrow.transform.localPosition = startPos;
            arrow.transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, -90f * random));
            startPos += new Vector3(0.8f,0f, 0f);
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
    public void DespawnArrows()
    {
        var count = ArrowQueue.Count;
        for (var i = 0; i < count; i++)
            Destroy(ArrowQueue.Dequeue());
        Debug.Log($"총 {count}개의 화살표를 삭제했습니다!");
    }

    #endregion
}
