using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EncodeDataSavePart : MonoBehaviour
{
    private ADFGVX adfgvx;

    private Button_ADFGVX_ChangeSecurityLevel securityLevel;
    private InputField_ADFGVX data;
    private InputField_ADFGVX title;
    private TextField date;
    private TextField dateUI;
    private TextField sender;
    private TextField senderUI;
    private Button_ADFGVX_SaveEncodedData save;

    private void Awake()
    {
        adfgvx = GameObject.Find("GameManager").GetComponent<ADFGVX>();

        securityLevel = transform.GetChild(2).GetComponent<Button_ADFGVX_ChangeSecurityLevel>();
        data = transform.GetChild(3).GetComponent<InputField_ADFGVX>();
        title = transform.GetChild(4).GetComponent<InputField_ADFGVX>();
        date = transform.GetChild(5).GetComponent<TextField>();
        dateUI = transform.GetChild(6).GetComponent<TextField>();
        sender = transform.GetChild(7).GetComponent<TextField>();
        senderUI = transform.GetChild(8).GetComponent<TextField>();
        save = transform.GetChild(9).GetComponent<Button_ADFGVX_SaveEncodedData>();
    }

    public void SetLayer(int layer)//하위 요소의 입력 제어
    {
        title.gameObject.layer = layer;
        data.gameObject.layer = layer;
        securityLevel.gameObject.layer = layer;
        save.transform.gameObject.layer = layer;
        if(layer==2)
        {
            title.SetIsReadyForInput(false);
            title.SetIsFlash(false);
            data.SetIsReadyForInput(false);
            data.SetIsFlash(false);
        }
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
        return securityLevel.GetTMP().text.ToString();
    }
}
