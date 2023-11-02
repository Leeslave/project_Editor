using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;

public class NPC : MonoBehaviour
{   
    /**
    대사 실행 오브젝트
    - 타입에 따라 타이밍 맞춰 대사 실행
    */

    public const string NPCFILEPATH = "/Resources/Chat/Text/";

    public string npcFileName;  // NPC 파일명
    public NPCData npcData;    // NPC 데이터
    public int talkCount = 0;   // 대화 횟수 카운트

    void Start()
    {
        // 데이터 로드 전
        if (npcData == null)
        {
            return;
        }

        // 활성화마다 실행
        if (npcData.triggerType == NPCData.TriggerType.EveryStart)
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
    /// NPCData 로드
    /// </summary>
    public void GetData()
    {
        string path = Application.dataPath + NPCFILEPATH + npcFileName + ".json";
        if (!File.Exists(path))
        {
            Debug.Log($"NPC FILE CANNOT FOUND : ${path}");
            return;
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
}
