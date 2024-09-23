using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DisplayEncrypted : MonoBehaviour
{
    public BasicText Title { get; set; }
    public BasicText EncryptedTextTitle { get; set; }
    public BasicInputField EncryptedTextBody { get; set; }
    public BasicText EncryptedTextWriter { get; set; }
    public BasicText EncryptedTextDate { get; set; }
    public BasicButton SaveButton { get; set; }

    private void Awake()
    {
        Title = transform.GetChild(0).GetComponent<BasicText>();
        EncryptedTextTitle = transform.GetChild(1).GetComponent<BasicText>();
        EncryptedTextBody = transform.GetChild(2).GetComponent<BasicInputField>();
        EncryptedTextWriter = transform.GetChild(3).GetComponent<BasicText>();
        EncryptedTextDate = transform.GetChild(4).GetComponent<BasicText>();
        SaveButton = transform.GetChild(5).GetComponent<BasicButton>();
    }
    private void Start()
    {
        Init();
    }
    public void Init()
    {
        EncryptedTextBody.Init();
    }

    /// <summary>
    /// 일정 시간 동안 입력을 차단한다
    /// </summary>
    /// <param name="wait"> 대기 시간 </param>
    /// <param name="duration"> 차단 시간 </param>
    public void CutAvailabilityInputForWhile(float wait, float duration)
    {
        EncryptedTextBody.CutAvailabilityForWhile(wait, duration);
        SaveButton.CutAvailabilityForWhile(wait, duration);
    }
    
    /// <summary>
    /// 플레이어 입력 가능 여부를 설정한다
    /// </summary>
    /// <param name="value"> 가능 여부 </param>
    public void SetAvailable(bool value)
    {
        EncryptedTextBody.SetAvailability(value);
        SaveButton.SetAvailability(value);
    }

    /// <summary>
    /// 암호호된 텍스트를 파일로 저장한다
    /// </summary>
    public void SaveEncryptedTextFile()
    {
        //튜토리얼 전용
        if (ADFGVXGameManager.ADFGVXTutorialManager.IsEncryptPlaying())
        {
            if (EncryptedTextBody.StringBuffer.Replace(" ", "") == ADFGVXGameManager.Instance.encryptResultText)
                ADFGVXGameManager.ADFGVXTutorialManager.MoveToNextTutorialPhase(6.62f + 2f);
            else
                return;
        }
        
        //암호화 연출을 띄우고 성공 실패 여부에 따라서 리턴
        if (!ADFGVXGameManager.ResultPanel.PrintEncryptionResult())
            return;
        
        var filePath = Application.dataPath + "/Resources/GameData/Encrypt/Encrypted/" + EncryptedTextTitle.TextTMP + ".txt";
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
    
    public void ShowNextTarget()
    {
        LJWConverter.Instance.GradientSpriteRendererColor(false, 0f, 0.5f, new Color(1f, 0.2f, 0.2f, 0.2f), ADFGVXGameManager.KeyPriorityTranspose.TargetFill);
        LJWConverter.Instance.GradientSpriteRendererColor(false, 0f, 0.5f, new Color(1f, 0.2f, 0.2f, 1f), ADFGVXGameManager.KeyPriorityTranspose.TargetFrame);
        CalculateNextTarget();
    }
    public void HideNextTarget()
    {
        LJWConverter.Instance.GradientSpriteRendererColor(false, 0f, 0.5f, new Color(1f, 0.2f, 0.2f, 0f), ADFGVXGameManager.KeyPriorityTranspose.TargetFill);
        LJWConverter.Instance.GradientSpriteRendererColor(false, 0f, 0.5f, new Color(1f, 0.2f, 0.2f, 0f), ADFGVXGameManager.KeyPriorityTranspose.TargetFrame);
    }
    public void CalculateNextTarget()
    {
        if (ADFGVXGameManager.KeyPriorityTranspose.KeyInputField.StringBuffer.Length == 0)
            return;
        if (ADFGVXGameManager.KeyPriorityTranspose.ReverseTransposeLines.StringBuffer.Length == 0)
            return;

        EncryptedTextBody.InputFieldTMP.color = EncryptedTextBody.StringBuffer.Replace(" ", "") == ADFGVXGameManager.Instance.encryptResultText
            ? new Color(0.2f, 1f, 1f, 1f)
            : new Color(1f, 0.2f, 0.2f, 1f);
        
        int line = ADFGVXGameManager.KeyPriorityTranspose.KeyInputField.StringBuffer.Length;
        int row = ADFGVXGameManager.KeyPriorityTranspose.ReverseTransposeLines.StringBuffer.Length / line;
        int lineProgress = EncryptedTextBody.StringBuffer.Replace(" ", "").Length / row;
        int rowProgress = EncryptedTextBody.StringBuffer.Replace(" ", "").Length % row;
        if (lineProgress < line)
        {
            ADFGVXGameManager.KeyPriorityTranspose.TargetFill.color = new Color(1f, 0.2f, 0.2f, 0.2f);
            ADFGVXGameManager.KeyPriorityTranspose.TargetFrame.color = new Color(1f, 0.2f, 0.2f, 1f);
        }
        else if (lineProgress == line)
        {
            ADFGVXGameManager.KeyPriorityTranspose.TargetFill.color = new Color(0.2f, 1f, 1f, 0.2f);
            ADFGVXGameManager.KeyPriorityTranspose.TargetFrame.color = new Color(0.2f, 1f, 1f, 1f);
            return;
        }
        Dictionary<int, char> code = new()
        {
            { 1, '1' }, { 2, '2' }, { 3, '3' }, { 4, '4' }, { 5, '5' },
            { 6, '6' }, { 7, '7' }, { 8, '8' }, { 9, '9' },
        };
        string priority = ADFGVXGameManager.KeyPriorityTranspose.KeyPriority.TextTMP.text.Replace(" ", "");
        int gridX = priority.IndexOf(code[lineProgress + 1]);
        int gridY = rowProgress;
        float posX = 2.25f + gridX * 3.15f;
        float posY = -18.25f + gridY * -3.6f;
        ADFGVXGameManager.KeyPriorityTranspose.TargetFill.transform.localPosition = new Vector3(posX, posY, 0f);
        ADFGVXGameManager.KeyPriorityTranspose.TargetFrame.transform.localPosition = new Vector3(posX, posY, 0f);
    }
}
