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

    protected override void Start()
    {
        base.Start();

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
                SetClickSpriteColor(Exit);
                SetMarkText(inputString);
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
            SetClickSpriteColor(Enter);
        isCursorOverInputField = true;
    }

    protected override void OnMouseExit()
    {
        ConvertClickSpriteColor(Exit);
        isCursorOverInputField = false;
    }

    public string GetInputString()//검색창 입력 내용 반환
    {
        return inputString;
    }

    public bool GetIsReadyForInput()//검색창 입력 가능 상태
    {
        return isReadyForInput;
    }

    public void SetIsReadyForInput(bool value)//검색창 입력 가능 상태 제어
    {
        isReadyForInput = value;
    }

    public bool GetIsFlash()//IsFlash 반환
    {
        return isFlash;
    }

    public void SetIsFlash(bool value)//IsFlash 설정
    {
        isFlash = value;
    }

    public void AddInputFieldByKeyboard(string value)//입력창에 키보드로 입력
    {
        if (!GetIsReadyForInput())
            return;

        if(inputString.Length > InputFieldMaxLength)
        {
            GameManager.InformError(InputFieldName + " 최대 입력 : 입력 불가");
            return;
        }

        inputString += value;
        SetMarkText(inputString);
        skipOneFlash = true;

        GameManager.PlayAudioSource(ADFGVX.Audio.AddChar);
    }

    public void AddInputFieldByButton(string value)//입력창에 버튼으로 입력
    {
        if (inputString.Length > InputFieldMaxLength)
        {
            GameManager.InformError(InputFieldName + " 최대 입력 : 입력 불가");
            return;
        }

        inputString += value;
        SetMarkText(inputString);
        skipOneFlash = true;

        GameManager.PlayAudioSource(ADFGVX.Audio.AddChar);
    }

    public void DeleteInputFieldByKeyboard(int length)//입력창에 키보드로 삭제
    {
        if (!GetIsReadyForInput())
            return;

        if (inputString.Length == 0)
        {
            GameManager.InformError(InputFieldName + " 최소 입력 : 삭제 불가");
            return;
        }

        inputString = inputString.Substring(0, inputString.Length - length);
        SetMarkText(inputString);
        skipOneFlash = true;

        GameManager.PlayAudioSource(ADFGVX.Audio.DeleteChar);
    }

    public void DeleteInputFieldByButton(int length)//입력창에 버튼으로 삭제
    {
        if (inputString.Length == 0)
        {
            GameManager.InformError(InputFieldName + " 최소 입력 : 삭제 불가");
            return;
        }

        inputString = inputString.Substring(0, inputString.Length - length);
        SetMarkText(inputString);
        skipOneFlash = true;

        GameManager.PlayAudioSource(ADFGVX.Audio.DeleteChar);
    }

    public void ClearInputFieldByButton()//입력창에 버튼으로 비움
    {
        inputString = "";
        SetMarkText("");
    }

    public void ReturnInputFieldByKeyboard()//검색창에 키보드로 엔터
    {
        if (!GetIsReadyForInput())
            return;

        StopFlashInputField();
        InitInputField();
    }

    public void DisplayErrorInInputField(string value)//검색창에 에러 메세지를 띄운다
    {
        SetMarkText(value);
        inputString = "";
        isReadyForInput = false;
        isFlash = false;
    }

    public void StopFlashInputField()//검색창 선택과 깜박임을 비활성화한다
    {
        SetMarkText(inputString);
        isReadyForInput = false;
        isFlash = false;
    }

    private void StartFlashInputField()//검색창을 깜박이게 만든다
    {
        StartCoroutine(FlashInputFieldIEnumerator());
    }

    private IEnumerator FlashInputFieldIEnumerator()
    {
        if (inputString.Length <= InputFieldMaxLength && isReadyForInput && !skipOneFlash)//입력창 길이를 넘기거나, 입력 중이 아니거나, 스킵 명령이 있다면 건너뛴다
        {
            if (isFlash)
            {
                SetMarkText(inputString);
                isFlash = false;
            }
            else
            {
                SetMarkText(inputString + "…");
                isFlash = true;
            }
        }
        else if (!isReadyForInput && inputString != "")//입력 중이 아니나 빈칸이 아니라면, 입력 상태를 유지한다
            SetMarkText(inputString);

        //이번 턴에 스킵했으니 다음 번에는 깜박여야 한다
        skipOneFlash = !skipOneFlash ? false : false;
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(FlashInputFieldIEnumerator());
    }

    private void InitInputField()//검색창을 초기화한다
    {
        if (inputString == "")
            SetMarkText("클릭하여 입력…");
    }
}
