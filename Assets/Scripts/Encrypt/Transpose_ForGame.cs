using System;
using System.Collections;
using UnityEngine;
using TMPro;

public class Transpose_ForGame : MonoBehaviour
{
    private ADFGVXSceneManager_ForGame SceneManager;

    public BasicText Title { get; set; }
    public BasicInputField KeyInputField { get; set; }
    public BasicText KeyPriority { get; set; }
    public TextMeshPro[] TransposeLines { get; set; }
    public BasicInputField ReverseTransposeLines { get; set; }

    //키 순위 전치 관련 변수
    public int[] Priority { get; set; } = new int[9];
    public int RowLength { get; set; }
    public int LineLength { get; set; }
    public string[] TempLine { get; set; } = new string[9];
    public string CurrentTransposedText { get; set; } = "";

    private void Awake()
    {
        SceneManager = FindObjectOfType<ADFGVXSceneManager_ForGame>();

        Title = this.transform.GetChild(0).GetComponent<BasicText>();
        KeyInputField = this.transform.GetChild(1).GetComponent<BasicInputField>();
        KeyPriority = this.transform.GetChild(2).GetComponent<BasicText>();
        TransposeLines = new TextMeshPro[9];
        for(var i = 0; i < 9; i++)
        {
            TransposeLines[i] = this.transform.GetChild(3).transform.GetChild(i).GetComponent<TextMeshPro>();
            TransposeLines[i].text = "";
        }
        ReverseTransposeLines = this.transform.GetChild(4).GetComponent<BasicInputField>();
        
        Initialize();
    }

    /// <summary>
    /// 모드가 전환될 때마다 버퍼에 있는 내용을 깨끗하게 비우고 초기화해야 한다
    /// </summary>
    public void Initialize() {
        KeyInputField.Initialize();
        KeyPriority.TextTMP.text = "_ _ _ _ _ _ _ _ _";
        ClearTransposedMatrix();
        ReverseTransposeLines.Initialize();
        
        switch (SceneManager.CurrentSystemMode)
        {
            case ADFGVXSceneManager_ForGame.SystemMode.Decryption:
                ReverseTransposeLines.gameObject.SetActive(false);
                break;
            case ADFGVXSceneManager_ForGame.SystemMode.Encryption:
                ReverseTransposeLines.gameObject.SetActive(true);
                break;
        }
    }

    /// <summary>
    /// 일정 시간 동안 입력을 차단한다
    /// </summary>
    /// <param name="wait"> 대기 시간 </param>
    /// <param name="duration"> 차단 시간 </param>
    public IEnumerator CutOffInputForWhile(float wait, float duration)
    {
        yield return new WaitForSeconds(wait);
        KeyInputField.SetAvailable(false);
        ReverseTransposeLines.SetAvailable(false);
        yield return new WaitForSeconds(duration);
        KeyInputField.SetAvailable(true);
        ReverseTransposeLines.SetAvailable(true);
    }
    
    /// <summary>
    /// 플레이어 입력 가능 여부를 설정한다
    /// </summary>
    /// <param name="value"> 가능 여부 </param>
    public void SetAvailable(bool value)
    {
        KeyInputField.SetAvailable(value);
        ReverseTransposeLines.SetAvailable(value);
    }
    
    /// <summary>
    /// 키 값이 변경되었다면 새롭게 키 순위를 계산해야 한다
    /// </summary>
    public void UpdateKeyPriority()
    {
        var value = KeyInputField.StringBuffer;
        
        //키 순서를 결정하는 알고리즘
        for(var i = 0; i < value.Length; i++)
            Priority[i] = 1;
        for (var i = 0; i < value.Length; i++)
        {
            for (var j = 0; j < value.Length; j++)
            {
                if (i == j) 
                    continue;
                if (value[i] > value[j])
                    Priority[i]++;
                if (value[i] == value[j] && i < j)
                    Priority[j]++;
            }
        }
        
        //디스플레이 부분
        var result = "";
        for (var i = 0; i < KeyInputField.StringBuffer.Length; i++)
            result += Priority[i] + " ";
        for (var i = KeyInputField.StringBuffer.Length; i < 9; i++)
            result += "_ ";
        KeyPriority.TextTMP.text = result;
        
        //행렬 디스플레이 비움
        ClearTransposedMatrix();
        
        if (value.Length == 0 || value.Length == 1)
        {
            //키의 길이는 0이나 1이 될 수 없다
            ReverseTransposeLines.InputFieldTMP.color = new Color(1f, 0.3f, 0.3f, 1f);
            ReverseTransposeLines.InputFieldTMP.rectTransform.sizeDelta = new Vector2(3.03f * 9, 82f);
            ReverseTransposeLines.InputFieldTMP.characterSpacing = 0.0f;
            ReverseTransposeLines.InputFieldTMP.text = "길이가 0, 1인 키는 사용할 수 없습니다!";
        }
        else if((ReverseTransposeLines.StringBuffer.Length != 2 * SceneManager.WritePlain.PlainTextBody.StringBuffer.Length) || (ReverseTransposeLines.StringBuffer.Length % value.Length != 0))
        {
            //평문 전체를 전치하지 못했거나 키값이 유효하지 않다면
            ReverseTransposeLines.InputFieldTMP.color = new Color(1f, 0.3f, 0.3f, 1f);
            ReverseTransposeLines.InputFieldTMP.rectTransform.sizeDelta = new Vector2(3.03f * value.Length, 82f);
            ReverseTransposeLines.InputFieldTMP.characterSpacing = 37.5f;
            ReverseTransposeLines.InputFieldTMP.text = ReverseTransposeLines.StringBuffer;
        }
        else
        {
            //색을 통해서 플레이어에게 현재 역전치 내용이 유효함을 알린다
            ReverseTransposeLines.InputFieldTMP.color = new Color(0.3f, 1f, 0.3f, 1f);
            ReverseTransposeLines.InputFieldTMP.rectTransform.sizeDelta = new Vector2(3.03f * value.Length, 82f);
            ReverseTransposeLines.InputFieldTMP.characterSpacing = 37.5f;
            ReverseTransposeLines.InputFieldTMP.text = ReverseTransposeLines.StringBuffer;
        }
        
    }

    /// <summary>
    /// 암호화되어 있는 데이터를 키 순위에 따라서 전치한다
    /// </summary>
    public void OnTransposeDown()
    {
        switch (SceneManager.CurrentSystemMode)
        {
            case ADFGVXSceneManager_ForGame.SystemMode.Decryption: 
            {
                KeyInputField.IsReadyForInput = false;
                KeyInputField.IsFlash = false;
                
                //빈칸을 제거하여 반환
                var encryptedText = SceneManager.LoadEncrypted.EncryptedTextBody.TextTMP.text.Replace(" ", "");
                
                
                var key = KeyInputField.StringBuffer;

                //에러
                if (key.Length == 0)
                    return;
                if (encryptedText.Length / key.Length > 30)
                    return;
                if (encryptedText.Length % key.Length != 0)
                    return;

                ClearTransposedMatrix();

                var inputPriority = 1;
                RowLength = key.Length;
                LineLength = encryptedText.Length / key.Length;
                for (var i = 0; i < key.Length; i++)
                {
                    for (var j = 0; j < key.Length; j++)
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
        
                //전치 행렬 전달
                SceneManager.BilateralSubstitute.UpdateTransposedTextDisplayAndTable();

                //전치가 완료될 때까지 전체 입력 차단
                SceneManager.CutOffInputForWhile(0f, (RowLength + LineLength) * 0.1f);
                
                break;
            }
            case ADFGVXSceneManager_ForGame.SystemMode.Encryption: 
            {
                KeyInputField.IsReadyForInput = false;
                KeyInputField.IsFlash = false;

                //이제 필요없는 부분
                return;
            }
        }
    }
    
    /// <summary>
    /// 현재 전치되어 있는 행렬을 깨끗하게 비운다
    /// </summary>
    private void ClearTransposedMatrix()
    {
        KeyInputField.CheckInputField();
        for (var i = 0; i < 9; i++)
        {
            TempLine[i] = "";
            TransposeLines[i].text = "";
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
}