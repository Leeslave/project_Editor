using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public World dest = World.NullMax;
    public int destPos = -1;

    public virtual void OnClick()
    {
        // 월드 매니저 오류
        if (WorldSceneManager.Instance == null)
        {
            throw new Exception($"World Manager Missed!! : Door");
        }

        // 지역 이동 실행
        if (destPos >= 0)
        {
            // GameSystem.Instance.gameData.SetPosition(destPos);
        }
        if (dest != World.NullMax)
        {
            WorldSceneManager.Instance.MoveLocation(dest);
        }
    }
}
