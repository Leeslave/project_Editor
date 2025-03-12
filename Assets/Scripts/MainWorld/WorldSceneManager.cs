using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    private List<WorldVector> _blockList;    // 지역 이동 제한 리스트

    public Location CurrentLocation => locationList[(int)GameSystem.Instance.currentLocation.GetLocation()];

    public SoundManager worldBGM;  // 지역 내 배경음악

    private bool _moving;    // 지역 내 이동 버튼 활성화 여부
    
    [Header("지역 이동 효과")]
    public float moveDelay;     // 지역 이동 딜레이
    
    [SerializeField]
    private Image curtain;      // 지역 이동 효과 이미지


    /// 씬이 새로 로딩될때마다 월드 재로딩
    new void Awake()
    {
        base.Awake();
        
        ReloadWorld();
    }


    /// <summary>
    /// 월드 재로딩, 날짜, 시간대 재적용
    /// </summary>
    public void ReloadWorld()
    {
        // 모든 지역 리로드
        WorldObjectFactory.Instance.Clear();
        
        // TODO: 각 지역 로드를 스레드로 처리
        foreach(var iter in locationList)
        {
            iter.InActiveLocation();
            iter.SetButtons();
            iter.SetObjects();
            iter.SetBGMCode();
        }
        
        // 지역 이동 제한 초기화
        _blockList = GameSystem.Instance.DayData
            .dayTimes[GameSystem.Instance.timeIndex].block;

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
        _moving = !_moving;
        CurrentLocation.SetButtonActive(_moving);
    }

    
    /// <summary>
    /// 지역 변경
    /// </summary>
    /// <remarks>지역을 변경하고 지역 내 위치 동기화</remarks>
    public bool MoveLocation(World location, int position)
    {
        WorldVector newVector = new(location, position);
        
        // 제한된 지역 이동
        if (_blockList.Contains(newVector))
        {
            return false;
        }
        
        // 기존 지역 비활성화
        CurrentLocation.InActiveLocation();

        // 현재 지역 설정
        GameSystem.Instance.currentLocation = newVector;
        
        // 이동 버튼 적용
        CurrentLocation.SetButtonActive(_moving);

        // 새 지역 활성화
        CurrentLocation.ActiveLocation(position);
        
        return true;
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


    /// <summary>
    /// 화면 전환 효과
    /// </summary>
    private IEnumerator FadeInOut()
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