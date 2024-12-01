using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ChatTrigger : MonoBehaviour
{
    [SerializeField] private string[] chatAssets;
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
