using System;
using System.Collections;
using UnityEngine;
using System.IO;
using Unity.VisualScripting;

public class LoadEncrypted : MonoBehaviour
{
    private ADFGVXGameManager _gameManager;
    
    public BasicText Title { get; set; }
    public BasicInputField EncryptedTextTitle { get; set; }
    public BasicText EncryptedTextBody { get; set; }
    public BasicText EncryptedTextWriter { get; set; }
    public BasicText EncryptedTextDate { get; set; }

    private void Awake()
    {
        _gameManager = FindObjectOfType<ADFGVXGameManager>();
        
        Title = this.transform.GetChild(0).GetComponent<BasicText>();
        EncryptedTextTitle = this.transform.GetChild(1).GetComponent<BasicInputField>();
        EncryptedTextBody = this.transform.GetChild(2).GetComponent<BasicText>();
        EncryptedTextWriter = this.transform.GetChild(3).GetComponent<BasicText>();
        EncryptedTextDate = this.transform.GetChild(4).GetComponent<BasicText>();
        
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
    
    /// <summary>
    /// 
    /// </summary>
    public void LoadEncryptedText()
    {
        var filePath = "Assets/Resources/GameData/Encrypt/Encrypted/" + EncryptedTextTitle.StringBuffer + ".txt";
        if (new FileInfo(filePath).Exists)
        {
            var reader = new StreamReader(filePath, System.Text.Encoding.UTF8);
            LJWConverter.Instance.PrintTMPByDuration(false, 0f, 1.5f, reader.ReadLine(), true, EncryptedTextBody.TextTMP);
            LJWConverter.Instance.PrintTMPByDuration(false, 0f, 1.5f, reader.ReadLine(), true, EncryptedTextWriter.TextTMP);
            LJWConverter.Instance.PrintTMPByDuration(false, 0f, 1.5f, reader.ReadLine(), true, EncryptedTextDate.TextTMP);
            LJWConverter.Instance.PrintTMPByDuration(false, 0f, 1.5f, "Decrypted-" + EncryptedTextTitle.StringBuffer, true, _gameManager.DisplayDecrypted.DecryptedTextTitle.TextTMP);
            _gameManager.CutAvailabilityInputForWhile(0f, 1.5f);
            reader.Close();   
        }
        else
        {
            LJWConverter.Instance.PrintTMPByDuration(false, 0f, 1f, "잘못된 파일 이름을 입력했습니다.", true, EncryptedTextBody.TextTMP);
        }
    }


}
