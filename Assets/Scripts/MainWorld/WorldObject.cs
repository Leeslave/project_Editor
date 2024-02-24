using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class WorldObject : MonoBehaviour
{
    public int type; // 상호작용 타입
    public int count = 0;   // 상호작용 가능 횟수 (0 : 무제한)

    // 오브젝트 클릭 시
    public virtual void ObjectClicked()
    {

    }

    // 오브젝트 활성화 시
    public virtual void ObjectActive()
    {

    }

    void Awake()
    {
        // 버튼 이벤트 할당
        if (TryGetComponent(out Button bt))
        {
            bt.onClick.AddListener(ObjectClicked);
        }
    }
}
