using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class WorldObject : MonoBehaviour
{
    public World location;  // 장소
    public int position;    // 장소 내 위치
    public int count = 0;   // 상호작용 가능 횟수 (0 : 무제한)

    public string awakeParam = "";   // Active 함수 매개변수
    public string clickParam = "";   // Click 함수 매개변수

    protected WorldObject(World _location, int _position)
    {
        location = _location;
        position = _position;
    }


    // 오브젝트 클릭 시
    public virtual void OnClicked()
    {

    }

    // 오브젝트 활성화 시
    public virtual void OnActive()
    {

    }


    public virtual void Copy(WorldObject @object)
    {
        location = @object.location;
        position = @object.position;
        awakeParam = @object.awakeParam;
        clickParam = @object.clickParam;
    }

    void Awake()
    {
        // 버튼 이벤트 할당
        if (TryGetComponent(out Button bt))
        {
            bt.onClick.AddListener(OnClicked);
        }
    }

    void OnEnable()
    {
        OnActive();
    }
}
