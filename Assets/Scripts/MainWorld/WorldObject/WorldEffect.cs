using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class WorldEffect : WorldObject<EffectData>
{   
    /**
    대사 실행 오브젝트
    - 타입에 따라 타이밍 맞춰 대사 실행
    */
    public override void OnActive()
    {
        switch (data.objType)
        {
            case ObjectType.BGM:
                Debug.Log(data.awakeParam);
                SetBGM(int.Parse(data.awakeParam));
                break;
            case ObjectType.Block:
                Debug.Log($"Block : {data.awakeParam}");
                BlockButton(int.Parse(data.awakeParam));
                break;
            default:
                return;
        }
    }


    private void SetBGM(int bgm)
    {
        location.SetBGM(bgm);
    }


    private void BlockButton(int idx)
    {
        Destroy(location.buttons[idx]);
    }
}