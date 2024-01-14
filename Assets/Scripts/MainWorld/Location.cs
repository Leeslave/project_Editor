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
    [SerializeField]
    int connectLen;     // 연결 가능 최대 길이

    [SerializeField]
    private List<NPC> npcList = new();  // NPC 리스트
    private static readonly string NPCPATH = "/Resources/Chat/Text/"; // NPC 파일 경로
    private static readonly string CGPATH = "Chat/Character/";    // 캐릭터 파일 경로
    public GameObject npcPrefab;   // NPC 생성용 프리팹
    public float sizeMultiplier = 1;    // NPC 크기 배율

    void OnEnable()
    {
        MovePosition(GameSystem.Instance.position);
    }


    /// <summary>
    /// 지역 내 이동
    /// </summary>
    /// <param name="newPos">이동할 새 위치</param>
    public void MovePosition(int newPos)
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

        // 위치값 새로설정
        GameSystem.Instance.SetPosition(newPos);
    }


    // 연결된 맵 왼쪽으로 이동
    public void MoveLeft()
    {
        int newPos = GameSystem.Instance.position - 1;
        if (newPos < 0)
        {
            Debug.Log($"WORLD MOVE ERROR : Invalid position {newPos}");
            return;
        }

        MovePosition(newPos);
    }


    // 연결된 맵 오른쪽으로 이동
    public void MoveRight()
    {
        int newPos = GameSystem.Instance.position + 1;
        if (newPos > connectLen)
        {
            Debug.Log($"WORLD MOVE ERROR : Invalid position {newPos}");
            return;
        }

        MovePosition(newPos);
    }


    public void SetNPC(int date, int time)
    {
        // 이전 오브젝트들 삭제
        foreach(var oldNPC in npcList)
        {
            Destroy(oldNPC);
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
                rect.anchorMin = _data.anchor;
                rect.anchorMax = _data.anchor;
                // 크기 설정
                rect.sizeDelta = _data.size * sizeMultiplier;
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
