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
    Interrogation
}

public class WorldSceneManager : MonoBehaviour 
{
    /**
    * MainWorld 씬 매니저
    *   - 날짜, 시간대 맞춰서 지역 동기화
    *   - 지역 내 위치 이동
    *   - 지역 간 이동
    */
    [SerializeField]
    private DayIntro intro;      // 인트로 오브젝트

    [Header("지역 데이터")]
    [SerializeField]
    private GameObject[] locationList;    // 지역 오브젝트 리스트

    [SerializeField]
    private List<GameObject> npcList = new();     // 모든 지역 NPC 리스트
    [SerializeField]
    private static int sizeMultiplier;  // NPC 크기 배율

    private void Start()
    {
        // 추가 인트로 실행 (날짜 변경 후 0시에만)
        if (GameSystem.Instance.currentTime == 0)
        {
            if (intro)
                StartCoroutine("WaitForIntro");
        }
        else
        {
            // 위치 설정
            SetLocation((int)GameSystem.Instance.currentLocation);

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
        SetLocation((int)GameSystem.Instance.currentLocation);

        // npc 생성
        SetWorldObject();
    }

    /// 지역 이동
    public void SetLocation(int location)
    {
        // 이동할 지역 설정
        if (!Enum.IsDefined(typeof(World), location))
        {
            Debug.Log($"ERROR: Location number incorrect [${location}]");
            location = 0;
        }

        // 해당하는 지역만 활성화
        for(int i = 0; i < locationList.Length; i++)
        {
            locationList[i].SetActive(false);
            if (i == location)
                locationList[i].SetActive(true);
        }

        // 현재 지역 설정
        GameSystem.Instance.currentLocation = (World)location;

        // 지역 내 위치 동기화
        SetPosition(GameSystem.Instance.currentPosition);
    }

    /// 지역 내 이동
    public void SetPosition(int position)
    {
        // 현재 월드 오브젝트
        Transform currentWorldObject = locationList[(int)GameSystem.Instance.currentLocation].transform;

        // 위치값 예외 처리
        if (position < 0 || GameSystem.Instance.currentPosition >= currentWorldObject.childCount)
            position = 0;
        
        // 해당 위치 활성화
        for(int i = 0; i < currentWorldObject.childCount; i++)
        {
            currentWorldObject.GetChild(i).gameObject.SetActive(false);
            if (i == position)
                currentWorldObject.GetChild(i).gameObject.SetActive(true);
        }

        // 현재 위치 설정
        GameSystem.Instance.currentPosition = position;
    }

    /// <summary>
    /// 시간대 변경
    /// </summary>
    /// <remarks>시간대를 변경하고 현재 지역 동기화
    public void ChangeTime(int time)
    {
        /**
        시간대 변경에 따른 지역들 동기화
        - NPC들 활성화, 위치 동기화
        - 지역들 배경 이미지 변경
        - 바로 다음 시간대로만 변경
        */

        // 시간대 변경 제한
        if (time < 1 || time > 4)
            return;
        
        // 해당하는 시간 변경이 아닐 시 종료
        if (time != GameSystem.Instance.currentTime + 1)
        {
            return;
        }

        // 날짜 변경
        if (time == 4)
        {
            GameSystem.Instance.SetDate();
            SetWorldObject();
            return;
        }

        // 시간 변경
        GameSystem.Instance.SetTime(time);
        SetWorldObject();
    }

    /// <summary>
    /// 현재 시간대의 월드 오브젝트 설정
    /// </summary>
    private void SetWorldObject()
    {
        // 새 오브젝트 리스트
        List<string> npcFiles = GameSystem.Instance.today.npcList[GameSystem.Instance.currentTime];
        if (npcFiles == null)
            return;
            
        // 이전 오브젝트들 삭제
        foreach(var oldNPC in npcList)
        {
            Destroy(oldNPC);
        }
        // 리스트 초기화
        npcList = new();

        // 오브젝트들 생성 및 위치 설정
        foreach(var newNPCName in npcFiles)
        {
            // 새 오브젝트 생성
            GameObject newNPC = Chat.CreateNPC(newNPCName);
            if (newNPC == null)
            {
                Debug.Log($"NPC 생성 오류: {newNPCName}");
                continue;
            }
            NPCData newNPCData = newNPC.GetComponent<NPC>().npcData;
            RectTransform newNPCRect = newNPC.GetComponent<RectTransform>();
            newNPC.SetActive(false);

            // 오브젝트 이미지 설정
            if (newNPCData.image != null)
            {
                Image newImage = newNPC.GetComponent<Image>();      
                newImage.sprite = Chat.GetSprite(newNPCData.image);
                if (newImage.sprite == null)
                {
                    Debug.Log($"이미지 없음 : {newNPCData.name}");
                    Destroy(newNPC);
                    continue;
                }
                // 오브젝트 크기 설정
                newNPCRect.sizeDelta = newNPCData.size * new Vector2(1, newImage.sprite.rect.height/ newImage.sprite.rect.width) * sizeMultiplier;    // 비율 맞춰서 사이즈 설정
            }   
            
            // 오브젝트 transform 설정
            newNPC.transform.SetParent(locationList[(int)newNPCData.location].transform.GetChild(newNPCData.locationIndex));
            newNPCRect.anchoredPosition = newNPCData.position;
            newNPCRect.localScale = new Vector3(1,1,1);   // 스케일 초기화

            npcList.Add(newNPC);
        }
    }
}