using System;
using UnityEngine;

public class Door : MonoBehaviour
{
    /**
     * 클릭시 지정된 Location으로 이동
     * - 월드 매니저에 이동 호출
     */
    
    public World dest = World.Street;
    public int destPos;

    public virtual void OnClick()
    {
        // 월드 매니저 오류
        if (WorldSceneManager.Instance == null)
        {
            throw new Exception($"World Manager Missed!! : Door");
        }

        WorldSceneManager.Instance.MoveLocation(dest, destPos);
    }
}
