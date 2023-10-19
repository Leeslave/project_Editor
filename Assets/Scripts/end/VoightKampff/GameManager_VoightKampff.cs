using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager_VoightKampff : MonoBehaviour
{
    private Chat_VoightKampff chat;
    private Eye eye;

    private GameObject addressPart;
    private GameObject namePart;
    private GameObject familyPart;
    private GameObject propertyPart;
    private GameObject careerPart;    
    private GameObject suspecionPart;

    [Header("디버그 로그 창")]
    public DebugLog debugLog;

    private void Start()
    {
        chat = GameObject.Find("Chat_VoightKampff").GetComponent<Chat_VoightKampff>();

        eye = GameObject.Find("Eye").GetComponent<Eye>();

        addressPart = GameObject.Find("Address");
        namePart = GameObject.Find("Name");
        familyPart = GameObject.Find("Family");
        propertyPart = GameObject.Find("Property");
        careerPart = GameObject.Find("Career");
        suspecionPart = GameObject.Find("Suspecion");

        //입력 차단
        SetLayer(2, 2, 2, 2, 2, 2, 2);

        debugLog.DebugInfo("보이트 캄프 테스트 머신 정상 작동");
    }

    public Chat_VoightKampff GetChat()
    {
        return chat;
    }

    public Eye GetEye()
    {
        return eye;
    }

    public void SetLayer(int layer_Address, int layer_Name, int layer_Family, int layer_Property, int layer_Career, int layer_Suspecion, int layer_DebugLog)//하위 파트의 모든 입력 제어
    {
        addressPart.layer = layer_Address;
        namePart.layer = layer_Name;
        familyPart.layer = layer_Family;
        propertyPart.layer = layer_Property;
        careerPart.layer = layer_Career;
        suspecionPart.layer = layer_Suspecion;
        debugLog.SetLayer(layer_DebugLog);
        
        Debug.Log("[Function : SetLayer] addressPart : " + layer_Address);
        Debug.Log("[Function : SetLayer] namePart : " + layer_Name);
        Debug.Log("[Function : SetLayer] familyPart : " + layer_Family);
        Debug.Log("[Function : SetLayer] propertyPart : " + layer_Property);
        Debug.Log("[Function : SetLayer] careerPart : " + layer_Career);
        Debug.Log("[Function : SetLayer] suspecionPart : " + layer_Suspecion);
        Debug.Log("[Function : SetLayer] DebugLog : " + layer_DebugLog);
    }

    public void OnNodeDown(int line)
    {
        //입력 차단
        SetLayer(2, 2, 2, 2, 2, 2, 2);
        chat.LoadLine(line);
    }
}
