using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class NPC : WorldObject<NPCData>
{   
    /**
    대사 실행 오브젝트
    - 타입에 따라 타이밍 맞춰 대사 실행
    */
    public override void OnActive()
    {
        data.awakeTalk = null;
        if (data.awakeParam is not "")
        {
            data.awakeTalk = DataLoader.GetChatData(data.awakeParam);
        }
        data.clickTalk = null;
        if (data.clickParam is not "")
        {
            data.clickTalk = DataLoader.GetChatData(data.clickParam);
        }

        InitChat(data.awakeTalk);
    }


    public void OnClicked()
    {
        InitChat(data.clickTalk);
    }


    public void SetPosition()
    {
        // 위치 설정
        RectTransform rect = GetComponent<RectTransform>();
        rect.anchorMin = data.anchor;
        rect.anchorMax = data.anchor;
        
        // 크기 설정
        Debug.Log(data.size);
        rect.sizeDelta *= data.size;
        rect.localScale = new Vector3(1f,1f,1f);
    }

    private void InitChat(List<Paragraph> para)
    {
        if (para == null)
        {
            return;
        }
        
        if (data.count == 0 || data.count > count)
        {
            count++;
            Chat.Instance.StartChat(para);
        }
    }
}
