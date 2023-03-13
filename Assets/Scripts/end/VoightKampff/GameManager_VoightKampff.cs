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

    [Header("����� �α�")]
    public DebugLog debugLog;

    private void Start()
    {
        //ä�� UI�� ����
        chat = GameObject.Find("Chat_VoightKampff").GetComponent<Chat_VoightKampff>();

        //Eye�� ����
        eye = GameObject.Find("Eye").GetComponent<Eye>();

        //�� ��Ʈ Ȯ��
        addressPart = GameObject.Find("Address");
        namePart = GameObject.Find("Name");
        familyPart = GameObject.Find("Family");
        propertyPart = GameObject.Find("Property");
        careerPart = GameObject.Find("Career");
        suspecionPart = GameObject.Find("Suspecion");

        //�Է� ���� ����
        SetLayer(2, 2, 2, 2, 2, 2, 2);

        debugLog.DebugInfo("���̵� į�� �׽�Ʈ ���� ���α׷� ���� ����");
    }

    public Chat_VoightKampff GetChat()//chat ��ȯ
    {
        return chat;
    }

    public Eye GetEye()//eye ��ȯ
    {
        return eye;
    }

    public void SetLayer(int layer_Address, int layer_Name, int layer_Family, int layer_Property, int layer_Career, int layer_Suspecion, int layer_DebugLog)//��� �Է� ����
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
        //�Է� ���� ����
        SetLayer(2, 2, 2, 2, 2, 2, 2);
        chat.LoadLine(line);
    }
}
