using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChatEditor : MonoBehaviour
{
    public List<Paragraph> data;
    
    [Header("데이터 수정 오브젝트")] 
    public GameObject talkPrefab;
    public GameObject choicePrefab;

    public void OnEnable()
    {
        RefreshList();
    }


    public void RefreshList()
    {
         
    }
}
