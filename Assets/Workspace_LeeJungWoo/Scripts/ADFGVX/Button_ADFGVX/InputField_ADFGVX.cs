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

    public string GetInputString()//�˻�â �Է� ���� ��ȯ
    {
        return inputString;
    }

    public bool GetIsReadyForInput()//�˻�â �Է� ���� ����
    {
        return isReadyForInput;
    }

    public void SetIsReadyForInput(bool value)//�˻�â �Է� ���� ���� ����
    {
        isReadyForInput = value;
    }

    public bool GetIsFlash()//IsFlash ��ȯ
    {
        return isFlash;
    }

    public void SetIsFlash(bool value)//IsFlash ����
    {
        isFlash = value;
    }

    public void AddInputFieldByKeyboard(string value)//�Է�â�� Ű����� �Է�
    {
        if (!GetIsReadyForInput())
            return;

        if(inputString.Length > InputFieldMaxLength)
        {
            GameManager.InformError(InputFieldName + " �ִ� �Է� : �Է� �Ұ�");
            return;
        }

        inputString += value;
        SetMarkText(inputString);
        skipOneFlash = true;

        GameManager.PlayAudioSource(ADFGVX.Audio.AddChar);
    }

    public void AddInputFieldByButton(string value)//�Է�â�� ��ư���� �Է�
    {
        if (inputString.Length > InputFieldMaxLength)
        {
            GameManager.InformError(InputFieldName + " �ִ� �Է� : �Է� �Ұ�");
            return;
        }

        inputString += value;
        SetMarkText(inputString);
        skipOneFlash = true;

        GameManager.PlayAudioSource(ADFGVX.Audio.AddChar);
    }

    public void DeleteInputFieldByKeyboard(int length)//�Է�â�� Ű����� ����
    {
        if (!GetIsReadyForInput())
            return;

        if (inputString.Length == 0)
        {
            GameManager.InformError(InputFieldName + " �ּ� �Է� : ���� �Ұ�");
            return;
        }

        inputString = inputString.Substring(0, inputString.Length - length);
        SetMarkText(inputString);
        skipOneFlash = true;

        GameManager.PlayAudioSource(ADFGVX.Audio.DeleteChar);
    }

    public void DeleteInputFieldByButton(int length)//�Է�â�� ��ư���� ����
    {
        if (inputString.Length == 0)
        {
            GameManager.InformError(InputFieldName + " �ּ� �Է� : ���� �Ұ�");
            return;
        }

        inputString = inputString.Substring(0, inputString.Length - length);
        SetMarkText(inputString);
        skipOneFlash = true;

        GameManager.PlayAudioSource(ADFGVX.Audio.DeleteChar);
    }

    public void ClearInputFieldByButton()//�Է�â�� ��ư���� ���
    {
        inputString = "";
        SetMarkText("");
    }

    public void ReturnInputFieldByKeyboard()//�˻�â�� Ű����� ����
    {
        if (!GetIsReadyForInput())
            return;

        StopFlashInputField();
        InitInputField();
    }

    public void DisplayErrorInInputField(string value)//�˻�â�� ���� �޼����� ����
    {
        SetMarkText(value);
        inputString = "";
        isReadyForInput = false;
        isFlash = false;
    }

    public void StopFlashInputField()//�˻�â ���ð� �������� ��Ȱ��ȭ�Ѵ�
    {
        SetMarkText(inputString);
        isReadyForInput = false;
        isFlash = false;
    }

    private void StartFlashInputField()//�˻�â�� �����̰� �����
    {
        StartCoroutine(FlashInputFieldIEnumerator());
    }

    private IEnumerator FlashInputFieldIEnumerator()
    {
        if (inputString.Length <= InputFieldMaxLength && isReadyForInput && !skipOneFlash)//�Է�â ���̸� �ѱ�ų�, �Է� ���� �ƴϰų�, ��ŵ ����� �ִٸ� �ǳʶڴ�
        {
            if (isFlash)
            {
                SetMarkText(inputString);
                isFlash = false;
            }
            else
            {
                SetMarkText(inputString + "��");
                isFlash = true;
            }
        }
        else if (!isReadyForInput && inputString != "")//�Է� ���� �ƴϳ� ��ĭ�� �ƴ϶��, �Է� ���¸� �����Ѵ�
            SetMarkText(inputString);

        //�̹� �Ͽ� ��ŵ������ ���� ������ ���ڿ��� �Ѵ�
        skipOneFlash = !skipOneFlash ? false : false;
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(FlashInputFieldIEnumerator());
    }

    private void InitInputField()//�˻�â�� �ʱ�ȭ�Ѵ�
    {
        if (inputString == "")
            SetMarkText("Ŭ���Ͽ� �Է¡�");
    }
}
