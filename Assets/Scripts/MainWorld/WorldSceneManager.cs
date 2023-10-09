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
    private List<GameObject> npcList;     // 모든 지역 NPC 리스트

    [Header("NPC 생성 정보")]
    [SerializeField]
    private GameObject npcPrefab;   // NPC 생성용 프리팹
    [SerializeField]
    private int npcSizeMultiplier;  // NPC 크기 배율

    private void Start()
    {
        // 추가 인트로 실행 (날짜 변경 후 0시에만)
        if (GameSystem.Instance.time == 0)
        {
            if (intro)
                StartCoroutine("WaitForIntro");
        }
        else
        {
            // 위치 설정
            SetLocation((int)GameSystem.Instance.location);

            // npc 생성
            npcList = new();
            SetWorldObject(); 
        }              
    }

    // 인트로 오브젝트 대기
    IEnumerator WaitForIntro()
    {
        intro.gameObject.SetActive(true);
        yield return new WaitUntil(() => intro.isFinished);
        
        // 위치 설정
        SetLocation((int)GameSystem.Instance.location);

        // npc 생성
        npcList = new();
        SetWorldObject();
    }

    /// 지역 이동
    public void SetLocation(int location)
    {
        // 이동할 지역 설정
        if (!Enum.IsDefined(typeof(World), location))
        {
            Debug.Log("Location number ERROR");
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
        GameSystem.Instance.location = (World)location;

        // 지역 내 위치 동기화
        SetPosition(GameSystem.Instance.position);
    }

    /// 지역 내 이동
    public void SetPosition(int position)
    {
        // 현재 월드 오브젝트
        Transform currentWorldObject = locationList[(int)GameSystem.Instance.location].transform;

        // 위치값 예외 처리
        if (position < 0 || GameSystem.Instance.position >= currentWorldObject.childCount)
            position = 0;
        
        // 해당 위치 활성화
        for(int i = 0; i < currentWorldObject.childCount; i++)
        {
            currentWorldObject.GetChild(i).gameObject.SetActive(false);
            if (i == position)
                currentWorldObject.GetChild(i).gameObject.SetActive(true);
        }

        // 현재 위치 설정
        GameSystem.Instance.position = position;
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
        if (time != GameSystem.Instance.time + 1)
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
    /// 월드 오브젝트 설정
    /// </summary>
    private void SetWorldObject()
    {
        // 새 오브젝트 리스트
        List<NPCSchedule> schedules = GameSystem.Instance.today.npcScheduleList[GameSystem.Instance.time];

        // 이전 오브젝트들 삭제
        foreach(var oldNPC in npcList)
        {
            Destroy(oldNPC);
        }
        // 리스트 초기화
        npcList = new();

        // 새 오브젝트 생성
        foreach(var newNPCData in schedules)
        {
            GameObject newNPC = CreateWorldObject(newNPCData);
            if (newNPC == null)
            {
                Debug.Log($"NPC 생성 오류: {newNPCData.name}");
                continue;
            }
            npcList.Add(newNPC);
        }
    }

    /// <summary>
    /// 월드에 새 NPC 생성
    /// </summary>
    /// <param name="npc"></param>
    /// <returns></returns>
    private GameObject CreateWorldObject(NPCSchedule npc)
    {
        // 오브젝트 생성
        GameObject newNPC = Instantiate(npcPrefab);
        newNPC.name = npc.name;

        // 오브젝트 transform 설정
        newNPC.transform.SetParent(locationList[npc.location].transform.GetChild(npc.position));
        RectTransform npcTransform = newNPC.GetComponent<RectTransform>();
        npcTransform.anchoredPosition = new Vector2(npc.x, npc.y);
        npcTransform.localScale = new Vector3(1,1,1);   // 스케일 초기화

        // 오브젝트 이미지 설정
        if (npc.image != null)
        {
            Image newImage = newNPC.GetComponent<Image>();      
            newImage.sprite = Resources.Load<Sprite>("Image/" + npc.image);
            if (newImage.sprite == null)
            {
                Debug.Log($"이미지 오류 : {npc.name}");
                return null;
            }
            // 오브젝트 크기 설정
            npcTransform.sizeDelta = new Vector2(npc.size * npcSizeMultiplier, npc.size * npcSizeMultiplier * (newImage.sprite.rect.height/ newImage.sprite.rect.width));   // 비율 맞춰서 사이즈 설정
        }        

        // Chat Trigger 설정
        if (npc.chat != null)
        {
            ChatTrigger npcChatTrigger = newNPC.GetComponent<ChatTrigger>(); 
            npcChatTrigger.chatName = npc.chat;     // 대사 파일명 설정 
            npcChatTrigger.triggerType = npc.chatType;  // 대사 타입 설정
        }
        
        return newNPC;
    }
}