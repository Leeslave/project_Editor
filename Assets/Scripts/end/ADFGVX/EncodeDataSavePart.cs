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

    public void SetLayer(int layer)//하위 요소의 입력 제어
    {
        transform.Find("Title").gameObject.layer = layer;
        transform.Find("Data").gameObject.layer = layer;
        transform.Find("SecurityLevel").gameObject.layer = layer;
    }

    public InputField_ADFGVX GetInputField_Title()//제목 입력창 반환
    {
        return title;
    }

    public InputField_ADFGVX GetInputField_Data()//데이터 입력창 반환
    {
        return data;
    }

    public string GetSecurityLevel()//보안 등급 반환
    {
        return level.GetMarkText();
    }
}
