using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EncodeDataSavePart : MonoBehaviour
{
    private ADFGVX adfgvx;

    private InputField title;
    private InputField data;
    private Button_SecurityLevel level;

    private void Start()
    {
        adfgvx = GameObject.Find("ADFGVX").GetComponent<ADFGVX>();

        title = transform.Find("Title").GetComponent<InputField>();
        data = transform.Find("Data").GetComponent<InputField>();
        level = transform.Find("SecurityLevel").GetComponent<Button_SecurityLevel>();
    }

    public void SetLayer(int layer)//��� �Է� ����
    {
        this.gameObject.layer = layer;
        title.gameObject.layer = layer;
        data.gameObject.layer = layer;
        level.gameObject.layer = layer;
    }

    public void UnvisiblePart()//��Ʈ �񰡽�
    {
        this.gameObject.transform.localPosition = new Vector3(70.7f, -300, 0);
    }

    public void VisiblePart()//��Ʈ ����
    {
        this.gameObject.transform.localPosition = new Vector3(70.7f, -67.9f, 0);
    }

    public InputField GetInputField_Title()//���� ��ǲ �ʵ� ��ȯ
    {
        return title;
    }

    public InputField GetInputField_Data()//���� ��ǲ �ʵ� ��ȯ
    {
        return data;
    }

    public Button_SecurityLevel GetButton_SecurityLevel()//���� ��� �ؽ�Ʈ �ʵ� ��ȯ
    {
        return level;
    }

    public void AddInputField_DataByKeyboard(string value)
    {
        data.AddInputFieldByKeyboard(value + " ");
    }
}
