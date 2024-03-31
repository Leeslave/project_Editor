using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class WorldObject<T> : MonoBehaviour where T : WorldObjectData 
{
    public T data;


    // 오브젝트 활성화 시
    public virtual void OnActive()
    {

    }
}
