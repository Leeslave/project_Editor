using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIObjectManagement : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 오브젝트의 활성화/비활성화 전환
    /// </summary>
    public void ObjectOnOff()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }
}
