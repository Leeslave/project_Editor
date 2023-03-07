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

    public void SetLayer(int layer)//이 게임오브젝트 하위 요소의 레이어 제어
    {
        transform.Find("Title").gameObject.layer = layer;
        transform.Find("Data").gameObject.layer = layer;
        transform.Find("SecurityLevel").gameObject.layer = layer;
    }

    public void UnvisiblePart()//파트 비가시
    {
        this.gameObject.transform.localPosition = new Vector3(70.7f, -300, 0);
    }

    public void VisiblePart()//파트 가시
    {
        this.gameObject.transform.localPosition = new Vector3(70.7f, -67.9f, 0);
    }

    public InputField_ADFGVX GetInputField_Title()//제목 인풋 필드 반환
    {
        return title;
    }

    public InputField_ADFGVX GetInputField_Data()//내용 인풋 필드 반환
    {
        return data;
    }

    public string GetSecurityLevel()//보안 등급 레벨 반환
    {
        return level.GetMarkText();
    }
}
