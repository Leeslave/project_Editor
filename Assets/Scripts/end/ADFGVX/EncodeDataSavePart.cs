using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EncodeDataSavePart : MonoBehaviour
{
    private ADFGVX adfgvx;

    private InputField_ADFGVX title;
    private InputField_ADFGVX data;
    private Button_ADFGVX_ChangeSecurityLevel level;

    private void Start()
    {
        adfgvx = GameObject.Find("GameManager").GetComponent<ADFGVX>();

        title = transform.Find("Title").GetComponent<InputField_ADFGVX>();
        data = transform.Find("Data").GetComponent<InputField_ADFGVX>();
        level = transform.Find("SecurityLevel").GetComponent<Button_ADFGVX_ChangeSecurityLevel>();
    }

    public void SetLayer(int layer)//�� ���ӿ�����Ʈ ���� ����� ���̾� ����
    {
        transform.Find("Title").gameObject.layer = layer;
        transform.Find("Data").gameObject.layer = layer;
        transform.Find("SecurityLevel").gameObject.layer = layer;
    }

    public void UnvisiblePart()//��Ʈ �񰡽�
    {
        this.gameObject.transform.localPosition = new Vector3(70.7f, -300, 0);
    }

    public void VisiblePart()//��Ʈ ����
    {
        this.gameObject.transform.localPosition = new Vector3(70.7f, -67.9f, 0);
    }

    public InputField_ADFGVX GetInputField_Title()//���� ��ǲ �ʵ� ��ȯ
    {
        return title;
    }

    public InputField_ADFGVX GetInputField_Data()//���� ��ǲ �ʵ� ��ȯ
    {
        return data;
    }

    public string GetSecurityLevel()//���� ��� ���� ��ȯ
    {
        return level.GetMarkText();
    }
}
