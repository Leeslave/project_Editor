using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class BilateralSubstitute_ForGame : MonoBehaviour
{
    public ADFGVXSceneManager_ForGame SceneManager { get; set; }

    public BasicText Title { get; set; }
    public BasicText CurrentTableNumDisplay { get; set; }
    public BasicText TransposedTextDisplay { get; set; }
    public BasicText[] TableElements { get; set; } = new BasicText[36];
    public BasicText[] LineElements { get; set; } = new BasicText[6];
    public BasicText[] RowElements { get; set; } = new BasicText[6];
    public BasicButton TablePlusButton { get; set; }
    public BasicButton TableMinusButton { get; set; }

    public int[] LastLineRowElements { get; set; } = new int[2] {
        0, 0
    };
    public Dictionary<char, int> LineRowDecode { get; set; } = new Dictionary<char, int>() {
        {'A', 0},
        {'D', 1},
        {'F', 2},
        {'G', 3},
        {'V', 4},
        {'X', 5},
    };

    public int CurrentTableNum { get; set; } = 0;
    
    private const int TableNumMax = 4;
    
    private void Awake()
    {
        SceneManager = FindObjectOfType<ADFGVXSceneManager_ForGame>();

        Title = this.transform.GetChild(0).GetComponent<BasicText>();
        CurrentTableNumDisplay = this.transform.GetChild(1).GetComponent<BasicText>();
        TransposedTextDisplay = this.transform.GetChild(2).GetComponent<BasicText>();
        for (var i = 0; i < 36; i++)
            TableElements[i] = this.transform.GetChild(3).transform.GetChild(i).GetComponent<BasicText>();
        for (var i = 0; i < 6; i++)
        {
            LineElements[i] = this.transform.GetChild(4).transform.GetChild(i).GetComponent<BasicText>();
            RowElements[i] = this.transform.GetChild(5).transform.GetChild(i).GetComponent<BasicText>();
        }
        TablePlusButton = this.transform.GetChild(6).GetComponent<BasicButton>();
        TableMinusButton = this.transform.GetChild(7).GetComponent<BasicButton>();
        
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
            LJWConverter.Instance.ConvertSpriteRendererColor(false, 0.0f, 0.2f, new Color(1.0f, 1.0f, 1.0f, 0.0f), TableElements[i].TextFill);
        LJWConverter.Instance.ConvertSpriteRendererColor(false, 0.0f, 0.2f, new Color(1.0f, 1.0f, 1.0f, 0.0f), LineElements[LastLineRowElements[0]].TextFill);
        LJWConverter.Instance.ConvertSpriteRendererColor(false, 0.0f, 0.2f, new Color(1.0f, 1.0f, 1.0f, 0.0f), RowElements[LastLineRowElements[1]].TextFill);
    }

    /// <summary>
    /// 일정 시간 동안 입력을 차단한다
    /// </summary>
    /// <param name="wait"> 대기 시간 </param>
    /// <param name="duration"> 차단 시간 </param>
    public IEnumerator CutOffInputForWhile(float wait, float duration)
    {
        yield return new WaitForSeconds(wait);
        TablePlusButton.SetAvailable(false);
        TableMinusButton.SetAvailable(false);
        yield return new WaitForSeconds(duration);
        TablePlusButton.SetAvailable(true);
        TableMinusButton.SetAvailable(true);
    }
    
    /// <summary>
    /// 플레이어 입력 가능 여부를 설정한다
    /// </summary>
    /// <param name="value"> 가능 여부 </param>
    public void SetAvailable(bool value)
    {
        TablePlusButton.SetAvailable(value);
        TableMinusButton.SetAvailable(value);
    }
    
    /// <summary>
    /// DisplayDecrypted에 복호화 입력 필드가 변할 때마다 플레이어에게 다음 입력 내용을 알려주기 위한 업데이트가 필요하다
    /// </summary>
    public void UpdateTransposedTextDisplayAndTable()
    {
        switch (SceneManager.CurrentSystemMode)
        {
            case ADFGVXSceneManager_ForGame.SystemMode.Decryption: 
            { 
                //복호화 해야하는 전치된 텍스트를 불러온다
                var transposedText = SceneManager.Transpose.CurrentTransposedText;
               
                //전치된 텍스트가 없으면 종료
                if (transposedText == "")
                {
                    Initialize();
                    return;
                }

                //역전치 라인에 필요 이상의 인풋이 들어가지 않도록 막는다
                SceneManager.DisplayDecrypted.DecryptedTextBody.MaxInputLength = transposedText.Length / 2;

                //현재까지 복호화한 내용의 길이
                var length = SceneManager.DisplayDecrypted.DecryptedTextBody.StringBuffer.Length;
                
                //현재까지 진행된 복호화 정도에 따라서 작업
                if (2 * length >= transposedText.Length)
                {
                    //복호화가 완료되었을 경우
                    TransposedTextDisplay.TextTMP.text = "<color=#2DFF2D>TASK COMPLETE!</color>";
                    
                    //테이블 소등
                    LJWConverter.Instance.ConvertSpriteRendererColor(false, 0.0f, 0.2f, new Color(1.0f, 1.0f, 1.0f, 0.0f), LineElements[LastLineRowElements[0]].TextFill);
                    LJWConverter.Instance.ConvertSpriteRendererColor(false, 0.0f, 0.2f, new Color(1.0f, 1.0f, 1.0f, 0.0f), RowElements[LastLineRowElements[1]].TextFill);
                    for(var i = 0; i < LastLineRowElements[1]; i++)
                        LJWConverter.Instance.ConvertSpriteRendererColor(false, 0.0f, 0.2f, new Color(1.0f, 1.0f, 1.0f, 0.0f), TableElements[6 * LastLineRowElements[0] + i].TextFill);
                    for(var i = 0; i < LastLineRowElements[0]; i++)
                        LJWConverter.Instance.ConvertSpriteRendererColor(false, 0.0f, 0.2f, new Color(1.0f, 1.0f, 1.0f, 0.0f), TableElements[6 * i + LastLineRowElements[1]].TextFill);
                    LJWConverter.Instance.ConvertSpriteRendererColor(false, 0.0f, 0.2f, new Color(1.0f, 1.0f, 1.0f, 0.0f), TableElements[6 * LastLineRowElements[0] + LastLineRowElements[1]].TextFill);
                }
                else
                {
                    //복호화가 진행 중일 경우
                    var red = transposedText.Substring(2 * length, 2);
                    var left = transposedText.Substring(2 * length + 2);
                    var result = "<color=#FF0000>" + red + "</color>" + left;
                    TransposedTextDisplay.TextTMP.text = result;
                    
                    //테이블 소등
                    LJWConverter.Instance.ConvertSpriteRendererColor(false, 0.0f, 0.2f, new Color(1.0f, 1.0f, 1.0f, 0.0f), LineElements[LastLineRowElements[0]].TextFill);
                    LJWConverter.Instance.ConvertSpriteRendererColor(false, 0.0f, 0.2f, new Color(1.0f, 1.0f, 1.0f, 0.0f), RowElements[LastLineRowElements[1]].TextFill);
                    for(var i = 0; i < LastLineRowElements[1]; i++)
                        LJWConverter.Instance.ConvertSpriteRendererColor(false, 0.0f, 0.2f, new Color(1.0f, 1.0f, 1.0f, 0.0f), TableElements[6 * LastLineRowElements[0] + i].TextFill);
                    for(var i = 0; i < LastLineRowElements[0]; i++)
                        LJWConverter.Instance.ConvertSpriteRendererColor(false, 0.0f, 0.2f, new Color(1.0f, 1.0f, 1.0f, 0.0f), TableElements[6 * i + LastLineRowElements[1]].TextFill);
                    LJWConverter.Instance.ConvertSpriteRendererColor(false, 0.0f, 0.2f, new Color(1.0f, 1.0f, 1.0f, 0.0f), TableElements[6 * LastLineRowElements[0] + LastLineRowElements[1]].TextFill);
                    
                    //테이블 점등
                    LastLineRowElements[0] = LineRowDecode[red.ToCharArray()[0]];
                    LastLineRowElements[1] = LineRowDecode[red.ToCharArray()[1]];
                    LJWConverter.Instance.ConvertSpriteRendererColor(false, 0.0f, 0.2f, new Color(1.0f, 0.1764f, 0.1764f, 1.0f), LineElements[LastLineRowElements[0]].TextFill);
                    LJWConverter.Instance.ConvertSpriteRendererColor(false, 0.0f, 0.2f, new Color(1.0f, 0.1764f, 0.1764f, 1.0f), RowElements[LastLineRowElements[1]].TextFill);
                    for(var i = 0; i < LastLineRowElements[1]; i++)
                        LJWConverter.Instance.ConvertSpriteRendererColor(false, 0.0f, 0.2f, new Color(1.0f, 0.1764f, 0.1764f, 0.5f), TableElements[6 * LastLineRowElements[0] + i].TextFill);
                    for(var i = 0; i < LastLineRowElements[0]; i++)
                        LJWConverter.Instance.ConvertSpriteRendererColor(false, 0.0f, 0.2f, new Color(1.0f, 0.1764f, 0.1764f, 0.5f), TableElements[6 * i + LastLineRowElements[1]].TextFill);
                    LJWConverter.Instance.ConvertSpriteRendererColor(false, 0.0f, 0.2f, new Color(1.0f, 0.1764f, 0.1764f, 1.0f), TableElements[6 * LastLineRowElements[0] + LastLineRowElements[1]].TextFill);
                }
        
                break;
            }
            case ADFGVXSceneManager_ForGame.SystemMode.Encryption:
            {
                //암호화 해야하는 평문을 불러온다
                var plainText = SceneManager.WritePlain.PlainTextBody.StringBuffer;
                
                //평문 텍스트가 없으면 종료
                if (plainText == "")
                {
                    Initialize();
                    return;
                }
                
                //역전치 라인에 필요 이상의 인풋이 들어가지 않도록 막는다
                SceneManager.Transpose.ReverseTransposeLines.MaxInputLength = plainText.Length * 2;
                
                //현재까지 역전치한 내용의 길이
                var length = SceneManager.Transpose.ReverseTransposeLines.StringBuffer.Length;

                //현재까지 진행된 암호화 정도에 따라서 작업
                if (length >= plainText.Length * 2)
                {
                    //역전치가 완료되었을 경우
                    TransposedTextDisplay.TextTMP.text = "<color=#2DFF2D>TASK COMPLETE!</color>";
                    
                    //테이블 소등
                    LJWConverter.Instance.ConvertSpriteRendererColor(false, 0.0f, 0.2f, new Color(1.0f, 1.0f, 1.0f, 0.0f), LineElements[LastLineRowElements[0]].TextFill);
                    LJWConverter.Instance.ConvertSpriteRendererColor(false, 0.0f, 0.2f, new Color(1.0f, 1.0f, 1.0f, 0.0f), RowElements[LastLineRowElements[1]].TextFill);
                    for(var i = 0; i < LastLineRowElements[1]; i++)
                        LJWConverter.Instance.ConvertSpriteRendererColor(false, 0.0f, 0.2f, new Color(1.0f, 1.0f, 1.0f, 0.0f), TableElements[6 * LastLineRowElements[0] + i].TextFill);
                    for(var i = 0; i < LastLineRowElements[0]; i++)
                        LJWConverter.Instance.ConvertSpriteRendererColor(false, 0.0f, 0.2f, new Color(1.0f, 1.0f, 1.0f, 0.0f), TableElements[6 * i + LastLineRowElements[1]].TextFill);
                    LJWConverter.Instance.ConvertSpriteRendererColor(false, 0.0f, 0.2f, new Color(1.0f, 1.0f, 1.0f, 0.0f), TableElements[6 * LastLineRowElements[0] + LastLineRowElements[1]].TextFill);
                }
                else
                {
                    //역전치가 진행 중일 경우
                    var red = plainText.Substring(Mathf.FloorToInt(length / 2f), 1);
                    var left = plainText.Substring(Mathf.FloorToInt(length / 2f) + 1);
                    var result = "<color=#FF0000>" + red + "</color>" + left;
                    TransposedTextDisplay.TextTMP.text = result;
                    
                    //테이블 소등
                    LJWConverter.Instance.ConvertSpriteRendererColor(false, 0.0f, 0.2f, new Color(1.0f, 1.0f, 1.0f, 0.0f), LineElements[LastLineRowElements[0]].TextFill);
                    LJWConverter.Instance.ConvertSpriteRendererColor(false, 0.0f, 0.2f, new Color(1.0f, 1.0f, 1.0f, 0.0f), RowElements[LastLineRowElements[1]].TextFill);
                    for(var i = 0; i < LastLineRowElements[1]; i++)
                        LJWConverter.Instance.ConvertSpriteRendererColor(false, 0.0f, 0.2f, new Color(1.0f, 1.0f, 1.0f, 0.0f), TableElements[6 * LastLineRowElements[0] + i].TextFill);
                    for(var i = 0; i < LastLineRowElements[0]; i++)
                        LJWConverter.Instance.ConvertSpriteRendererColor(false, 0.0f, 0.2f, new Color(1.0f, 1.0f, 1.0f, 0.0f), TableElements[6 * i + LastLineRowElements[1]].TextFill);
                    LJWConverter.Instance.ConvertSpriteRendererColor(false, 0.0f, 0.2f, new Color(1.0f, 1.0f, 1.0f, 0.0f), TableElements[6 * LastLineRowElements[0] + LastLineRowElements[1]].TextFill);
                    
                    //테이블 점등
                    var idx = 0;
                    for (; idx < 36; idx++)
                        if (TableElements[idx].TextTMP.text == red)
                            break;
                    LastLineRowElements[0] = idx / 6;
                    LastLineRowElements[1] = idx % 6;
                    LJWConverter.Instance.ConvertSpriteRendererColor(false, 0.0f, 0.2f, new Color(1.0f, 0.1764f, 0.1764f, 1.0f), LineElements[LastLineRowElements[0]].TextFill);
                    LJWConverter.Instance.ConvertSpriteRendererColor(false, 0.0f, 0.2f, new Color(1.0f, 0.1764f, 0.1764f, 1.0f), RowElements[LastLineRowElements[1]].TextFill);
                    for(var i = 0; i < LastLineRowElements[1]; i++)
                        LJWConverter.Instance.ConvertSpriteRendererColor(false, 0.0f, 0.2f, new Color(1.0f, 0.1764f, 0.1764f, 0.5f), TableElements[6 * LastLineRowElements[0] + i].TextFill);
                    for(var i = 0; i < LastLineRowElements[0]; i++)
                        LJWConverter.Instance.ConvertSpriteRendererColor(false, 0.0f, 0.2f, new Color(1.0f, 0.1764f, 0.1764f, 0.5f), TableElements[6 * i + LastLineRowElements[1]].TextFill);
                    LJWConverter.Instance.ConvertSpriteRendererColor(false, 0.0f, 0.2f, new Color(1.0f, 0.1764f, 0.1764f, 1.0f), TableElements[6 * LastLineRowElements[0] + LastLineRowElements[1]].TextFill);
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
        var filePath = "Assets/Resources/Tables/Table_" + CurrentTableNum.ToString() + ".txt";
        FileInfo txtFile = new FileInfo(filePath);

        if (!txtFile.Exists)
            Debug.Log("Unexist Filename!");
            
        StreamReader reader = new StreamReader(filePath);
        var value = reader.ReadToEnd();
        reader.Close();

        for(var i = 0; i < 36; i++)
        {
            TableElements[i].TextTMP.text = value[i].ToString();
        }
        
    }

    /// <summary>
    /// 현재 테이블 번호를 하나 늘린다
    /// </summary>
    public void TablePlus()
    {
        CurrentTableNum++;
        CurrentTableNum %= TableNumMax;
        CurrentTableNumDisplay.TextTMP.text = "치환 테이블 " + CurrentTableNum.ToString() + "번";
        UpdateTable();
    }

    /// <summary>
    /// 현재 테이블 번호를 하나 줄인다
    /// </summary>
    public void TableMinus()
    {
        CurrentTableNum--;
        if (CurrentTableNum < 0)
            CurrentTableNum = TableNumMax - 1;
        CurrentTableNumDisplay.TextTMP.text = "치환 테이블 " + CurrentTableNum.ToString() + "번";
        UpdateTable();
    }
}
