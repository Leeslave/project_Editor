using GameService;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using Utility;

public class Door : MonoBehaviour, IPointerClickHandler
{
    private LocationManager locationManager;
    
    [Header("Destination")]
    public World destination;   //목적지 설정
    public int position;
    private ChatTrigger _blockChat;   // Block일시 출력할 대사
    
    private void Awake()
    {
        _blockChat = GetComponent<ChatTrigger>();
        locationManager = LocationManager.Instance;
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        if (!locationManager)
        {
            EditorLogger.LogWarning($"World Manager Missed!! : Door");
        }
        
        // 지역이동 제한
        if (locationManager.MoveLocation(new WorldVector(destination, position)))
        {
            // 이동 제한 텍스트 출력
            _blockChat?.StartChat();
        }
    }
}
