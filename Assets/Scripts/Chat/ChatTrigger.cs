using JetBrains.Annotations;
using System.Collections.Generic;
using UnityEngine;

public class ChatTrigger : MonoBehaviour
{
    /**
     * 대사를 출력하는 컴포넌트
     * - 필요한 대사 목록으로 데이터를 미리 로드
     * - index를 통해 대사를 선택하여 출력 가능
     */
    
    public List<string> chatAssets;
    private List<List<Paragraph>> _chatData;


    private void Start()
    {
        LoadChatData();
    }

    /// <summary>
    /// Load All chat Data
    /// </summary>
    public void LoadChatData()
    {
        _chatData = new();
        
        foreach (var asset in chatAssets)
        {
            var item = DataLoader.GetChatData(asset);
            _chatData.Add(item);
        }
    }


    /// <summary>
    /// Start Chat
    /// </summary>
    /// <param name="idx">chat data index</param>
    public void Init(int idx)
    {
        if (idx < 0 || idx >= _chatData.Count )
        {
            Debug.LogWarning("Invalid Chat Asset Index");
            return;
        }

        if (_chatData[idx] is null)
        {
            Debug.LogWarning("Failed to Load Chat Data");
            return;
        }
              
        Chat.Instance.StartChat(_chatData[idx]);
    }
}
