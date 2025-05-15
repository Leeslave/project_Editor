using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class ChatEditor : MonoBehaviour
{
    public List<Paragraph> dataList;
    
    [Header("데이터 수정 오브젝트")] 
    public GameObject talkPrefab;
    public GameObject choicePrefab;
    public Transform dataPanel;



    /// <summary>
    /// 대사 리스트 새로고침
    /// </summary>
    /// <remarks>대사 리스트에 있는 버튼들을 데이터 순서에 맞춰서 새로 생성</remarks>
    public void RefreshList(List<Paragraph> newData = null)
    {
         // 기존 버튼들 삭제
         foreach (var obj in dataPanel.GetComponentsInChildren<Transform>().Where(t => t != dataPanel.transform))
         {
             Destroy(obj.gameObject);
         }

         if (newData?.Count != 0)
         {
             dataList = newData;
         }
         
         // 데이터에 맞춰 버튼들 생성
         if (dataList?.Count == 0) return;
         foreach (var data in dataList)
         {
             GameObject newObj;
             if (data is TalkParagraph)
             {
                 newObj = Instantiate(talkPrefab, dataPanel);
             }
             else if (data is ChoiceParagraph)
             {
                 newObj = Instantiate(choicePrefab, dataPanel);
             }
             else
                 return;

             TMP_Text[] texts = newObj.transform.GetComponentsInChildren<TMP_Text>();
             texts[0].text = dataPanel.childCount.ToString();
         }
    }
}
