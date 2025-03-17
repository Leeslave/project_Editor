using System;
using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class ResultPanel : MonoBehaviour
{
    private BasicButton CloseButton { get; set; }
    public TextMeshPro Title { get; set; }
    private TextMeshPro Result { get; set; } 
    
    private void Awake()
    {
        CloseButton = transform.GetChild(0).GetComponent<BasicButton>();
        Title = transform.GetChild(1).GetChild(0).GetComponent<TextMeshPro>();
        Result = transform.GetChild(2).GetChild(0).GetComponent<TextMeshPro>();
    }

    public void ClosePanel()
    {
        transform.position = new Vector3(-350f, 0f, 0f);
        ADFGVXGameManager.SetAvailable(true);
    }

    public bool PrintDecryptionResult()
    {
        CloseButton.SetAvailability(false);
        
        if (ADFGVXGameManager.LoadEncrypted.EncryptedTextBody.TextTMP.text == "")
        {
            StartCoroutine(PrintDecryptionFailed_IE("복호화하고 싶은 파일을 로드하지 않았습니다!"));
            return false;
        }

        if (ADFGVXGameManager.KeyPriorityTranspose.KeyInputField.StringBuffer == "")
        {
            StartCoroutine(PrintDecryptionFailed_IE("복호화 키를 입력하지 않았습니다!"));
            return false;
        }
        
        if (ADFGVXGameManager.KeyPriorityTranspose.TransposeLines[0].text == "")
        {
            StartCoroutine(PrintDecryptionFailed_IE("키 순위 전치가 실행되지 않았습니다!"));
            return false;
        }
        
        if (ADFGVXGameManager.DisplayDecrypted.DecryptedTextBody.StringBuffer == "")
        {
            StartCoroutine(PrintDecryptionFailed_IE("ADFGVX 테이블에 따른 이중 문자 치환을 하지 않았습니다!"));
            return false;
        }

        //플레이어가 로드한 암호화된 텍스트
        var encryptedText = ADFGVXGameManager.LoadEncrypted.EncryptedTextBody.TextTMP.text.Replace(" ", "");
        //키 순위에서 빈칸 및 언더라인 제거
        var keyPriority = ADFGVXGameManager.KeyPriorityTranspose.KeyPriority.TextTMP.text.Replace(" ", "");
        keyPriority = keyPriority.Replace("_", "");
        //키 순위에 따라서 전치한 텍스트
        string[] orderedText = new string[keyPriority.Length];
        var transposedText = "";
        //복호화 텍스트
        var table = ADFGVXGameManager.BilateralSubstitute.TableElements;
        var dic = ADFGVXGameManager.BilateralSubstitute.LineRowDecode;
        var decryptedText = "";

        //일시적인 저장 공간으로 전치
        for (var i = 0; i < keyPriority.Length; i++)
            for (var j = 0; j < encryptedText.Length / keyPriority.Length; j++)
                orderedText[i] += encryptedText[(int.Parse(keyPriority[i].ToString()) - 1) * (encryptedText.Length / keyPriority.Length) + j];
        
        //최종 전치 결과
        for (var i = 0; i < encryptedText.Length / keyPriority.Length; i++)
            for(var j = 0; j < keyPriority.Length; j++)
                transposedText += orderedText[j][i];
        
        //테이블에 따라서 복호화
        for (var i = 0; i < encryptedText.Length / 2; i++)
        {
            var line = dic[transposedText[0]];
            var row = dic[transposedText[1]];
            decryptedText += table[row + line * 6].TextTMP.text;
            transposedText = transposedText.Substring(2);
        }
        
        //Debug.Log($"플레이어가 복호화한 결과물: {decryptedText}");

        if(ADFGVXGameManager.DisplayDecrypted.DecryptedTextBody.StringBuffer != ADFGVXGameManager.Instance.decryptResultText)
        {
            StartCoroutine(PrintDecryptionFailed_IE("복호화 데이터 무결성 검사를 통과하지 못했습니다!"));
            return false;
        }
        
        StartCoroutine(PrintDecryptionSuccess_IE());
        return true;
    }
    private IEnumerator PrintDecryptionSuccess_IE()
    {
        //결과 창 이동
        transform.localPosition = new Vector3(-57f, 17f, 0f);
        ADFGVXGameManager.SetAvailable(false);

        Result.text = "";
        
        LJWConverter.Instance.PrintTMPByDuration(false, 0f, 0.3f, "DECRYPT ADFGVX TEDP VER 2.0", false, Result);
        yield return new WaitForSeconds(0.75f);
        Result.text += "\n";
        LJWConverter.Instance.PrintTMPByDuration(false, 0f, 0.3f, "USER: BLACK-007 CHECK... ", false, Result);
        yield return new WaitForSeconds(0.75f);
        Result.text += "OK\n";
        LJWConverter.Instance.PrintTMPByDuration(false, 0f, 0.3f, "CONSOLE: RX-78-2 CHECK... ", false, Result);
        yield return new WaitForSeconds(0.75f);
        Result.text += "OK\n";
        LJWConverter.Instance.PrintTMPByDuration(false, 0f, 0.3f, "CONNECTION: CENTRAL TERMINAL CHECK... ", false, Result);
        yield return new WaitForSeconds(0.75f);
        Result.text += "OK\n";
        var task = string.Format("{0:D6}", Random.Range(0, 999999));
        LJWConverter.Instance.PrintTMPByDuration(false, 0f, 0.3f, "TASK " + task + " IS NOW ON OPERATION...", false, Result);
        yield return new WaitForSeconds(0.75f);
        Result.text += "\n";
        
        var before = Result.text;
        char[] gauge = new char[53];
        
        for (var i = 0; i < 53; i++)
        {
            gauge[i] = ' ';
        }
        for (var i = 0; i < 53; i++)
        {
            var builder = new StringBuilder();
            gauge[i] = '-';
            Result.text = before + "[" + builder.Append(gauge) + "]";
            yield return new WaitForSeconds(0.04f);
        }
        
        Result.text += "\n";
        LJWConverter.Instance.PrintTMPByDuration(false, 0f, 0.3f, "TASK " + task + " SUCCESS!", false, Result);
       yield return new WaitForSeconds(0.75f);
        Result.text += "\n";
        LJWConverter.Instance.PrintTMPByDuration(false, 0f, 0.3f, "복호화 파일을 저장했습니다!", false, Result);

        if (ADFGVXGameManager.ADFGVXTutorialManager.IsDecryptPlaying())
        {
            //튜토리얼 상황이었을 경우
            CloseButton.OnMouseUpEvent.RemoveListener(ClosePanel);
            CloseButton.OnMouseUpEvent.AddListener(() => {
                //Debug.Log("복호화 튜토리얼 종료!");
                GameSystem.Instance.ClearTask("ADFGVX_DT");
                GameSystem.LoadScene("Screen"); });
        }
        else
        {
            //복호화 태스크 클리어 마킹
            ADFGVXGameManager.Instance.decryptClear = true;
        
            //만약 암호화 태스크가 존재했고, 이미 클리어한 상태라면
            if (ADFGVXGameManager.Instance.encryptClear && ADFGVXGameManager.Instance.decryptClear)
            {
                //모든 태스크을 완료했으므로 씬에서 나갈 준비
                CloseButton.OnMouseUpEvent.RemoveListener(ClosePanel);
                CloseButton.OnMouseUpEvent.AddListener(() => {            
                    GameSystem.Instance.ClearTask("ADFGVX");
                    GameSystem.LoadScene("Screen"); });
            }   
        }
        
        //결과 창 닫기 버튼 활성화
        CloseButton.SetAvailability(true);   
    }
    private IEnumerator PrintDecryptionFailed_IE(string error)
    {
        //결과 창 이동
        this.transform.localPosition = new Vector3(-57f, 17f, 0f);
        ADFGVXGameManager.SetAvailable(false);

        Result.text = "";
        
        LJWConverter.Instance.PrintTMPByDuration(false, 0f, 0.3f, "DECRYPT ADFGVX TEDP VER 2.0", false, Result);
       yield return new WaitForSeconds(0.75f);
        Result.text += "\n";
        LJWConverter.Instance.PrintTMPByDuration(false, 0f, 0.3f, "USER: BLACK-007 CHECK... ", false, Result);
        yield return new WaitForSeconds(0.75f);
        Result.text += "OK\n";
        LJWConverter.Instance.PrintTMPByDuration(false, 0f, 0.3f, "CONSOLE: RX-78-2 CHECK... ", false, Result);
        yield return new WaitForSeconds(0.75f);
        Result.text += "OK\n";
        LJWConverter.Instance.PrintTMPByDuration(false, 0f, 0.3f, "CONNECTION: CENTRAL TERMINAL CHECK... ", false, Result);
        yield return new WaitForSeconds(0.75f);
        Result.text += "OK\n";
        var task = string.Format("{0:D6}", Random.Range(0, 999999));
        LJWConverter.Instance.PrintTMPByDuration(false, 0f, 0.3f, "TASK " + task + " IS NOW ON OPERATION...", false, Result);
        yield return new WaitForSeconds(0.75f);
        Result.text += "\n";
        
        var before = Result.text;
        char[] gauge = new char[53];
        
        for (var i = 0; i < 53; i++)
        {
            gauge[i] = ' ';
        }
        for (var i = 0; i < 53; i++)
        {
            var builder = new StringBuilder();
            gauge[i] = '-';
            Result.text = before + "[" + builder.Append(gauge) + "]";
            yield return new WaitForSeconds(0.04f);
        }
        
        Result.text += "\n";
        LJWConverter.Instance.PrintTMPByDuration(false, 0f, 0.3f, "TASK " + task + " FAILED!", false, Result);
       yield return new WaitForSeconds(0.75f);
        Result.text += "\n";
        LJWConverter.Instance.PrintTMPByDuration(false, 0f, 0.3f, error, false, Result);

        CloseButton.SetAvailability(true);
    }

    public bool PrintEncryptionResult()
    {
        CloseButton.SetAvailability(false);

        if (ADFGVXGameManager.WritePlain.PlainTextBody.StringBuffer != ADFGVXGameManager.Instance.encryptTargetText)
        {
            StartCoroutine(PrintDecryptionFailed_IE("잘못된 평문을 작성했습니다!"));
            return false;
        }
        
        if (ADFGVXGameManager.WritePlain.PlainTextBody.StringBuffer == "")
        {
            StartCoroutine(PrintEncryptionFailed_IE("암호화 하고 싶은 평문을 입력하지 않았습니다!"));
            return false;
        }

        if (ADFGVXGameManager.KeyPriorityTranspose.KeyInputField.StringBuffer == "")
        {
            StartCoroutine(PrintEncryptionFailed_IE("암호화 키를 입력하지 않았습니다!"));
            return false;
        }
        
        if (ADFGVXGameManager.KeyPriorityTranspose.ReverseTransposeLines.StringBuffer == "")
        {
            StartCoroutine(PrintEncryptionFailed_IE("ADFGVX 테이블에 따른 이중 문자 치환을 하지 않았습니다!"));
            return false;
        }

        if (ADFGVXGameManager.DisplayEncrypted.EncryptedTextBody.StringBuffer == "")
        {
            StartCoroutine(PrintEncryptionFailed_IE("키 순위 전치가 실행되지 않았습니다!"));
            return false;
        }

        if (ADFGVXGameManager.DisplayEncrypted.EncryptedTextTitle.TextTMP.text == "")
        {
            StartCoroutine(PrintEncryptionFailed_IE("복호화 데이터를 저장할 파일 이름을 입력하지 않았습니다!"));
            return false;
        }
        
        //플레이어가 작성한 평문
        var plainText = ADFGVXGameManager.WritePlain.PlainTextBody.StringBuffer;
        //키 순위에서 빈칸 및 언더라인 제거
        var keyPriority = ADFGVXGameManager.KeyPriorityTranspose.KeyPriority.TextTMP.text.Replace(" ", "");
        keyPriority = keyPriority.Replace("_", "");
        //테이블에 의해서 치환된 텍스트
        string[] adfgvx = { "A", "D", "F", "G", "V", "X" };
        var table = ADFGVXGameManager.BilateralSubstitute.TableElements;
        var tabledText = "";
        //키 순위에 의해서 전치된 텍스트
        string[] orderedText = new string[keyPriority.Length];
        for (var i = 0; i < orderedText.Length; i++)
            orderedText[i] = "";
        //최종 결과
        var encryptionResult = "";

        //플레이어가 작성한 평문과 현재 설정된 테이블을 이용하여 올바른 중간 암호화된 텍스트를 이끌어 낸다
        for (var i = 0; i < plainText.Length; i++)
            for (var j = 0; j < table.Length; j++)
                if (plainText[i].ToString() == table[j].TextTMP.text)
                    tabledText += adfgvx[j / 6] + adfgvx[j % 6];
        
        if (tabledText != ADFGVXGameManager.KeyPriorityTranspose.ReverseTransposeLines.StringBuffer)
        {
            StartCoroutine(PrintEncryptionFailed_IE("이중 문자 치환 결과가 유효하지 않습니다!"));
            return false;
        }
        
        //중간 암호화된 텍스트를 현재 키와 그 키 순서를 이용해서 정렬한 내용을 이용하여 정렬된 텍스트를 이끌어 낸다
        for (var i = 0; i < orderedText.Length; i++)
            for (var j = 0; j < tabledText.Length / keyPriority.Length; j++)
                orderedText[int.Parse(keyPriority[i].ToString()) - 1] += tabledText[i + keyPriority.Length * j].ToString();
        
        //올바른 암호화 텍스트를 획득한다
        for (var i = 0; i < orderedText.Length; i++)
            encryptionResult += orderedText[i];

        //Debug.Log($"플레이어가 암호화한 결과물: {encryptionResult}");
        
        if (ADFGVXGameManager.DisplayEncrypted.EncryptedTextBody.StringBuffer.Replace(" ", "") != ADFGVXGameManager.Instance.encryptResultText)
        {
            StartCoroutine(PrintEncryptionFailed_IE("암호화 데이터 무결성 검사를 통과하지 못했습니다!"));
            return false;
        }

        //플레이어가 올바른 암호화에 성공함
        StartCoroutine(PrintEncryptionSuccess_IE());
        return true;
    }
    private IEnumerator PrintEncryptionSuccess_IE()
    {
        //결과 창 이동
        this.transform.localPosition = new Vector3(-57f, 17f, 0f);
        ADFGVXGameManager.SetAvailable(false);

        Result.text = "";
        
        LJWConverter.Instance.PrintTMPByDuration(false, 0f, 0.3f, "ENCRYPT ADFGVX TEDP VER 2.0", false, Result);
       yield return new WaitForSeconds(0.75f);
        Result.text += "\n";
        LJWConverter.Instance.PrintTMPByDuration(false, 0f, 0.3f, "USER: BLACK-007 CHECK... ", false, Result);
        yield return new WaitForSeconds(0.75f);
        Result.text += "OK\n";
        LJWConverter.Instance.PrintTMPByDuration(false, 0f, 0.3f, "CONSOLE: RX-78-2 CHECK... ", false, Result);
        yield return new WaitForSeconds(0.75f);
        Result.text += "OK\n";
        LJWConverter.Instance.PrintTMPByDuration(false, 0f, 0.3f, "CONNECTION: CENTRAL TERMINAL CHECK... ", false, Result);
        yield return new WaitForSeconds(0.75f);
        Result.text += "OK\n";
        var task = string.Format("{0:D6}", Random.Range(0, 999999));
        LJWConverter.Instance.PrintTMPByDuration(false, 0f, 0.3f, "TASK " + task + " IS NOW ON OPERATION...", false, Result);
        yield return new WaitForSeconds(0.75f);
        Result.text += "\n";
        
        var before = Result.text;
        char[] gauge = new char[53];
        
        for (var i = 0; i < 53; i++)
        {
            gauge[i] = ' ';
        }
        for (var i = 0; i < 53; i++)
        {
            var builder = new StringBuilder();
            gauge[i] = '-';
            Result.text = before + "[" + builder.Append(gauge) + "]";
            yield return new WaitForSeconds(0.04f);
        }
        
        Result.text += "\n";
        LJWConverter.Instance.PrintTMPByDuration(false, 0f, 0.3f, "TASK " + task + " SUCCESS!", false, Result);
        yield return new WaitForSeconds(0.75f);
        Result.text += "\n";
        LJWConverter.Instance.PrintTMPByDuration(false, 0f, 0.3f, "암호화 파일을 저장했습니다!", false, Result);
        
        if (ADFGVXGameManager.ADFGVXTutorialManager.IsEncryptPlaying())
        {
            //튜토리얼 상황이었을 경우
            CloseButton.OnMouseUpEvent.RemoveListener(ClosePanel);
            CloseButton.OnMouseUpEvent.AddListener(() =>
            {
                //Debug.Log("암호화 튜토리얼 종료!");
                GameSystem.Instance.ClearTask("ADFGVX_ET");
                GameSystem.LoadScene("Screen");
            });
        }
        else
        {
            //암호화 태스크 클리어 마킹
            ADFGVXGameManager.Instance.encryptClear = true;
        
            //만약 복호화 태스크가 존재했고, 이미 클리어한 상태라면
            if (ADFGVXGameManager.Instance.decryptClear && ADFGVXGameManager.Instance.encryptClear)
            {
                //모든 태스크을 완료했으므로 씬에서 나갈 준비
                CloseButton.OnMouseUpEvent.RemoveListener(ClosePanel);
                CloseButton.OnMouseUpEvent.AddListener(() => {            
                    GameSystem.Instance.ClearTask("ADFGVX");
                    GameSystem.LoadScene("Screen"); });
            }
        }
        
        //결과 창 닫기 버튼 활성화
        CloseButton.SetAvailability(true);      
    }
    private IEnumerator PrintEncryptionFailed_IE(string error)
    { 
        //결과 창 이동
        this.transform.localPosition = new Vector3(-57f, 17f, 0f);
        ADFGVXGameManager.SetAvailable(false);

        Result.text = "";
        
        LJWConverter.Instance.PrintTMPByDuration(false, 0f, 0.3f, "ENCRYPT ADFGVX TEDP VER 2.0", false, Result);
       yield return new WaitForSeconds(0.75f);
        Result.text += "\n";
        LJWConverter.Instance.PrintTMPByDuration(false, 0f, 0.3f, "USER: BLACK-007 CHECK... ", false, Result);
        yield return new WaitForSeconds(0.75f);
        Result.text += "OK\n";
        LJWConverter.Instance.PrintTMPByDuration(false, 0f, 0.3f, "CONSOLE: RX-78-2 CHECK... ", false, Result);
        yield return new WaitForSeconds(0.75f);
        Result.text += "OK\n";
        LJWConverter.Instance.PrintTMPByDuration(false, 0f, 0.3f, "CONNECTION: CENTRAL TERMINAL CHECK... ", false, Result);
        yield return new WaitForSeconds(0.75f);
        Result.text += "OK\n";
        var task = string.Format("{0:D6}", Random.Range(0, 999999));
        LJWConverter.Instance.PrintTMPByDuration(false, 0f, 0.3f, "TASK " + task + " IS NOW ON OPERATION...", false, Result);
        yield return new WaitForSeconds(0.75f);
        Result.text += "\n";
        
        var before = Result.text;
        char[] gauge = new char[53];
        
        for (var i = 0; i < 53; i++)
        {
            gauge[i] = ' ';
        }
        for (var i = 0; i < 53; i++)
        {
            var builder = new StringBuilder();
            gauge[i] = '-';
            Result.text = before + "[" + builder.Append(gauge) + "]";
            yield return new WaitForSeconds(0.04f);
        }
        
        Result.text += "\n";
        LJWConverter.Instance.PrintTMPByDuration(false, 0f, 0.3f, "TASK " + task + " FAILED!", false, Result);
       yield return new WaitForSeconds(0.75f);
        Result.text += "\n";
        LJWConverter.Instance.PrintTMPByDuration(false, 0f, 0.3f, error, false, Result);

        CloseButton.SetAvailability(true);
    }
}
