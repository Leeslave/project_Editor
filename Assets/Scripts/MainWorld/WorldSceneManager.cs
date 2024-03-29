using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public enum World {
    /**
    월드 내 지역 목록
    */
    Street,
    Bar,
    Cafe,
    Restaurant,
    Hallway,
    Office,
    Office2,
    Interrogate
}


public class WorldSceneManager : MonoBehaviour 
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
    private int CurrentIndex { get { return (int)GameSystem.Instance.gameData.location; } }    // 현재 지역

    public SoundManager worldBGM;  // 지역 내 배경음악
    
    public bool IsMoving { get; private set; } = false;    // 지역 내 이동 버튼 활성화 여부
    public GameObject objPrefab;

    [Header("지역 이동 효과")]
    public float moveDelay;     // 지역 이동 딜레이
    [SerializeField]
    private Image curtain;      // 지역 이동 효과 이미지

    /// 싱글턴 선언
    private static WorldSceneManager _instance;
    public static WorldSceneManager Instance { get { return _instance; } }


    /// 씬이 새로 로딩될때마다 월드 재로딩
    void Awake()
    {
        if (!_instance)
            _instance = this;
        else
            Destroy(gameObject);

        ReloadWorld();
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
        locationList[CurrentIndex].ActiveLocation(true);
    }


    /// <summary>
    /// 지역 이동 버튼 활성화
    /// </summary>
    public void SetMoveActive()
    {
        IsMoving = !IsMoving;
        locationList[CurrentIndex].SetButtonActive(IsMoving);
    }

    /// <summary>
    /// 지역 변경
    /// </summary>
    /// <remarks>지역을 변경하고 지역 내 위치 동기화
    public void MoveLocation(World location)
    {
        // 기존 지역 비활성화
        locationList[CurrentIndex].ActiveLocation(false);

        // 현재 지역 설정
        GameSystem.Instance.gameData.SetLocation(location);

        // 새 지역 활성화
        locationList[CurrentIndex].ActiveLocation(true);
    }


    /// <summary>
    /// 지역 변경
    /// </summary>
    /// <remarks>지역을 변경하고 지역 내 위치 동기화
    public void MoveLocation(string location)
    {
        World newLocation = Enum.Parse<World>(location);
        // 기존 지역 비활성화
        locationList[CurrentIndex].ActiveLocation(false);

        // 현재 지역 설정
        GameSystem.Instance.gameData.SetLocation(newLocation);

        // 새 지역 활성화
        locationList[CurrentIndex].ActiveLocation(true);
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