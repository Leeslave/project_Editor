using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine.UI;
using Newtonsoft.Json;
using UnityEngine;

public class NPC : MonoBehaviour
{   
    /**
    대사 실행 오브젝트
    - 타입에 따라 타이밍 맞춰 대사 실행
    */

    public const string NPCFILEPATH = "/Resources/Chat/Text/";    // 대화 파일 경로
    public const string CHARACTERFILEPATH = "Chat/Character/";    // 캐릭터 파일 경로
    public const int IMAGESIZEMULTIPLIER = 150;  // NPC 크기 배율

    [Header("NPC 정보")]
    public string npcFileName;  // NPC 파일명
    public NPCData npcData;    // NPC 데이터
    [Space(10)]
    public int talkCount = 0;   // 대화 횟수 카운트


    void Start()
    {
        // 데이터 로드 전
        if (npcData == null)
        {
            return;
        }

        // 활성화마다 실행
        if (npcData.triggerType == NPCData.TriggerType.EveryStart || npcData.triggerType == NPCData.TriggerType.GlobalChat)
        {
            StartChat();
        }
        // 첫 활성화시 실행
        else if (npcData.triggerType == NPCData.TriggerType.OnStart)
        {
            if (talkCount == 0)
            {
                StartChat();
            }
        }
    }

    void OnEnable()
    {
        if (npcData == null)
        {
            return;
        }
        
        // 활성화마다 실행
        if (npcData.triggerType == NPCData.TriggerType.EveryStart)
        {
            StartChat();
        }
    }
    
    /// <summary>
    /// 대화 시작
    /// </summary>
    public void StartChat()
    {
        if (npcData == null)
        {
            Debug.Log($"NPC DATA CANNOT FOUND : ${npcFileName}");
            return;
        }

        // 첫 활성화 1회 or 클릭 1회 제한
        if (npcData.triggerType == NPCData.TriggerType.OnStart || npcData.triggerType == NPCData.TriggerType.Once)
        {
            if (talkCount > 0)
                return;
        }
        
        Chat.Instance.StartChat(npcData.chatList);
        talkCount++;
    }

    /// <summary>
    /// NPCData 로드
    /// </summary>
    public void GetData()
    {
        string path = Application.dataPath + NPCFILEPATH + npcFileName + ".json";
        if (!File.Exists(path))
        {
            Debug.Log($"NPC FILE CANNOT FOUND : ${path}");
            Destroy(gameObject);
        }
        FileStream fileStream = new FileStream(path, FileMode.Open);
        byte[] data = new byte[fileStream.Length];
        fileStream.Read(data, 0, data.Length);
        fileStream.Close();

        // jsonString 읽어오기
        string jsonText = Encoding.UTF8.GetString(data);
        NPCWrapper wrapper = JsonConvert.DeserializeObject<NPCWrapper>(jsonText, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });
        
        // Wrapper 전환
        npcData = new NPCData(wrapper);

        // 이미지 할당 및 크기 설정
        if (npcData.image != null)
        {
            // 이미지 불러오기
            Sprite newSprite = Chat.GetSprite(CHARACTERFILEPATH + npcData.image);
            if(newSprite == null)
            {
                Debug.Log($"NPC IMAGE CANNOT FOUND : {CHARACTERFILEPATH + npcData.image}");
            }

            // 이미지 크기 조절
            RectTransform rect = GetComponent<RectTransform>();     // 스케일 초기화
            rect.localScale = new Vector3(1,1,1);
            rect.sizeDelta = npcData.size * IMAGESIZEMULTIPLIER 
                                                    * new Vector2(1, newSprite.rect.height/ newSprite.rect.width);
        }
    }
}
