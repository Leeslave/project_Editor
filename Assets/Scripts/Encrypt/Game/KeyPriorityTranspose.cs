using System.Collections;
using UnityEngine;
using TMPro;

public class KeyPriorityTranspose : MonoBehaviour
{
    public BasicText Title { get; set; }
    public BasicInputField KeyInputField { get; set; }
    public BasicText KeyPriority { get; set; }
    public TextMeshPro[] TransposeLines { get; set; }
    public BasicInputField ReverseTransposeLines { get; set; }
    public SpriteRenderer TargetFill { get; set; }
    public SpriteRenderer TargetFrame { get; set; }

    //키 순위 전치 관련 변수
    public int[] Priority { get; set; } = new int[9];
    public int RowLength { get; set; }
    public int LineLength { get; set; }
    public string[] TempLine { get; set; } = new string[9];
    public string CurrentTransposedText { get; set; } = "";

    private void Awake()
    {
        Title = transform.GetChild(0).GetComponent<BasicText>();
        KeyInputField = transform.GetChild(1).GetComponent<BasicInputField>();
        KeyPriority = transform.GetChild(2).GetComponent<BasicText>();
        TransposeLines = new TextMeshPro[9];
        for(var i = 0; i < 9; i++)
        {
            TransposeLines[i] = transform.GetChild(3).transform.GetChild(i).GetComponent<TextMeshPro>();
            TransposeLines[i].text = "";
        }
        ReverseTransposeLines = transform.GetChild(4).GetComponent<BasicInputField>();
        TargetFill = transform.GetChild(5).GetComponent<SpriteRenderer>();
        TargetFrame = transform.GetChild(6).GetComponent<SpriteRenderer>();
    }
    private void Start() => Init();

    /// <summary>
    /// 모드가 전환될 때마다 버퍼에 있는 내용을 깨끗하게 비우고 초기화해야 한다
    /// </summary>
    public void Init() {
        KeyInputField.Init();
        KeyPriority.TextTMP.text = "_ _ _ _ _ _ _ _ _";
        KeyInputField.InputFieldTMP.color = new Color(1f, 1f, 1f, 1f);
        KeyPriority.TextTMP.color = new Color(1f, 1f, 1f, 1f);
        
        KeyInputField.CheckInputField();
        for (int i = 0; i < 9; i++)
        {
            TempLine[i] = "";
            TransposeLines[i].text = "";
        }
        
        ReverseTransposeLines.Init();
        ReverseTransposeLines.SetAvailability(ADFGVXGameManager.CurrentSystemMode != ADFGVXGameManager.SystemMode.Decryption);
    }
    
    /// <summary>
    /// 플레이어 입력 가능 여부를 설정한다
    /// </summary>
    /// <param name="value"> 가능 여부 </param>
    public void SetAvailable(bool value)
    {
        KeyInputField.SetAvailability(value);
        ReverseTransposeLines.SetAvailability(value);
    }
    
    /// <summary>
    /// 키 값이 변경되었다면 새롭게 키 순위를 계산해야 한다
    /// </summary>
    public void UpdateKeyPriority()
    {
        string inputKey = KeyInputField.StringBuffer;
        
        //키 순서를 결정하는 알고리즘
        for(int i = 0; i < inputKey.Length; i++)
                Priority[i] = 1;
        for (int i = 0; i < inputKey.Length; i++)
            for (int j = 0; j < inputKey.Length; j++)
            {
                if (i == j) 
                    continue;
                if (inputKey[i] > inputKey[j])
                    Priority[i]++;
                if (inputKey[i] == inputKey[j] && i < j)
                    Priority[j]++;
            }
        
        //키 순위 디스플레이
        string result = "";
        for (int i = 0; i < KeyInputField.StringBuffer.Length; i++)
            result += Priority[i] + " ";
        for (int i = KeyInputField.StringBuffer.Length; i < 9; i++)
            result += "_ ";
        KeyPriority.TextTMP.text = result;
        
        //행렬 디스플레이 초기화
        for (int i = 0; i < 9; i++)
        {
            TempLine[i] = "";
            TransposeLines[i].text = "";
        }
        
        //전치 키 확인
        if (inputKey.Length == 0)
        {
            KeyInputField.InputFieldTMP.color = new Color(1f, 1f, 1f, 1f);
            KeyPriority.TextTMP.color = new Color(1f, 1f, 1f, 1f);
            return;
        }

        if (ADFGVXGameManager.CurrentSystemMode == ADFGVXGameManager.SystemMode.Encryption)
        {
            //키 값이 정확할 경우
            Color color = ADFGVXGameManager.Instance.encryptTransposeKey == inputKey
                ? new Color(0.2f, 1f, 1f, 1f)
                : new Color(1f, 0.2f, 0.2f, 1f);
            KeyInputField.InputFieldTMP.color = color;
            KeyPriority.TextTMP.color = color;
            
            //전치 입력이 정확할 경우
            color = ADFGVXGameManager.Instance.encryptTransposeText == ReverseTransposeLines.StringBuffer
                ? new Color(0.2f, 1f, 1f, 1f)
                : new Color(1f, 0.2f, 0.2f, 1f);
            ReverseTransposeLines.InputFieldTMP.color = color;
            ReverseTransposeLines.InputFieldTMP.rectTransform.sizeDelta = new Vector2(3f * inputKey.Length, 80);
        }
        else
        {
            //키 값이 정확할 경우
            Color color = ADFGVXGameManager.Instance.decryptTransposeKey == inputKey
                ? new Color(0.2f, 1f, 1f, 1f)
                : new Color(1f, 0.2f, 0.2f, 1f);
            KeyInputField.InputFieldTMP.color = color;
            KeyPriority.TextTMP.color = color;
            
            foreach(var i in TransposeLines)
                i.color = inputKey == ADFGVXGameManager.Instance.decryptTransposeKey
                    ? new Color(0.2f, 1f, 1f, 1f)
                    : new Color(1f, 0.2f, 0.2f, 1f);
        }
    }

    /// <summary>
    /// 암호화되어 있는 데이터를 키 순위에 따라서 전치한다
    /// </summary>
    public void OnTransposeDown()
    {
        switch (ADFGVXGameManager.CurrentSystemMode)
        {
            case ADFGVXGameManager.SystemMode.Decryption: 
            {
                KeyInputField.IsReadyForInput = false;
                KeyInputField.IsFlash = false;
                
                //빈칸을 제거하여 반환
                string encryptedText = ADFGVXGameManager.LoadEncrypted.EncryptedTextBody.TextTMP.text.Replace(" ", "");
                string inputKey = KeyInputField.StringBuffer;

                //에러
                if (inputKey.Length == 0)
                    return;
                if (encryptedText.Length / inputKey.Length > 30)
                    return;
                if (encryptedText.Length % inputKey.Length != 0)
                    return;
                
                KeyInputField.CheckInputField();
                for (int i = 0; i < 9; i++)
                {
                    TempLine[i] = "";
                    TransposeLines[i].text = "";
                }
                
                var inputPriority = 1;
                RowLength = inputKey.Length;
                LineLength = encryptedText.Length / inputKey.Length;
                for (var i = 0; i < inputKey.Length; i++)
                {
                    for (var j = 0; j < inputKey.Length; j++)
                    {
                        if (Priority[j] == inputPriority)
                        {
                            inputPriority++;
                            TempLine[j] = encryptedText.Substring(0, LineLength);
                            encryptedText = encryptedText.Substring(LineLength);
                        }
                    }
                }

                CurrentTransposedText = "";
                for(var i = 0; i < LineLength; i++)
                for(var j = 0; j < RowLength; j++)
                    CurrentTransposedText += TempLine[j].ToCharArray()[i];

                //전치 행렬 출력
                StartCoroutine(CircumventRow(0));
        
                //복호화해야하는 순서 계산하여 출력
                ADFGVXGameManager.BilateralSubstitute.UpdateTransposedTextDisplayAndTable();

                //전치가 완료될 때까지 전체 입력 차단
                ADFGVXGameManager.CutAvailabilityInputForWhile(0f, (RowLength + LineLength) * 0.1f);
                
                break;
            }
            case ADFGVXGameManager.SystemMode.Encryption: 
            {
                KeyInputField.IsReadyForInput = false;
                KeyInputField.IsFlash = false;
                return;
            }
        }
    }

    /// <summary>
    /// 행을 순회하면서 열에 데이터가 순차적으로 출력되도록 하며, 결과적으로 물결 흐름처럼 데이터가 출력된다
    /// </summary>
    private IEnumerator CircumventRow(int rowIdx)
    {
        if(rowIdx == RowLength)
            yield break;

        StartCoroutine(PrintLine(rowIdx, 0));
        yield return new WaitForSeconds(0.1f);
        StartCoroutine(CircumventRow(rowIdx + 1));
    }

    /// <summary>
    /// 열에 데이터가 순차적으로 출력되도록 한다
    /// </summary>
    private IEnumerator PrintLine(int rowIdx, int lineIdx)
    {
        if(lineIdx == LineLength)
            yield break;
        TransposeLines[rowIdx].text += TempLine[rowIdx][lineIdx].ToString();
        yield return new WaitForSeconds(0.1f);
        StartCoroutine(PrintLine(rowIdx, lineIdx + 1));
    }

    #region 튜토리얼 전용

    public void EncryptTutorialPhase_2()
    {
        if (ADFGVXGameManager.ADFGVXTutorialManager.IsEncryptPlaying())
            if(KeyInputField.StringBuffer == "WAIT")
                ADFGVXGameManager.ADFGVXTutorialManager.MoveToNextTutorialPhase(2f);
    }
    public void EncryptTutorialPhase_3()
    {
        if (ADFGVXGameManager.ADFGVXTutorialManager.IsEncryptPlaying())
            if(ReverseTransposeLines.StringBuffer == "XXAFAFAVFGVGXVFGDDAVAGAFAFAVFGGG")
                ADFGVXGameManager.ADFGVXTutorialManager.MoveToNextTutorialPhase(2f);
    }
    public void DecryptTutorialPhase_1()
    {
        if (ADFGVXGameManager.ADFGVXTutorialManager.IsDecryptPlaying())
            if(KeyInputField.StringBuffer == "WAIT")
                ADFGVXGameManager.ADFGVXTutorialManager.MoveToNextTutorialPhase(2f);
    }

    #endregion
}