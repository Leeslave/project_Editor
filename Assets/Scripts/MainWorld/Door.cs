using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public World dest;
    public int destPos;

    public virtual void OnClick()
    {
        // 월드 매니저 오류
        if (WorldSceneManager.Instance == null)
        {
            throw new Exception($"World Manager Missed!! : Door");
        }

        // 지역 이동 실행
        GameSystem.Instance.gameData.SetPosition(destPos);
        WorldSceneManager.Instance.MoveLocation(dest);
    }
}
