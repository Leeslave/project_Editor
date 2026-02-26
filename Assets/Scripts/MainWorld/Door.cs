using GameService;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using Utility;

public class Door : MonoBehaviour, IPointerClickHandler
{
    [Header("Destination")]
    public World destination;   //목적지 설정
    public int position;
    private ChatTrigger _blockChat;   // Block일시 출력할 대사
    
    private ILocationService _locationService;
    
    private void Awake()
    {
        _blockChat = GetComponent<ChatTrigger>();
    }

    private void Start()
    {
        ServiceProvider.Get<ILocationService>(service => _locationService = service);
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        if (_locationService == null)
        {
            EditorLogger.LogWarning($"World Manager Missed!! : Door");
        }
        
        // 지역이동 제한
        if (_locationService != null && _locationService.MoveLocation(new WorldVector(destination, position)) == null)
        {
            // 이동 제한 텍스트 출력
            _blockChat?.StartChat();
        }
    }
}
