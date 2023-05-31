using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputField_ADFGVX : Button_ADFGVX
{
    private string inputString;
    private bool isCursorOverInputField;
    private bool isReadyForInput;
    private bool isFlash;
    private bool skipOneFlash;

    public int InputFieldMaxLength;
    public string InputFieldName;

    protected override void Awake()
    {
        base.Awake();

        inputString = "";
        isFlash = false;
        InitInputField();
        StartFlashInputField();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (isCursorOverInputField && !isReadyForInput)
            {
                GetSTRConverter().ConvertSpriteRendererColor(0f, Exit, GetClickSprite());
                GetTMP().text = inputString;
                isReadyForInput = true;
                isFlash = false;
            }
            else if (isCursorOverInputField && isReadyForInput)
            {

            }
            else
            {
                InitInputField();
                isReadyForInput = false;
                isFlash = false;
            }
        }
    }

    protected override void OnMouseDown()
    {

    }

    protected override void OnMouseUp()
    {
    }

    protected override void OnMouseEnter()
    {
        if (!isReadyForInput)
            GetSTRConverter().ConvertSpriteRendererColor(0f, Enter, GetClickSprite());
        isCursorOverInputField = true;
    }

    protected override void OnMouseExit()
    {
        GetSTRConverter().ConvertSpriteRendererColor(0.3f, Exit, GetClickSprite());
        isCursorOverInputField = false;
    }

    public string GetInputString()//입력값 반환
    {
        return inputString;
    }

    public bool GetIsReadyForInput()//입력 가능 상태 여부 반환
    {
        return isReadyForInput;
    }

    public void SetIsReadyForInput(bool value)//입력 가능 상태 여부 설정
    {
        isReadyForInput = value;
    }

    public bool GetIsFlash()//현재 깜박임 여부 반환
    {
        return isFlash;
    }

    public void SetIsFlash(bool value)//현재 깜박임 여부 설정
    {
        isFlash = value;
    }

    public void AddInputField(string value)//입력값에 value 추가한 후 입력창 업데이트
    {
        if(inputString.Length > InputFieldMaxLength)
        {
            GameManager.InformError(InputFieldName + " 입력 불가 : 최대 입력");
            return;
        }

        inputString += value;
        GetTMP().text = inputString;
        skipOneFlash = true;

        GameManager.PlayAudioSource(ADFGVX.Audio.AddChar);
    }

    public void DeleteInputField(int length)//입력값에 length만큼 삭제 후 입력창 업데이트
    {
        if (inputString.Length == 0)
        {
            GameManager.InformError(InputFieldName + " 삭제 불가 : 최소 입력");
            return;
        }

        inputString = inputString.Substring(0, inputString.Length - length);
        GetTMP().text = inputString;
        skipOneFlash = true;

        GameManager.PlayAudioSource(ADFGVX.Audio.DeleteChar);
    }

    public void ClearInputField()//입력창 비움
    {
        inputString = "";
        GetTMP().text = inputString;
    }

    public void ExitInputField()//입력창 탈출
    {
        StopFlashInputField();
        InitInputField();
    }

    public void DisplayErrorInInputField(string value)//입력창에 에러 메세지 띄움
    {
        GetTMP().text = value;
        inputString = "";
        isReadyForInput = false;
        isFlash = false;
    }

    public void StopFlashInputField()//입력창 깜박임 정지
    {
        GetTMP().text = inputString;
        isReadyForInput = false;
        isFlash = false;
    }

    private void StartFlashInputField()//입력창 깜박임 개시
    {
        StartCoroutine(StartFlashInputField_IE());
    }

    private IEnumerator StartFlashInputField_IE()//StartFlashInputField_IEnumerator
    {
        if (inputString.Length <= InputFieldMaxLength && isReadyForInput && !skipOneFlash)
        {
            if (isFlash)
            {
                GetTMP().text = inputString;
                isFlash = false;
            }
            else
            {
                GetTMP().text = inputString + "…";
                isFlash = true;
            }
        }
        else if (!isReadyForInput && inputString != "")
            GetTMP().text = inputString;

        //한번 깜박임을 건너뛰었다면 다음에는 깜박여야 한다
        skipOneFlash = !skipOneFlash ? false : false;
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(StartFlashInputField_IE());
    }

    private void InitInputField()
    {
        if (inputString == "")
            GetTMP().text = "클릭하여 입력…";
    }
}
