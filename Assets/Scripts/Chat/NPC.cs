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

    // NPC 대화 타입
    public enum TriggerType {   
        OnClick,    // 버튼 누를 시
        OnStart,    // 활성화시 자동 1회
        EveryStart, // 매번 활성화마다
        Once,       // 한번 실행 후 삭제
        GlobalChat, // NPC 없이 배경 한번 실행 후 삭제
    }

    public const string NPCFILEPATH = "/Resources/Chat/Text/";

    public string npcFileName;  // NPC 파일명
    public NPCData npcData;    // NPC 데이터
    public int talkCount = 0;   // 대화 횟수 카운트

    void Start()
    {
        if (npcData == null)
        {
            return;
        }

        if (npcData.triggerType == TriggerType.EveryStart)
        {
            StartChat();
        }
        else if (npcData.triggerType == TriggerType.OnStart)
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
        
        if (npcData.triggerType == TriggerType.EveryStart)
        {
            StartChat();
        }
    }

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
        if (npcData.triggerType == TriggerType.OnStart || npcData.triggerType == TriggerType.Once)
        {
            if (talkCount > 0)
                return;
        }
        
        Chat.Instance.StartChat(npcData.chatList);
        talkCount++;
    }
}
