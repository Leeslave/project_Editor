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
    Desk,
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
    private GameObject[] locationList;    // 지역 오브젝트 리스트
    [SerializeField]
    private GameObject LeftButton;      // 왼쪽 이동 버튼
    [SerializeField]
    private GameObject RightButton;     // 오른쪽 이동 버튼
    public float moveDelay;     // 지역 이동 딜레이
    [SerializeField]
    private Image curtain;      // 지역 이동 효과 이미지
    public AudioSource worldBGM;  // 지역 내 배경음악

    [Header("NPC 생성 정보")]
    [SerializeField]
    private List<GameObject> npcList = new();     // 모든 지역 NPC 리스트
    [SerializeField]
    private GameObject npcPrefab;   // NPC 생성용 프리팹

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
        if (GameSystem.Instance.currentTime == 0)
        {
            if (intro)
                StartCoroutine(WaitForIntro());
        }
        else
        {
            // 위치 설정
            MoveLocation(GameSystem.Instance.currentLocation.ToString());
            MovePosition(GameSystem.Instance.currentPosition.ToString());

            // npc 생성
            SetWorldObject(); 
        }              
    }

    // 인트로 오브젝트 대기
    IEnumerator WaitForIntro()
    {
        intro.gameObject.SetActive(true);
        yield return new WaitUntil(() => intro.isFinished);
        
        // 위치 설정
        MoveLocation(GameSystem.Instance.currentLocation.ToString());
        MovePosition(GameSystem.Instance.currentPosition.ToString());

        // npc 생성
        SetWorldObject();
    }

    /// <summary>
    /// 지역 변경
    /// </summary>
    /// <remarks>지역을 변경하고 지역 내 위치 동기화
    public void MoveLocation(string location)
    {
        World newLocation;
        try
        {
            // 이동할 지역 설정
            newLocation = Enum.Parse<World>(location);
        }
        catch(ArgumentException)
        {
            Debug.Log($"LOCATION LOAD FAILED : Invalid location Name {location}");
            return;
        }

        // 해당하는 지역만 활성화
        for(int i = 0; i < locationList.Length; i++)
        {
            locationList[i].SetActive(false);
            if (i == (int)newLocation)
            {
                locationList[i].SetActive(true);
                // 지역 내 정보 활성화
                if (locationList[i].TryGetComponent(out AudioSource audio))
                {
                    worldBGM = audio;
                    worldBGM.Play();
                }
            }
        }        

        // 현재 지역 설정
        GameSystem.Instance.currentLocation = newLocation;
    }

    /// <summary>
    /// 지역 내 이동
    /// </summary>
    /// <remarks>지역 내 위치를 이동
    public void MovePosition(string position)
    {
        int newPos;

        // 좌, 우 or int로 이동할 위치값 설정
        switch (position)
        {
            case "Left":
                newPos = GameSystem.Instance.currentPosition - 1;
                break;
            case "Right":
                newPos = GameSystem.Instance.currentPosition + 1;
                break;
            default:
                if(!int.TryParse(position, out newPos))
                {
                    Debug.Log($"WORLD MOVE ERROR : Cannot move to position {position}");
                    return;
                }
                break;
        }
        

        // 현재 월드 오브젝트
        Transform currentWorldTransform = locationList[(int)GameSystem.Instance.currentLocation].transform;

        // 위치값 예외 처리
        if (newPos < 0 || newPos >= currentWorldTransform.childCount)
        {
            Debug.Log($"WORLD MOVE ERROR : Invalid position {newPos}");
            return;
        }

        // 양쪽 이동 버튼 설정
        LeftButton.SetActive(false);
        RightButton.SetActive(false);    
        
        // 0보다 클때 왼쪽 이동 가능
        if (newPos > 0)
        {
            LeftButton.SetActive(true);
        }
        // 오른쪽 끝보다 작을때 오른쪽 이동 가능
        if(newPos < currentWorldTransform.childCount - 1)
        {
            RightButton.SetActive(true);
        }

        // 전환 효과 실행
        StartCoroutine(FadeInOut());
        
        // 해당 위치 활성화
        for(int i = 0; i < currentWorldTransform.childCount; i++)
        {
            currentWorldTransform.GetChild(i).gameObject.SetActive(false);
            if (i == newPos)
            {
                currentWorldTransform.GetChild(i).gameObject.SetActive(true);
            }
        }

        // 현재 위치 설정
        GameSystem.Instance.currentPosition = newPos;
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
        */

        // 특정 시간대 전환
        // 해당하는 시간대 전환이 아니면 실행 안함
        if (time > 0 && time < 4)
        {
            if (time != GameSystem.Instance.currentTime + 1)
            {
                return;
            }
        }

        // 자동 시간 넘김
        if(time == -1)
        {
            time = GameSystem.Instance.currentTime + 1;
        }

        // 날짜 변경 (시간대 4일시)
        if (time == 4)
        {
            GameSystem.Instance.SetDate();
            SetWorldObject();
            return;
        }

        // 해당시간으로 설정
        GameSystem.Instance.SetTime(time);

        // NPC 데이터 
        SetWorldObject();
    }

    /// <summary>
    /// 현재 시간대의 월드 오브젝트 설정
    /// </summary>
    private void SetWorldObject()
    {
        // 이전 오브젝트들 삭제
        foreach(var oldNPC in npcList)
        {
            Destroy(oldNPC);
        }

        // 리스트 초기화
        npcList = new();
        // 새 오브젝트 데이터
        List<string> npcFiles = GameSystem.Instance.today.npcList[GameSystem.Instance.currentTime];
        if (npcFiles == null)
            return;

        // 오브젝트들 생성 및 위치 설정
        foreach(var newNPCName in npcFiles)
        {
            // 새 오브젝트 생성
            GameObject newObject = Instantiate(npcPrefab);
            Debug.Log($"Create New NPC : {newNPCName}");

            // 생성한 오브젝트 정보 로드
            NPC newNPC = newObject.GetComponent<NPC>();
            newNPC.npcFileName = newNPCName;
            newNPC.GetData();

            // 오브젝트 정보 로드 실패
            if (newObject == null)
            {
                Debug.Log($"NPC CREATE FAIED : ${newNPCName}");
                continue;
            }
            
            // NPC 정보
            NPCData newNPCData = newNPC.npcData;

            // 오브젝트 transform 설정
            newObject.transform.SetParent(locationList[(int)newNPCData.location].transform.GetChild(newNPCData.locationIndex));

            // 생성 완료, 리스트에 추가
            npcList.Add(newObject);
        }
    }

    /// <summary>
    /// 화면 전환 효과
    /// </summary>
    IEnumerator FadeInOut()
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