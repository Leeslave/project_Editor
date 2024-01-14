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
    [Header("인트로 데이터")]
    [SerializeField]
    private DayIntro intro;      // 인트로 오브젝트

    [Header("지역 데이터")]
    [SerializeField]
    private Location[] locationList;    // 지역 오브젝트 리스트
    public float moveDelay;     // 지역 이동 딜레이
    [SerializeField]
    private Image curtain;      // 지역 이동 효과 이미지
    public SoundManager worldBGM;  // 지역 내 배경음악


    /// 싱글턴 선언
    private static WorldSceneManager _instance;
    public static WorldSceneManager Instance { get { return _instance; } }
    void Awake()
    {
        if (!_instance)
            _instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        // 추가 인트로 실행 (날짜 변경 후 0시에만)
        if (GameSystem.Instance.time == 0)
        {
            if (intro)
                StartCoroutine(WaitForIntro());
        }
        else
        {
            ChangeTime(GameSystem.Instance.time);
            // 위치 설정
            MoveLocation(GameSystem.Instance.location);
        }              
    }

    // 인트로 오브젝트 대기
    IEnumerator WaitForIntro()
    {
        intro.gameObject.SetActive(true);
        yield return new WaitUntil(() => intro.isFinished);
        
        // 시간 설정
        ChangeTime(0);
        // 위치 설정
        MoveLocation(GameSystem.Instance.location);
    }

    /// <summary>
    /// 지역 변경
    /// </summary>
    /// <remarks>지역을 변경하고 지역 내 위치 동기화
    public void MoveLocation(World location)
    {
        // 해당하는 지역만 활성화
        for(int i = 0; i < locationList.Length; i++)
        {
            locationList[i].gameObject.SetActive(false);
            if (i == (int)location)
            {
                locationList[i].gameObject.SetActive(true);
                // 지역 내 정보 활성화
                Debug.Log($"Set World BGM : {i}");
                worldBGM.OverlapPlay(i);
            }
        }        

        // 현재 지역 설정
        GameSystem.Instance.SetLocation(location);
    }

    public void SetPosition(int newPos)
    {
        GameSystem.Instance.SetPosition(newPos);
    }

    /// <summary>
    /// 시간대 변경
    /// </summary>
    /// <remarks>시간대를 변경하고 현재 지역 동기화
    public void ChangeTime(int time = -1)
    {
        /**
        시간대 변경에 따른 지역들 동기화
        - NPC들 활성화, 위치 동기화
        - 지역들 배경 이미지 변경
        - 바로 다음 시간대로만 변경
        - 0시로 변경가능 (초기화)
        */

        // 자동 시간 넘김
        if(time == -1)
        {
            time = GameSystem.Instance.time + 1;
        }

        // 시간대 오류
        if(time > 4 || time < 0)
        {
            Debug.Log($"Invalid Time : {time}");
            return;
        }

        // 특정 시간대 전환
        // 해당하는 시간대 전환이 아니면 실행 안함
        if (time > 0 && time <= 4)
        {
            if (time != GameSystem.Instance.time + 1)
            {
                return;
            }
        }

        // 날짜 변경 (시간대 4일시)
        if (time == 4)
        {
            // 날짜 재설정
            GameSystem.Instance.SetDate(GameSystem.Instance.date + 1);
            
            // NPC 옵저빙
            SetWorldObject();

            // 위치 설정
            MoveLocation(GameSystem.Instance.today.startLocation);
            SetPosition(GameSystem.Instance.today.startPosition);
            return;
        }

        // 해당시간으로 설정
        GameSystem.Instance.SetTime(time);

        // NPC 옵저빙
        SetWorldObject();

        // 위치 설정
        MoveLocation(GameSystem.Instance.today.startLocation);
        SetPosition(GameSystem.Instance.today.startPosition);
    }

    /// <summary>
    /// 현재 시간대의 월드 오브젝트 설정
    /// </summary>
    private void SetWorldObject()
    {
        foreach(var location in locationList)
        {
            location.gameObject.SetActive(true);
            location.SetNPC(GameSystem.Instance.date, GameSystem.Instance.time);
            location.gameObject.SetActive(false);
        }
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