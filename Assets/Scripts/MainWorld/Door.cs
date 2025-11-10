using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class Door : MonoBehaviour, IPointerClickHandler
{
    [Header("Destination")]
    public World destination;   //목적지 설정
    public int position;
    private ChatTrigger _blockChat;   // Block일시 출력할 대사
    
    private void Awake()
    {
        _blockChat = GetComponent<ChatTrigger>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!WorldSceneManager.Instance)
        {
            throw new Exception($"World Manager Missed!! : Door");
        }
        
        // 지역이동 제한
        if (WorldSceneManager.Instance.MoveLocation(destination, position) is false)
        {
            // 이동 제한 텍스트 출력
            _blockChat?.StartChat();
        }
    }
}
