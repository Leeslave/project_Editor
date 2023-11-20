using System;
using System.Collections;
using UnityEngine;
using System.IO;

public class LoadEncrypted_ForGame : MonoBehaviour
{
    private ADFGVXSceneManager_ForGame SceneManager;
    
    public BasicText Title { get; set; }
    public BasicInputField EncryptedTextTitle { get; set; }
    public BasicText EncryptedTextBody { get; set; }
    public BasicText EncryptedTextWriter { get; set; }
    public BasicText EncryptedTextDate { get; set; }

    private void Awake()
    {
        SceneManager = FindObjectOfType<ADFGVXSceneManager_ForGame>();
        
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
    public IEnumerator CutOffInputForWhile(float wait, float duration)
    {
        yield return new WaitForSeconds(wait);
        EncryptedTextTitle.SetAvailable(false);
        yield return new WaitForSeconds(duration);
        EncryptedTextTitle.SetAvailable(true);
    }
    
    /// <summary>
    /// 플레이어 입력 가능 여부를 설정한다
    /// </summary>
    /// <param name="value"> 가능 여부 </param>
    public void SetAvailable(bool value)
    {
        EncryptedTextTitle.SetAvailable(value);
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
            LJWConverter.Instance.PrintTMPByDuration(false, 0f, 1.5f, "Decrypted-" + EncryptedTextTitle.StringBuffer, true, SceneManager.DisplayDecrypted.DecryptedTextTitle.TextTMP);
            SceneManager.CutOffInputForWhile(0f, 1.5f);
            reader.Close();   
        }
        else
        {
            LJWConverter.Instance.PrintTMPByDuration(false, 0f, 1f, "잘못된 파일 이름을 입력했습니다.", true, EncryptedTextBody.TextTMP);
        }
    }


}
