using System.Collections.Generic;
using UnityEngine;

public class WorldObject : MonoBehaviour
{
    /**
    상호작용 오브젝트
    - 버튼 클릭시 해당 파라미터로 상호작용
    */
    public List<(WorldVector worldVector, Anchor anchor)> positions;
    public int positionParam;
    

    /// <summary>
    /// 초기화 시
    /// </summary>
    /// <remarks>화면 위치 설정</remarks>
    public virtual void OnAwake()
    {
        SetAnchor();
        positionParam = 0;
    }


    /// <summary>
    /// 활성화 시
    /// </summary>
    public virtual void OnEnable()
    {
        
    }
    
    
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
        var _data = positions[positionParam];
        
        // 위치 설정
        RectTransform rect = GetComponent<RectTransform>();
        rect.anchorMin = _data.anchor.GetVector();
        rect.anchorMax = _data.anchor.GetVector();
        
        // 크기 설정
        rect.sizeDelta *= _data.anchor.size;
        rect.localScale = new Vector3(1f,1f,1f);
    }
}
