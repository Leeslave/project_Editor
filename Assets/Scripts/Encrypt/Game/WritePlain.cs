using System.Collections;
using Unity.VisualScripting;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;

public class WritePlain : MonoBehaviour
{ 
    public BasicText Title { get; set; }
    public BasicInputField PlainTextBody { get; set; }
    private BasicText PrimeNumDisplay { get; set; }

    private void Awake()
    {
        Title = transform.GetChild(0).GetComponent<BasicText>();
        PlainTextBody = transform.GetChild(1).GetComponent<BasicInputField>();
        PrimeNumDisplay = transform.GetChild(2).GetComponent<BasicText>();
        
        Initialize();
    }

    public void Initialize()
    {
        PlainTextBody.Initialize();
        PlainTextBody.InputFieldTMP.color = new Color(1f, 1f, 1f, 1f);
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

        if (length <= 0)
        {
            PlainTextBody.InputFieldTMP.color = Color.white;
            PrimeNumDisplay.TextTMP.text = "사용 가능한 암호키 길이: NULL";
            return;
        }
        
        PlainTextBody.InputFieldTMP.color = PlainTextBody.StringBuffer != ADFGVXGameManager.Instance.encryptTargetText 
            ? new Color(1f, 0.17f, 0.17f, 1f) 
            : new Color(0.17f, 1f, 1f, 1f);
        
        if (length <= 1)
            PrimeNumDisplay.TextTMP.text = "사용 가능한 암호키 길이: NULL";
        else
        {
            var result = "사용 가능한 암호키 길이: ";
            for (var i = 2; i <= 9; i++)
                if (length % i == 0)
                    result += i + ", ";
            PrimeNumDisplay.TextTMP.text = result.Substring(0, result.Length - 2);
        }
    }
}