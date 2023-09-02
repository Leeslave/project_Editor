using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextOff_Obs : MonoBehaviour
{
    
    [SerializeField]
    float RemoveTime;
    WaitForSeconds WFS;
    Transform Parent;
    private void Awake()
    {
        WFS = new WaitForSeconds(RemoveTime);
        Parent = transform.parent;
    }
    private void OnEnable()
    {
        StartCoroutine(Off());
    }
    IEnumerator Off()
    {
        yield return WFS;
        gameObject.SetActive(false);
        transform.SetParent(Parent);
    }
}
