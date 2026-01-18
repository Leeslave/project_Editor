using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class WorldObject : MonoBehaviour
{
    /**
    상호작용 오브젝트
    - 버튼 클릭시 해당 파라미터로 상호작용
    */
    public List<(WorldVector worldVector, Anchor anchor)> positions = new();
    public int positionParam;
    

    /// <summary>
    /// 초기화 시
    /// </summary>
    /// <remarks>화면 위치 설정</remarks>
    public virtual void Init()
    {
        if (positions.Count > 0)
        {
            SetAnchor();
        }
        positionParam = 0;
    }

    public abstract void OnBecameVisible();

    /// <summary>
    /// 클릭 시
    /// </summary>
    public virtual void OnClick()
    {
        
    }

    
    /// <summary>
    /// 위치값에 맞춰 앵커 설정
    /// </summary>
    private void SetAnchor()
    {
        var (vector, anchor) = positions[positionParam];
        
        // 위치 설정
        int x = (int)vector.location * 1000 + vector.position * 100;
        transform.position = new Vector3(x, 0, 0);
        transform.position += new Vector3(anchor.x * 50, anchor.y * 50, 0);
        
        // 크기 설정
        transform.localScale *= anchor.size;
    }
}
