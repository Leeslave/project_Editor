using System.Collections;
using UnityEngine;

public class WritePlain : MonoBehaviour
{ 
    
    
    public BasicText Title { get; set; }
    public BasicInputField PlainTextBody { get; set; }
    public BasicText PrimeNumDisplay { get; set; }
    public int[] PrimeNumArray { get; set; } = new int[10];

    private void Awake()
    {
        Title = this.transform.GetChild(0).GetComponent<BasicText>();
        PlainTextBody = this.transform.GetChild(1).GetComponent<BasicInputField>();
        PrimeNumDisplay = this.transform.GetChild(2).GetComponent<BasicText>();
        
        Initialize();
    }

    public void Initialize()
    {
        PlainTextBody.Initialize();
        PrimeNumDisplay.TextTMP.text = "사용 가능한 암호키 길이: NULL";
    }

    /// <summary>
    /// 일정 시간 동안 입력을 차단한다
    /// </summary>
    /// <param name="wait"> 대기 시간 </param>
    /// <param name="duration"> 차단 시간 </param>
    public void CutAvailabilityInputForWhile(float wait, float duration)
    {
        PlainTextBody.CutAvailabilityForWhile(wait, duration);
    }

    /// <summary>
    /// 플레이어 입력 가능 여부를 설정한다
    /// </summary>
    /// <param name="value"> 가능 여부 </param>
    public void SetAvailable(bool value)
    {
        PlainTextBody.SetAvailability(value);
    }
    
    public void CalculateKeyLength()
    {
        var length = PlainTextBody.StringBuffer.Length;

        if (length <= 1)
        {
            PrimeNumDisplay.TextTMP.text = "사용 가능한 암호키 길이: NULL";
            
        }
        else
        {
            var result = "사용 가능한 암호키 길이: ";
            for (var i = 2; i <= 9; i++)
                if (2 * length % i == 0)
                    result += i.ToString() + ", ";
            PrimeNumDisplay.TextTMP.text = result.Substring(0, result.Length - 2);
        }
    }
}