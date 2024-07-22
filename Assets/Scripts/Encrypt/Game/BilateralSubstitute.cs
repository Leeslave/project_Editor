using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class BilateralSubstitute : MonoBehaviour
{
    public BasicText Title { get; set; }
    public BasicText CurrentTableNumDisplay { get; set; }
    public BasicText TransposedTextDisplay { get; set; }
    public BasicText[] TableElements { get; set; } = new BasicText[36];
    public BasicText[] LineElements { get; set; } = new BasicText[6];
    public BasicText[] RowElements { get; set; } = new BasicText[6];
    public BasicButton NextTableButton { get; set; }
    public BasicButton BeforeTableButton { get; set; }

    private int[] LastLineRowElements { get; } = new int[2] {
        0, 0
    };
    public Dictionary<char, int> LineRowDecode { get; } = new() {
        {'A', 0},
        {'D', 1},
        {'F', 2},
        {'G', 3},
        {'V', 4},
        {'X', 5},
    };

    private int CurrentTableNum { get; set; } = 0;
    
    private const int TableNumMax = 4;
    
    private void Awake()
    {
        Title = transform.GetChild(0).GetComponent<BasicText>();
        CurrentTableNumDisplay = transform.GetChild(1).GetComponent<BasicText>();
        TransposedTextDisplay = transform.GetChild(2).GetComponent<BasicText>();
        for (var i = 0; i < 36; i++)
            TableElements[i] = transform.GetChild(3).transform.GetChild(i).GetComponent<BasicText>();
        for (var i = 0; i < 6; i++)
        {
            LineElements[i] = transform.GetChild(4).transform.GetChild(i).GetComponent<BasicText>();
            RowElements[i] = transform.GetChild(5).transform.GetChild(i).GetComponent<BasicText>();
        }
        NextTableButton = transform.GetChild(6).GetComponent<BasicButton>();
        BeforeTableButton = transform.GetChild(7).GetComponent<BasicButton>();
        
        UpdateTable();
        Initialize();
    }

    /// <summary>
    /// 모드가 전환될 때마다 버퍼에 있는 내용을 깨끗하게 비우고 초기화해야 한다
    /// </summary>
    public void Initialize()
    {
        TransposedTextDisplay.TextTMP.text = "-WAITING TASK-";
        
        //테이블 전체 소등
        for(var i = 0; i < 36; i++)
            LJWConverter.Instance.GradientSpriteRendererColor(false, 0.0f, 0.2f, new Color(1.0f, 1.0f, 1.0f, 0.0f), TableElements[i].TextFill);
        LJWConverter.Instance.GradientSpriteRendererColor(false, 0.0f, 0.2f, new Color(1.0f, 1.0f, 1.0f, 0.0f), LineElements[LastLineRowElements[0]].TextFill);
        LJWConverter.Instance.GradientSpriteRendererColor(false, 0.0f, 0.2f, new Color(1.0f, 1.0f, 1.0f, 0.0f), RowElements[LastLineRowElements[1]].TextFill);
    }

    /// <summary>
    /// 일정 시간 동안 입력을 차단한다
    /// </summary>
    /// <param name="wait"> 대기 시간 </param>
    /// <param name="duration"> 차단 시간 </param>
    public void CutAvailabilityInputForWhile(float wait, float duration)
    {
        NextTableButton.CutAvailabilityForWhile(wait, duration);
        BeforeTableButton.CutAvailabilityForWhile(wait, duration);
    }
    
    /// <summary>
    /// 플레이어 입력 가능 여부를 설정한다
    /// </summary>
    /// <param name="value"> 가능 여부 </param>
    public void SetAvailable(bool value)
    {
        NextTableButton.SetAvailability(value);
        BeforeTableButton.SetAvailability(value);
    }
    
    /// <summary>
    /// DisplayDecrypted에 복호화 입력 필드가 변할 때마다 플레이어에게 다음 입력 내용을 알려주기 위한 업데이트가 필요하다
    /// </summary>
    public void UpdateTransposedTextDisplayAndTable()
    {
        switch (ADFGVXGameManager.CurrentSystemMode)
        {
            case ADFGVXGameManager.SystemMode.Decryption: 
            { 
                //복호화 해야하는 전치된 텍스트를 불러온다
                var transposedText = ADFGVXGameManager.KeyPriorityTranspose.CurrentTransposedText;
               
                //전치된 텍스트가 없으면 종료
                if (transposedText == "")
                {
                    Initialize();
                    return;
                }

                //역전치 라인에 필요 이상의 인풋이 들어가지 않도록 막는다
                ADFGVXGameManager.DisplayDecrypted.DecryptedTextBody.MaxInputLength = transposedText.Length / 2;

                //현재까지 복호화한 내용의 길이
                var length = ADFGVXGameManager.DisplayDecrypted.DecryptedTextBody.StringBuffer.Length;
                
                //현재까지 진행된 복호화 정도에 따라서 작업
                if (2 * length >= transposedText.Length)
                {
                    //복호화가 완료되었을 경우
                    TransposedTextDisplay.TextTMP.text = "<color=#2DFF2D>TASK COMPLETE!</color>";
                    
                    //테이블 소등
                    LJWConverter.Instance.GradientSpriteRendererColor(false, 0.0f, 0.2f, new Color(1.0f, 1.0f, 1.0f, 0.0f), LineElements[LastLineRowElements[0]].TextFill);
                    LJWConverter.Instance.GradientSpriteRendererColor(false, 0.0f, 0.2f, new Color(1.0f, 1.0f, 1.0f, 0.0f), RowElements[LastLineRowElements[1]].TextFill);
                    for(var i = 0; i < LastLineRowElements[1]; i++)
                        LJWConverter.Instance.GradientSpriteRendererColor(false, 0.0f, 0.2f, new Color(1.0f, 1.0f, 1.0f, 0.0f), TableElements[6 * LastLineRowElements[0] + i].TextFill);
                    for(var i = 0; i < LastLineRowElements[0]; i++)
                        LJWConverter.Instance.GradientSpriteRendererColor(false, 0.0f, 0.2f, new Color(1.0f, 1.0f, 1.0f, 0.0f), TableElements[6 * i + LastLineRowElements[1]].TextFill);
                    LJWConverter.Instance.GradientSpriteRendererColor(false, 0.0f, 0.2f, new Color(1.0f, 1.0f, 1.0f, 0.0f), TableElements[6 * LastLineRowElements[0] + LastLineRowElements[1]].TextFill);
                }
                else
                {
                    //복호화가 진행 중일 경우
                    var red = transposedText.Substring(2 * length, 2);
                    var left = transposedText.Substring(2 * length + 2);
                    var result = "<color=#FF0000>" + red + "</color>" + left;
                    TransposedTextDisplay.TextTMP.text = result;
                    
                    //테이블 소등
                    LJWConverter.Instance.GradientSpriteRendererColor(false, 0.0f, 0.2f, new Color(1.0f, 1.0f, 1.0f, 0.0f), LineElements[LastLineRowElements[0]].TextFill);
                    LJWConverter.Instance.GradientSpriteRendererColor(false, 0.0f, 0.2f, new Color(1.0f, 1.0f, 1.0f, 0.0f), RowElements[LastLineRowElements[1]].TextFill);
                    for(var i = 0; i < LastLineRowElements[1]; i++)
                        LJWConverter.Instance.GradientSpriteRendererColor(false, 0.0f, 0.2f, new Color(1.0f, 1.0f, 1.0f, 0.0f), TableElements[6 * LastLineRowElements[0] + i].TextFill);
                    for(var i = 0; i < LastLineRowElements[0]; i++)
                        LJWConverter.Instance.GradientSpriteRendererColor(false, 0.0f, 0.2f, new Color(1.0f, 1.0f, 1.0f, 0.0f), TableElements[6 * i + LastLineRowElements[1]].TextFill);
                    LJWConverter.Instance.GradientSpriteRendererColor(false, 0.0f, 0.2f, new Color(1.0f, 1.0f, 1.0f, 0.0f), TableElements[6 * LastLineRowElements[0] + LastLineRowElements[1]].TextFill);
                    
                    //테이블 점등
                    LastLineRowElements[0] = LineRowDecode[red.ToCharArray()[0]];
                    LastLineRowElements[1] = LineRowDecode[red.ToCharArray()[1]];
                    LJWConverter.Instance.GradientSpriteRendererColor(false, 0.0f, 0.2f, new Color(1.0f, 0.2f, 0.2f, 1.0f), LineElements[LastLineRowElements[0]].TextFill);
                    LJWConverter.Instance.GradientSpriteRendererColor(false, 0.0f, 0.2f, new Color(1.0f, 0.2f, 0.2f, 1.0f), RowElements[LastLineRowElements[1]].TextFill);
                    for(var i = 0; i < LastLineRowElements[1]; i++)
                        LJWConverter.Instance.GradientSpriteRendererColor(false, 0.0f, 0.2f, new Color(1.0f, 0.2f, 0.2f, 0.5f), TableElements[6 * LastLineRowElements[0] + i].TextFill);
                    for(var i = 0; i < LastLineRowElements[0]; i++)
                        LJWConverter.Instance.GradientSpriteRendererColor(false, 0.0f, 0.2f, new Color(1.0f, 0.2f, 0.2f, 0.5f), TableElements[6 * i + LastLineRowElements[1]].TextFill);
                    LJWConverter.Instance.GradientSpriteRendererColor(false, 0.0f, 0.2f, new Color(0.2f, 1f, 1f, 1f), TableElements[6 * LastLineRowElements[0] + LastLineRowElements[1]].TextFill);
                }
        
                break;
            }
            case ADFGVXGameManager.SystemMode.Encryption:
            {
                //암호화 해야하는 평문을 불러온다
                var plainText = ADFGVXGameManager.WritePlain.PlainTextBody.StringBuffer;
                
                //평문 텍스트가 없으면 종료
                if (plainText == "")
                {
                    Initialize();
                    return;
                }
                
                //역전치 라인에 필요 이상의 인풋이 들어가지 않도록 막는다
                ADFGVXGameManager.KeyPriorityTranspose.ReverseTransposeLines.MaxInputLength = plainText.Length * 2;
                
                //현재까지 역전치한 내용의 길이
                var length = ADFGVXGameManager.KeyPriorityTranspose.ReverseTransposeLines.StringBuffer.Length;

                //현재까지 진행된 암호화 정도에 따라서 작업
                if (length >= plainText.Length * 2)
                {
                    //역전치가 완료되었을 경우
                    TransposedTextDisplay.TextTMP.text = "<color=#2DFF2D>TASK COMPLETE!</color>";
                    
                    //테이블 소등
                    LJWConverter.Instance.GradientSpriteRendererColor(false, 0.0f, 0.2f, new Color(1.0f, 1.0f, 1.0f, 0.0f), LineElements[LastLineRowElements[0]].TextFill);
                    LJWConverter.Instance.GradientSpriteRendererColor(false, 0.0f, 0.2f, new Color(1.0f, 1.0f, 1.0f, 0.0f), RowElements[LastLineRowElements[1]].TextFill);
                    for(var i = 0; i < LastLineRowElements[1]; i++)
                        LJWConverter.Instance.GradientSpriteRendererColor(false, 0.0f, 0.2f, new Color(1.0f, 1.0f, 1.0f, 0.0f), TableElements[6 * LastLineRowElements[0] + i].TextFill);
                    for(var i = 0; i < LastLineRowElements[0]; i++)
                        LJWConverter.Instance.GradientSpriteRendererColor(false, 0.0f, 0.2f, new Color(1.0f, 1.0f, 1.0f, 0.0f), TableElements[6 * i + LastLineRowElements[1]].TextFill);
                    LJWConverter.Instance.GradientSpriteRendererColor(false, 0.0f, 0.2f, new Color(1.0f, 1.0f, 1.0f, 0.0f), TableElements[6 * LastLineRowElements[0] + LastLineRowElements[1]].TextFill);
                }
                else
                {
                    //역전치가 진행 중일 경우
                    var red = plainText.Substring(Mathf.FloorToInt(length / 2f), 1);
                    var left = plainText.Substring(Mathf.FloorToInt(length / 2f) + 1);
                    var result = "<color=#FF0000>" + red + "</color>" + left;
                    TransposedTextDisplay.TextTMP.text = result;
                    
                    //테이블 소등
                    LJWConverter.Instance.GradientSpriteRendererColor(false, 0.0f, 0.2f, new Color(1.0f, 1.0f, 1.0f, 0.0f), LineElements[LastLineRowElements[0]].TextFill);
                    LJWConverter.Instance.GradientSpriteRendererColor(false, 0.0f, 0.2f, new Color(1.0f, 1.0f, 1.0f, 0.0f), RowElements[LastLineRowElements[1]].TextFill);
                    for(var i = 0; i < LastLineRowElements[1]; i++)
                        LJWConverter.Instance.GradientSpriteRendererColor(false, 0.0f, 0.2f, new Color(1.0f, 1.0f, 1.0f, 0.0f), TableElements[6 * LastLineRowElements[0] + i].TextFill);
                    for(var i = 0; i < LastLineRowElements[0]; i++)
                        LJWConverter.Instance.GradientSpriteRendererColor(false, 0.0f, 0.2f, new Color(1.0f, 1.0f, 1.0f, 0.0f), TableElements[6 * i + LastLineRowElements[1]].TextFill);
                    LJWConverter.Instance.GradientSpriteRendererColor(false, 0.0f, 0.2f, new Color(1.0f, 1.0f, 1.0f, 0.0f), TableElements[6 * LastLineRowElements[0] + LastLineRowElements[1]].TextFill);
                    
                    //테이블 점등
                    var idx = 0;
                    for (; idx < 36; idx++)
                        if (TableElements[idx].TextTMP.text == red)
                            break;
                    LastLineRowElements[0] = idx / 6;
                    LastLineRowElements[1] = idx % 6;
                    LJWConverter.Instance.GradientSpriteRendererColor(false, 0.0f, 0.2f, new Color(1.0f, 0.2f, 0.2f, 1.0f), LineElements[LastLineRowElements[0]].TextFill);
                    LJWConverter.Instance.GradientSpriteRendererColor(false, 0.0f, 0.2f, new Color(1.0f, 0.2f, 0.2f, 1.0f), RowElements[LastLineRowElements[1]].TextFill);
                    for(var i = 0; i < LastLineRowElements[1]; i++)
                        LJWConverter.Instance.GradientSpriteRendererColor(false, 0.0f, 0.2f, new Color(1.0f, 0.2f, 0.2f, 0.5f), TableElements[6 * LastLineRowElements[0] + i].TextFill);
                    for(var i = 0; i < LastLineRowElements[0]; i++)
                        LJWConverter.Instance.GradientSpriteRendererColor(false, 0.0f, 0.2f, new Color(1.0f, 0.2f, 0.2f, 0.5f), TableElements[6 * i + LastLineRowElements[1]].TextFill);
                    LJWConverter.Instance.GradientSpriteRendererColor(false, 0.0f, 0.2f, new Color(0.2f, 1f, 1f, 1f), TableElements[6 * LastLineRowElements[0] + LastLineRowElements[1]].TextFill);
                }
                
                break;
            }
        }
    }
    
    /// <summary>
    /// 현재 테이블 번호에 맞춰서 Elements의 텍스트를 업데이트한다
    /// </summary>
    private void UpdateTable()
    {
        string filePath = Application.dataPath + "/Resources/GameData/Encrypt/Tables/Table_" + CurrentTableNum.ToString() + ".txt";
        FileInfo txtFile = new(filePath);

        if (!txtFile.Exists)
            Debug.Log("테이블 로드에 문제 발생!");
            
        StreamReader reader = new(filePath);
        string value = reader.ReadToEnd();
        reader.Close();

        for(var i = 0; i < 36; i++)
            TableElements[i].TextTMP.text = value[i].ToString();

        //튜토리얼 전용
        if (ADFGVXGameManager.ADFGVXTutorialManager.IsDecryptPlaying() || ADFGVXGameManager.ADFGVXTutorialManager.IsEncryptPlaying())
            if(CurrentTableNum == 0)
                ADFGVXGameManager.ADFGVXTutorialManager.MoveToNextTutorialPhase(2f);
    }

    public void TablePlus()
    {
        int next = CurrentTableNum + 1 >= TableNumMax ? 0 : CurrentTableNum + 1;
        SetTable(next);
    }

    public void TableMinus()
    {
        int next = CurrentTableNum - 1 < 0 ? TableNumMax - 1 : CurrentTableNum - 1;
        SetTable(next);
    }

    public void SetTable(int num)
    {
        CurrentTableNum = num;
        CurrentTableNumDisplay.TextTMP.text = "치환 테이블 " + CurrentTableNum + "번";
        UpdateTable();
    }
}
