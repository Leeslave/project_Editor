using System;
using System.Collections;
using UnityEngine;
using System.IO;
using Unity.VisualScripting;

public class LoadEncrypted : MonoBehaviour
{
    private ADFGVXGameManager GameManager { get; set; }
    
    public BasicText Title { get; set; }
    private BasicInputField EncryptedTextTitle { get; set; }
    public BasicText EncryptedTextBody { get; set; }
    private BasicText EncryptedTextWriter { get; set; }
    private BasicText EncryptedTextDate { get; set; }
    private BasicText PrimeNumDisplay { get; set; }

    private void Awake()
    {
        GameManager = FindObjectOfType<ADFGVXGameManager>();
        
        Title = transform.GetChild(0).GetComponent<BasicText>();
        EncryptedTextTitle = transform.GetChild(1).GetComponent<BasicInputField>();
        EncryptedTextBody = transform.GetChild(2).GetComponent<BasicText>();
        EncryptedTextWriter = transform.GetChild(3).GetComponent<BasicText>();
        EncryptedTextDate = transform.GetChild(4).GetComponent<BasicText>();
        PrimeNumDisplay = transform.GetChild(5).GetComponent<BasicText>();
        
        Initialize();
    }

    /// <summary>
    /// 
    /// </summary>
    public void Initialize()
    {
        EncryptedTextTitle.Initialize();
        EncryptedTextBody.TextTMP.text = "";
        EncryptedTextWriter.TextTMP.text = "작성자: NULL";
        EncryptedTextDate.TextTMP.text = "작성자: NULL";
        PrimeNumDisplay.TextTMP.text = "사용 가능한 암호키 길이: NULL";
    }

    /// <summary>
    /// 일정 시간 동안 입력을 차단한다
    /// </summary>
    /// <param name="wait"> 대기 시간 </param>
    /// <param name="duration"> 차단 시간 </param>
    public void CutAvailabilityInputForWhile(float wait, float duration)
    {
        EncryptedTextTitle.CutAvailabilityForWhile(wait, duration);
    }
    
    /// <summary>
    /// 플레이어 입력 가능 여부를 설정한다
    /// </summary>
    /// <param name="value"> 가능 여부 </param>
    public void SetAvailable(bool value)
    {
        EncryptedTextTitle.SetAvailability(value);
    }

    public void LoadEncryptedText()
    {
        var filePath = "Assets/Resources/GameData/Encrypt/Encrypted/" + EncryptedTextTitle.StringBuffer + ".txt";
        if (new FileInfo(filePath).Exists)
        {
            var reader = new StreamReader(filePath, System.Text.Encoding.UTF8);
            LJWConverter.Instance.PrintTMPByDuration(false, 0f, 1.5f, reader.ReadLine(), true, EncryptedTextBody.TextTMP);
            LJWConverter.Instance.PrintTMPByDuration(false, 0f, 1.5f, reader.ReadLine(), true, EncryptedTextWriter.TextTMP);
            LJWConverter.Instance.PrintTMPByDuration(false, 0f, 1.5f, reader.ReadLine(), true, EncryptedTextDate.TextTMP);
            LJWConverter.Instance.PrintTMPByDuration(false, 0f, 1.5f, "Decrypted-" + EncryptedTextTitle.StringBuffer, true, GameManager.DisplayDecrypted.DecryptedTextTitle.TextTMP);

            //암호 키 추천
            CalculateKeyLength();
            
            //새로운 암호문을 로드하였으므로 전에 작업 내용은 파기
            GameManager.KeyPriorityTranspose.Initialize();
            GameManager.BilateralSubstitute.Initialize();
            GameManager.DisplayDecrypted.Initialize();
            
            //출력하는 동안 차단
            GameManager.CutAvailabilityInputForWhile(0f, 1.5f);
            reader.Close();   
        }
        else
        {
            LJWConverter.Instance.PrintTMPByDuration(false, 0f, 1f, "잘못된 파일 이름을 입력했습니다.", true, EncryptedTextBody.TextTMP);
        }
    }

    private void CalculateKeyLength()
    {
        var length = EncryptedTextBody.TextTMP.text.Replace(" ", "").Length;
        
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
