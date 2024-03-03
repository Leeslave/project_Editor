using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine.UI;
using Newtonsoft.Json;
using UnityEngine;

public class NPC : WorldObject
{   
    /**
    대사 실행 오브젝트
    - 타입에 따라 타이밍 맞춰 대사 실행
    */

    public string image;
    public Vector2 pos;
    public Vector2 size;

    public List<Paragraph>[] talkData = new List<Paragraph>[2];    // NPC 데이터
    public int talkCount = 0;   // 대화 횟수 카운트


    public NPC(int _location, int _position, string _awakeParam, string _clickParam) : base(_location, _position)
    {
        awakeParam = _awakeParam;
        clickParam = _clickParam;
    }


    public override void ObjectActive()
    {
        talkData[0] = null;
        if (awakeParam is not "")
        {
            talkData[0] = Chat.Instance.GetChatData(awakeParam);
        }

        talkData[1] = null;
        if (clickParam is not "")
        {
            talkData[1] = Chat.Instance.GetChatData(clickParam);
        }

        if (talkData[0] is not null) Chat.Instance.StartChat(talkData[0]);
    }


    public override void ObjectClicked()
    {
        if (talkData[1] is not null)
        {
            Chat.Instance.StartChat(talkData[1]);
        }
    }

    public override void Copy(WorldObject @object)
    {
        base.Copy(@object);

        if (@object is not NPC)
            return;

        NPC npc = @object as NPC;
        image = npc.image;
        pos = npc.pos;
        size = npc.size;
        talkData = new List<Paragraph>[2];
        talkData[0] = new();
        talkData[0].AddRange(npc.talkData[0]);
        talkData[1] = new();
        talkData[1].AddRange(npc.talkData[1]);
        talkCount = npc.talkCount;
    }
}
