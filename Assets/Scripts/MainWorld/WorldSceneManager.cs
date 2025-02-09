using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;


public class WorldSceneManager : Singleton<WorldSceneManager> 
{
    /**
    * MainWorld 씬 매니저
    *   - 날짜, 시간대 맞춰서 지역 동기화
    *   - 지역 내 위치 이동
    *   - 지역 간 이동
    */

    [Header("지역 데이터")]
    [SerializeField]
    private Location[] locationList;    // 지역 오브젝트 리스트

    public Location CurrentLocation => locationList[(int)GameSystem.Instance.currentLocation.GetLocation()];

    public SoundManager worldBGM;  // 지역 내 배경음악

    public bool isMoving { get; private set; }    // 지역 내 이동 버튼 활성화 여부
    
    [Header("지역 이동 효과")]
    public float moveDelay;     // 지역 이동 딜레이
    
    [SerializeField]
    private Image curtain;      // 지역 이동 효과 이미지


    /// 씬이 새로 로딩될때마다 월드 재로딩
    new void Awake()
    {
        base.Awake();

        ReloadWorld();
        CurrentLocation.SetButtonActive(isMoving);
    }


    /// <summary>
    /// 월드 재로딩, 날짜, 시간대 재적용
    /// </summary>
    public void ReloadWorld()
    {
        // 모든 지역 리로드
        foreach(var iter in locationList)
        {
            iter.InActiveLocation();
            iter.SetObjects();
        }

        // 날짜 변경 시 위치 초기화
        if (GameSystem.Instance.timeIndex == 0)
        {
            GameSystem.Instance.currentLocation = GameSystem.Instance.DayData.startLocation;
        }

        // 현재 지역 활성화
        CurrentLocation.ActiveLocation(GameSystem.Instance.currentLocation.position);
    }


    /// <summary>
    /// 지역 이동 버튼 활성화
    /// </summary>
    public void SetMoveActive()
    {
        isMoving = !isMoving;
        CurrentLocation.SetButtonActive(isMoving);
    }

    
    /// <summary>
    /// 지역 변경
    /// </summary>
    /// <remarks>지역을 변경하고 지역 내 위치 동기화</remarks>
    public void MoveLocation(World location, int position)
    {
        // 기존 지역 비활성화
        CurrentLocation.InActiveLocation();

        // 현재 지역 설정
        GameSystem.Instance.currentLocation = new WorldVector(location, position);

        // 새 지역 활성화
        CurrentLocation.ActiveLocation(position);
        
        // 이동 버튼 비활성화
        isMoving = false;
        CurrentLocation.SetButtonActive(isMoving);
    }

    
    /// <summary>
    /// 위치 이동 
    /// </summary>
    /// <param name="position">이동할 위치</param>
    private void MovePosition(int position)
    {
        StartCoroutine(FadeInOut());
        GameSystem.Instance.currentLocation.position = position;
        CurrentLocation.SetPosition(position);
    }
    
    
    // 연결된 맵 왼쪽으로 이동
    public void MoveLeft()
    {
        MovePosition(GameSystem.Instance.currentLocation.position - 1);
    }


    // 연결된 맵 오른쪽으로 이동
    public void MoveRight()
    {
        MovePosition(GameSystem.Instance.currentLocation.position + 1);
    }


    /// <summary>
    /// 화면 전환 효과
    /// </summary>
    public IEnumerator FadeInOut()
    {
        float elapsedTime = 0f;
        
        // 점점 밝아지기
        while (elapsedTime < moveDelay)
        {
            curtain.color = Color.Lerp(Color.black, Color.clear, elapsedTime / moveDelay);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
}