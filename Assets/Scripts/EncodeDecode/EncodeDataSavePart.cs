using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EncodeDataSavePart : MonoBehaviour
{
    private ADFGVX adfgvx;

    private Button_ADFGVX_ChangeSecurityLevel m_SecurityLevel;
    private InputField_ADFGVX m_EncodedData;
    private InputField_ADFGVX m_FileTitle;
    private TextMeshPro m_CreatingDate;
    private TextMeshPro m_CreatingDateUI;
    private TextMeshPro m_Creater;
    private TextMeshPro m_CreaterUI;
    private Button_ADFGVX_SaveEncodedData m_EncodedDataSave;

    private void Awake()
    {
        adfgvx = GameObject.Find("GameManager").GetComponent<ADFGVX>();

        m_CreatingDateUI = transform.GetChild(0).GetComponent<TextMeshPro>();
        m_CreatingDate = transform.GetChild(1).GetComponent<TextMeshPro>();
        m_CreaterUI = transform.GetChild(2).GetComponent<TextMeshPro>();
        m_Creater = transform.GetChild(3).GetComponent<TextMeshPro>();
        m_SecurityLevel = transform.GetChild(11).GetComponent<Button_ADFGVX_ChangeSecurityLevel>();
        m_EncodedData = transform.GetChild(12).GetComponent<InputField_ADFGVX>();
        m_FileTitle = transform.GetChild(13).GetComponent<InputField_ADFGVX>();
        m_EncodedDataSave = transform.GetChild(14).GetComponent<Button_ADFGVX_SaveEncodedData>();
    }

    public void SetLayer(int layer)//하위 요소의 입력 제어
    {
        m_FileTitle.gameObject.layer = layer;
        m_EncodedData.gameObject.layer = layer;
        m_SecurityLevel.gameObject.layer = layer;
        m_EncodedDataSave.transform.gameObject.layer = layer;
        if(layer==2)
        {
            m_FileTitle.SetIsReadyForInput(false);
            m_FileTitle.SetIsFlash(false);
            m_EncodedData.SetIsReadyForInput(false);
            m_EncodedData.SetIsFlash(false);
        }
    }

    public InputField_ADFGVX GetInputField_Title()//제목 입력창 반환
    {
        return m_FileTitle;
    }

    public InputField_ADFGVX GetInputField_Data()//데이터 입력창 반환
    {
        return m_EncodedData;
    }

    public string GetSecurityLevel()//보안 등급 반환
    {
        return m_SecurityLevel.GetTMP().text;
    }
}
