using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

public class Location : MonoBehaviour
{
    [SerializeField]
    private string locationName; // 지역명

    private bool isActive;  // 현재 활성화 여부

    [SerializeField]
    public int bgmNumber;   // BGM 번호

    [SerializeField]
    private int connectLen;     // 연결 가능 최대 길이
    [SerializeField]
    private List<GameObject> buttons;   // 지역 내 위치 이동 버튼들

    [SerializeField]
    private List<NPC> npcList = new();  // NPC 리스트
    private static readonly string NPCPATH = "/Resources/Chat/Text/"; // NPC 파일 경로
    private static readonly string CGPATH = "Chat/Character/";    // 캐릭터 파일 경로
    public GameObject npcPrefab;   // NPC 생성용 프리팹
    public float sizeMultiplier = 1;    // NPC 크기 배율


    /// <summary>
    /// 현재 지역을 활성화/비활성화
    /// </summary>
    public void ActiveLocation(bool _active)
    {
        // 변화 없음 예외
        if (isActive == _active)
            return;
        
        // 활성화
        if (_active)
        {
            // 음악 활성화
            WorldSceneManager.Instance.worldBGM.OverlapPlay(bgmNumber);

            // 해당하는 위치 활성화
            SetPosition(GameSystem.Instance.position);
        }
        // 비활성화
        else
        {
            // 위치 전부 비활성화
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }
        }
    }


    /// <summary>
    /// 지역 재로딩, 날짜&시간대 재적용
    /// </summary>
    public void ReloadLocation()
    {
        
        // NPC 새로 생성
        SetObjects(GameSystem.Instance.date, GameSystem.Instance.time);

        // 기본으로 비활성화
        ActiveLocation(false);
    }



    /// <summary>
    /// 지역 내 이동
    /// </summary>
    /// <param name="newPos">이동할 새 위치</param>
    [HideInInspector]
    public void SetPosition(int newPos)
    {
        // 위치값 오류
        if (newPos < 0 || newPos >= transform.childCount)
        {
            Debug.Log($"WORLD MOVE ERROR : Invalid position {newPos}");
            return;
        }

        // 페이드인아웃 효과 실행
        StartCoroutine(WorldSceneManager.Instance.FadeInOut());

        // 이동할 장소 활성화
        transform.GetChild(newPos).gameObject.SetActive(true);
        // 나머지 장소 비활성화
        for(int i = 0; i < transform.childCount; i++)
        {
            if (i == newPos)
            {
                continue;
            }
            transform.GetChild(i).gameObject.SetActive(false);
        }
    }


    // 연결된 맵 왼쪽으로 이동
    [SerializeField]
    private void MoveLeft()
    {
        int newPos = GameSystem.Instance.position - 1;
        if (newPos < 0)
        {
            Debug.Log($"WORLD MOVE ERROR : Invalid position {newPos}");
            return;
        }

        SetPosition(newPos);
    }


    // 연결된 맵 오른쪽으로 이동
    [SerializeField]
    private void MoveRight()
    {
        int newPos = GameSystem.Instance.position + 1;
        if (newPos > connectLen)
        {
            Debug.Log($"WORLD MOVE ERROR : Invalid position {newPos}");
            return;
        }

        SetPosition(newPos);
    }


    /// <summary>
    /// 지역 이동 버튼 활성화
    /// </summary>
    /// <param name="isActive">버튼 활성화 여부</param>
    [HideInInspector]
    public void SetButtonActive(bool isActive)
    {
        foreach(var button in buttons)
        {
            button.SetActive(isActive);
        }
    }


    /// <summary>
    /// 시간대에 해당하는 NPC들 생성
    /// </summary>
    /// <param name="date">날짜</param>
    /// <param name="time">시간대</param>
    public void SetObjects(int date, int time)
    {
        // 이전 오브젝트들 삭제
        foreach(var oldNPC in npcList)
        {
            Destroy(oldNPC.gameObject);
        }

        // 리스트 초기화
        npcList = new();

        // NPC 데이터들 파싱
        string path = Application.dataPath + $"{NPCPATH}day {date}/{time}/{locationName}";
        List<NPCData> dataList = ParseNPCData(path);
        if (dataList == null)
        {
            return;
        }

        // NPC들 생성
        foreach(NPCData _data in dataList)
        {
            GameObject newNPC = Instantiate(npcPrefab);     // instantiate 
            newNPC.transform.SetParent(transform.GetChild(_data.position)); // set Parent
            newNPC.name = _data.name;

            // NPC 리스트에 추가
            npcList.Add(newNPC.GetComponent<NPC>());

            // NPC property 설정
            newNPC.GetComponent<NPC>().data = _data;  

            // NPC transform 설정
            if (_data.image != null)
            {
                // 이미지 설정
                Sprite newSprite = Chat.GetSprite(CGPATH + _data.image + "_stand");
                if(newSprite == null)
                {
                    Debug.Log($"NPC IMAGE CANNOT FOUND : {CGPATH + _data.image}");
                }
                newNPC.GetComponent<Image>().sprite = newSprite;

                RectTransform rect = newNPC.GetComponent<RectTransform>();
                // 위치 설정
                Debug.Log(_data.anchor);
                rect.anchorMin = _data.anchor;
                rect.anchorMax = _data.anchor;
                Debug.Log($"Anchor applied : {rect.anchorMin} ~ {rect.anchorMax}");
                // 크기 설정
                rect.sizeDelta = _data.size * sizeMultiplier;
                rect.localScale = new Vector3(1f,1f,1f);
            }
        }

        Debug.Log($"{gameObject.name} : {npcList.Count}개 NPC 생성됨");
    }

    
    /// <summary>
    /// 경로에서 NPCData들 파싱
    /// </summary>
    /// <param name="path">파싱할 NPC 데이터 경로</param>
    /// <returns></returns>
    private static List<NPCData> ParseNPCData(string path)
    {
        // NPC 존재 여부 확인
        if (!Directory.Exists(path))
            return null;

        // NPC 파일들 불러오기
        string[] files = Directory.GetFiles(path, "*.json");
        List<NPCData> npcDatas = new();
        // string으로 전환
        for (int i = 0; i < files.Length; i++)
        {
            using (FileStream fs = new FileStream(files[i], FileMode.Open))
            {
                byte[] buffer = new byte[fs.Length];
                fs.Read(buffer, 0, (int)fs.Length);
                string jsonText = Encoding.UTF8.GetString(buffer);
                
                NPCWrapper wrapper = JsonConvert.DeserializeObject<NPCWrapper>(jsonText, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });
                npcDatas.Add(new NPCData(wrapper));
            }
        }

        return npcDatas;
    }
}
