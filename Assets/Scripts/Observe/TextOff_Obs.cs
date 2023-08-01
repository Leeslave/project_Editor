using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextOff_Obs : MonoBehaviour
{
    
    [SerializeField]
    float RemoveTime;
    WaitForSeconds WFS;
    private void Awake()
    {
        WFS = new WaitForSeconds(RemoveTime);
    }
    private void OnEnable()
    {
        StartCoroutine(Off());
    }
    IEnumerator Off()
    {
        yield return WFS;
        gameObject.SetActive(false);
    }
}
