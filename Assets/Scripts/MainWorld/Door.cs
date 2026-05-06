using GameService;
using UnityEngine;
using UnityEngine.EventSystems;

public class Door : MonoBehaviour, IPointerClickHandler
{
    [Header("Destination")]
    public World destination;   //목적지 설정
    public int position;
    private ChatTrigger _blockChat;   // Block일시 출력할 대사
    private WorldVector _dest;
    
    private static ILocationService LocationService => GameSystem.GetService<ILocationService>();
    private bool IsBlocked => LocationService.IsBlocked(new WorldVector(destination, position));
    
    private void Awake()
    {
        _blockChat = GetComponent<ChatTrigger>();
        _dest = new WorldVector(destination, position);
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        // 지역이동 제한
        if (IsBlocked)
        {
            // 이동 제한 텍스트 출력
            _blockChat?.StartChat();
            return;
        }
        
        LocationService.MoveLocation(_dest);
    }
}
