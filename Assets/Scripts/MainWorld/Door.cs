using System;
using UnityEngine;

public class Door : MonoBehaviour
{
    /**
     * 클릭시 지정된 Location으로 이동
     * - 월드 매니저에 이동 호출
     */
    
    protected bool Interactable = true;      // 버튼 활성화 상태
    
    [Header("Destination")]
    public World destination = World.Street;   //목적지 설정
    public int position;
    
    private ChatTrigger _blockChat;   // Block일시 출력할 대사


    private void Awake()
    {
        _blockChat = GetComponent<ChatTrigger>();
    }
    
    /// <summary>
    /// 버튼 활성화 
    /// </summary>
    /// <param name="active">활성화/비활성화 여부</param>
    public void SetActive(bool active)
    {
        if (Interactable)
        {
            gameObject.SetActive(active);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    
    /// <summary>
    /// 지역 이동
    /// </summary>
    /// <returns>성공 여부 (Block시 실패)</returns>
    /// <exception cref="Exception">월드매니저 오류</exception>
    public virtual void OnClick()
    {
        if (WorldSceneManager.Instance == null)
        {
            throw new Exception($"World Manager Missed!! : Door");
        }
        
        // 지역이동 제한
        if (WorldSceneManager.Instance.MoveLocation(destination, position) is false)
        {
            Interactable = false;
            
            // 이동 제한 텍스트 출력
            _blockChat?.StartChat();
            
            SetActive(false);
        }
    }
}
