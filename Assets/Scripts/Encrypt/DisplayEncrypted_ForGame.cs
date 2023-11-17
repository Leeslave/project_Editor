using System.Collections;
using System.IO;
using UnityEngine;

public class DisplayEncrypted_ForGame : MonoBehaviour
{
    public ADFGVXSceneManager_ForGame SceneManager { get; set; }
    
    public BasicText Title { get; set; }
    public BasicInputField EncryptedTextTitle { get; set; }
    public BasicInputField EncryptedTextBody { get; set; }
    public BasicText EncryptedTextWriter { get; set; }
    public BasicText EncryptedTextDate { get; set; }
    public BasicButton SaveButton { get; set; }

    private void Awake()
    {
        SceneManager = GameObject.FindObjectOfType<ADFGVXSceneManager_ForGame>();
        
        Title = this.transform.GetChild(0).GetComponent<BasicText>();
        EncryptedTextTitle = this.transform.GetChild(1).GetComponent<BasicInputField>();
        EncryptedTextBody = this.transform.GetChild(2).GetComponent<BasicInputField>();
        EncryptedTextWriter = this.transform.GetChild(3).GetComponent<BasicText>();
        EncryptedTextDate = this.transform.GetChild(4).GetComponent<BasicText>();
        SaveButton = this.transform.GetChild(5).GetComponent<BasicButton>();
        
        Initialize();
    }

    /// <summary>
    /// 
    /// </summary>
    public void Initialize()
    {
        EncryptedTextTitle.Initialize();
        EncryptedTextBody.Initialize();
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
        EncryptedTextBody.SetAvailable(false);
        SaveButton.SetAvailable(false);
        yield return new WaitForSeconds(duration);
        EncryptedTextTitle.SetAvailable(true);
        EncryptedTextBody.SetAvailable(true);
        SaveButton.SetAvailable(true);
    }
    
    /// <summary>
    /// 플레이어 입력 가능 여부를 설정한다
    /// </summary>
    /// <param name="value"> 가능 여부 </param>
    public void SetAvailable(bool value)
    {
        EncryptedTextTitle.SetAvailable(value);
        EncryptedTextBody.SetAvailable(value);
        SaveButton.SetAvailable(value);
    }

    /// <summary>
    /// 암호호된 텍스트를 파일로 저장한다
    /// </summary>
    public void SaveEncryptedTextFile()
    {
        //암호화 연출을 띄우고 성공 실패 여부에 따라서 리턴
        if (!SceneManager.ResultPanel.PrintEncryptionResult())
            return;
        
        var filePath = "Assets/Resources/Encrypted/" + EncryptedTextTitle.StringBuffer + ".txt";
        if(!new FileInfo(filePath).Exists)
        {
            StreamWriter writer = File.CreateText(filePath);
            writer.WriteLine(EncryptedTextBody.StringBuffer);
            writer.WriteLine(EncryptedTextWriter.TextTMP.text);
            writer.WriteLine(EncryptedTextDate.TextTMP.text);
            writer.Close();
        }
        else
        {
            Debug.Log("이미 같은 이름의 파일이 존재합니다!");
        }
    }
}
