using System;
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
    public Location CurrentLocation { get { return locationList[(int)GameSystem.Instance.currentLocation.getLocation()]; } }
    

    public SoundManager worldBGM;  // 지역 내 배경음악

    public bool IsMoving = false;    // 지역 내 이동 버튼 활성화 여부

    [Header("지역 이동 효과")]
    public float moveDelay;     // 지역 이동 딜레이
    [SerializeField]
    private Image curtain;      // 지역 이동 효과 이미지


    /// 씬이 새로 로딩될때마다 월드 재로딩
    new void Awake()
    {
        base.Awake();

        ReloadWorld();
        CurrentLocation.SetButtonActive(IsMoving);
    }


    /// <summary>
    /// 월드 재로딩, 날짜&시간대 재적용
    /// </summary>
    public void ReloadWorld()
    {
        // 모든 지역 리로드
        foreach(var iter in locationList)
        {
            iter.ReloadLocation();
        }

        // 현재 지역 활성화
        CurrentLocation.ActiveLocation(true);
    }


    /// <summary>
    /// 지역 이동 버튼 활성화
    /// </summary>
    public void SetMoveActive()
    {
        IsMoving = !IsMoving;
        CurrentLocation.SetButtonActive(IsMoving);
    }

    /// <summary>
    /// 지역 변경
    /// </summary>
    /// <remarks>지역을 변경하고 지역 내 위치 동기화
    public void MoveLocation(World location)
    {
        // 기존 지역 비활성화
        CurrentLocation.ActiveLocation(false);

        // TODO: 현재 지역 설정
        // GameSystem.Instance.SetLocation(location);

        // 새 지역 활성화
        CurrentLocation.ActiveLocation(true);
        
        // 이동 버튼 비활성화
        IsMoving = false;
        CurrentLocation.SetButtonActive(IsMoving);
    }


    /// <summary>
    /// 지역 변경
    /// </summary>
    /// <remarks>지역을 변경하고 지역 내 위치 동기화
    public void MoveLocation(string location)
    {
        MoveLocation(Enum.Parse<World>(location));
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