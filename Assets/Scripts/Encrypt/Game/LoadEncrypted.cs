using System;
using System.Collections;
using UnityEngine;
using System.IO;
using Unity.VisualScripting;

public class LoadEncrypted : MonoBehaviour
{
    public BasicText Title { get; set; }
    private BasicInputField EncryptedTextTitle { get; set; }
    public BasicText EncryptedTextBody { get; set; }
    private BasicText EncryptedTextWriter { get; set; }
    private BasicText EncryptedTextDate { get; set; }
    private BasicText PrimeNumDisplay { get; set; }

    public Action CompleteTutorialPhase = null;

    private void Awake()
    {
        Title = transform.GetChild(0).GetComponent<BasicText>();
        EncryptedTextTitle = transform.GetChild(1).GetComponent<BasicInputField>();
        EncryptedTextBody = transform.GetChild(2).GetComponent<BasicText>();
        EncryptedTextWriter = transform.GetChild(3).GetComponent<BasicText>();
        EncryptedTextDate = transform.GetChild(4).GetComponent<BasicText>();
        PrimeNumDisplay = transform.GetChild(5).GetComponent<BasicText>();
        
        Init();
    }
    
    public void Init()
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
        if (ADFGVXGameManager.Instance.decryptTargetText != EncryptedTextTitle.StringBuffer)
        {
            LJWConverter.Instance.PrintTMPByDuration(false, 0f, 1f, "잘못된 파일 이름을 입력했습니다.", true, EncryptedTextBody.TextTMP);
            return;
        }

        string title = EncryptedTextTitle.StringBuffer;
        string filePath = Application.dataPath + "/Resources/GameData/Encrypt/Encrypted/" + title + ".txt";
        if (new FileInfo(filePath).Exists)
        {
            //튜토리얼 전용
            if (ADFGVXGameManager.ADFGVXTutorialManager.IsDecryptPlaying())
            {
                if(title == "SONG-OF-YESTERDAY")
                    ADFGVXGameManager.ADFGVXTutorialManager.MoveToNextTutorialPhase(4f);
                else
                {
                    LJWConverter.Instance.PrintTMPByDuration(false, 0f, 1f, "튜토리얼 조건에 맞는 파일 이름을 입력해야합니다.", true, EncryptedTextBody.TextTMP);
                    return;
                }
            }
            
            //로드
            StreamReader reader = new(filePath, System.Text.Encoding.UTF8);
            var encryptedText = reader.ReadLine();
            LJWConverter.Instance.PrintTMPByDuration(false, 0f, 1f, encryptedText, true, EncryptedTextBody.TextTMP);
            LJWConverter.Instance.PrintTMPByDuration(false, 0f, 1f, reader.ReadLine(), true, EncryptedTextWriter.TextTMP);
            LJWConverter.Instance.PrintTMPByDuration(false, 0f, 1f, reader.ReadLine(), true, EncryptedTextDate.TextTMP);
            LJWConverter.Instance.PrintTMPByDuration(false, 0f, 1f, "Decrypted-" + EncryptedTextTitle.StringBuffer, true, ADFGVXGameManager.DisplayDecrypted.DecryptedTextTitle.TextTMP);

            //암호 키 추천
            CalculateKeyLength(encryptedText);
            
            //새로운 암호문을 로드하였으므로 전에 작업 내용은 파기
            ADFGVXGameManager.KeyPriorityTranspose.Initialize();
            ADFGVXGameManager.BilateralSubstitute.Initialize();
            
            //출력하는 동안 차단
            ADFGVXGameManager.CutAvailabilityInputForWhile(0f, 2f);
            reader.Close();   
        }
    }

    private void CalculateKeyLength(string value)
    {
        var length = value.Replace(" ", "").Length;
        
        if (length <= 1)
            PrimeNumDisplay.TextTMP.text = "사용 가능한 복호키 길이: NULL";
        else
        {
            var result = "사용 가능한 복호키 길이: ";
            for (var i = 2; i <= 9; i++)
                if (length % i == 0)
                    result += i + ", ";
            PrimeNumDisplay.TextTMP.text = result.Substring(0, result.Length - 2);
        }
    }
}
