using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FilterBug : MonoBehaviour
{
    bool Sub = false;
    WaitForSeconds wfs = new WaitForSeconds(0.01f);
    private void OnEnable()
    {
        if (Sub) { Sub = false; return; }
        else StartCoroutine(Fixer());
    }

    IEnumerator Fixer()
    {
        yield return wfs;
        gameObject.SetActive(false);
        gameObject.SetActive(true);
    }
}
