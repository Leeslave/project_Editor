using System;
using System.Collections;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;

public class DisplayDecrypted : MonoBehaviour
{
    public ADFGVXGameManager GameManager { get; set; }

    public BasicText Title { get; set; }
    public BasicText DecryptedTextTitle { get; set; }
    public BasicInputField DecryptedTextBody { get; set; }
    public BasicText DecryptedTextWriter { get; set; }
    public BasicText DecryptedTextDate { get; set; }
    public BasicButton SaveButton { get; set; }
    

    private void Awake()
    {
        GameManager = FindObjectOfType<ADFGVXGameManager>();

        Title = this.transform.GetChild(0).GetComponent<BasicText>();
        DecryptedTextTitle = this.transform.GetChild(1).GetComponent<BasicText>();
        DecryptedTextBody = this.transform.GetChild(2).GetComponent<BasicInputField>();
        DecryptedTextWriter = this.transform.GetChild(3).GetComponent<BasicText>();
        DecryptedTextDate = this.transform.GetChild(4).GetComponent<BasicText>();
        SaveButton = this.transform.GetChild(5).GetComponent<BasicButton>();
        
        DecryptedTextWriter.TextTMP.text = "작성자: ANONYMOUS";
        DecryptedTextDate.TextTMP.text = "작성일: " + DateTime.Today.Year + "년 " + DateTime.Today.Month + "월 " + DateTime.Today.Day + "일";
        
        Initialize();
    }
    
    public void Initialize()
    {
        DecryptedTextBody.Initialize();
        DecryptedTextTitle.TextTMP.text = "저장할 복호화 파일 이름";
        DecryptedTextWriter.TextTMP.text = "작성자: ANONYMOUS";
        DecryptedTextDate.TextTMP.text = "작성일: " + DateTime.Today.Year + "년 " + DateTime.Today.Month + "월 " + DateTime.Today.Day + "일";
    }

    /// <summary>
    /// 일정 시간 동안 입력을 차단한다
    /// </summary>
    /// <param name="wait"> 대기 시간 </param>
    /// <param name="duration"> 차단 시간 </param>
    public void CutAvailabilityInputForWhile(float wait, float duration)
    {
        DecryptedTextBody.CutAvailabilityForWhile(wait, duration);
        SaveButton.CutAvailabilityForWhile(wait, duration);
    }
    
    /// <summary>
    /// 플레이어 입력 가능 여부를 설정한다
    /// </summary>
    /// <param name="value"> 가능 여부 </param>
    public void SetAvailable(bool value)
    {
        DecryptedTextBody.SetAvailability(value);
        SaveButton.SetAvailability(value);
    }
    
    /// <summary>
    /// 
    /// </summary>
    public void SaveDecryptedTextFile()
    {
        //복호화 연출을 띄우고 성공 실패 여부에 따라서 리턴
        if (!GameManager.ResultPanel.PrintDecryptionResult())
            return;
        
        var filePath = "Assets/Resources/GameData/Encrypt/Decrypted/" + DecryptedTextTitle.TextTMP.text + ".txt";
        if(!new FileInfo(filePath).Exists)
        {
            StreamWriter writer = File.CreateText(filePath);
            writer.WriteLine(DecryptedTextBody.StringBuffer);
            writer.WriteLine(DecryptedTextWriter.TextTMP.text);
            writer.WriteLine(DecryptedTextDate.TextTMP.text);
            writer.Close();
        }
        else
        {
            Debug.Log("이미 같은 이름의 파일이 존재합니다!");
        }   
    }
}
