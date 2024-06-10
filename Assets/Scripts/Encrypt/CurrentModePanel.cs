using System;
using UnityEngine;

public class CurrentModePanel : MonoBehaviour
{
    private ADFGVXGameManager GameManager { get; set; }
    
    private BasicText Title { get; set; }
    private BasicButton Button { get; set; }

    private void Awake()
    {
        GameManager = FindObjectOfType<ADFGVXGameManager>();
        Title = transform.GetChild(0).GetComponent<BasicText>();
        Button = transform.GetChild(1).GetComponent<BasicButton>();
    }

    public void SwitchSystem()
    {
        Title.TextTMP.text = GameManager.CurrentSystemMode == ADFGVXGameManager.SystemMode.Decryption
            ? "암호화 모드" : "복호화 모드";
    }
    
    /// <summary>
    /// 일정 시간 동안 입력을 차단한다
    /// </summary>
    /// <param name="wait"> 대기 시간 </param>
    /// <param name="duration"> 차단 시간 </param>
    public void CutAvailabilityInputForWhile(float wait, float duration)
    {
        Button.CutAvailabilityForWhile(wait, duration);
    }
    
    /// <summary>
    /// 플레이어 입력 가능 여부를 설정한다
    /// </summary>
    /// <param name="value"> 가능 여부 </param>
    public void SetAvailable(bool value)
    {
        Button.SetAvailability(value);
    }
}
