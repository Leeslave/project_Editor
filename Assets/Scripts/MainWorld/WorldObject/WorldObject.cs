using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class WorldObject<T> : MonoBehaviour where T : WorldObjectData 
{
    public T data;
    public bool playAwake = false;
    public int count = 0;   // 상호작용 횟수
    public Location location;


    // 오브젝트 활성화 시
    public virtual void OnActive()
    {
        
    }
}
