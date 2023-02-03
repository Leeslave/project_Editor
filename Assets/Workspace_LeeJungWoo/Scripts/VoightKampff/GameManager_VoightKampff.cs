using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager_VoightKampff : MonoBehaviour
{
    private Chat chat;

    private GameObject addressPart;
    private GameObject namePart;
    private GameObject familyPart;
    private GameObject propertyPart;
    private GameObject careerPart;

    private void Start()
    {
        //ä�� UI�� ����
        chat = GameObject.Find("Chat").GetComponent<Chat>();

        //�� ��Ʈ Ȯ��
        addressPart = GameObject.Find("Address");
        namePart = GameObject.Find("Name");
        familyPart = GameObject.Find("Family");
        propertyPart = GameObject.Find("Property");
        careerPart = GameObject.Find("Career");

        //�Է� ���� ����
        SetLayer(2, 2, 2, 2, 2);
    }

    public void SetLayer(int layer_Address, int layer_Name, int layer_family, int layer_property, int layer_career)//��� �Է� ����
    {
        addressPart.layer = layer_Address;
        namePart.layer = layer_Name;
        familyPart.layer = layer_family;
        propertyPart.layer = layer_property;
        careerPart.layer = layer_career;
        
        Debug.Log("[Function : SetLayer] addressPart : " + layer_Address);
        Debug.Log("[Function : SetLayer] namePart : " + layer_Name);
        Debug.Log("[Function : SetLayer] familyPart : " + layer_family);
        Debug.Log("[Function : SetLayer] propertyPart : " + layer_property);
        Debug.Log("[Function : SetLayer] careerPart : " + layer_career);
    }

    public void OnNodeDown(int line)
    {
        //�Է� ���� ����
        SetLayer(2, 2, 2, 2, 2);
        chat.OnCharOrObjDown(line);
    }
}
