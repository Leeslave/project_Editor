using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Opening : MonoBehaviour
{
    public TMP_Text PE;
    public PatternManager PM;
    public bool CCCnt = true;

    private void Start()
    {
        StartCoroutine(Cntt());
    }
    private void Update()
    {
        if (Input.anyKey && CCCnt)
        {
            CCCnt = false;
            StopAllCoroutines();
            StartCoroutine(Cnt());
        }
    }
    IEnumerator Cnt()
    {
        for(int i = 100; i >= 0; i--)
        {
            for(int x = 1; x <= 2; x++)
                transform.GetChild(x).GetComponent<TMP_Text>().color = new Color(1, 1, 1, i * 0.01f);
            for(int x = 3; x <= 5; x++)
                transform.GetChild(x).GetComponent<Image>().color = new Color(1, 1, 1, i * 0.01f);
            yield return new WaitForSeconds(0.03f);
        }
        PM.StartInit();
        Destroy(gameObject);
    }

    IEnumerator Cntt()
    {
        while (true)
        {
            for (int i = 100; i >= 0; i--)
            {
                PE.color = new Color(1, 1, 1, i * 0.01f);
                yield return new WaitForSeconds(0.01f);
            }
            for (int i = 0; i <= 100; i++)
            {
                PE.color = new Color(1, 1, 1, i * 0.01f);
                yield return new WaitForSeconds(0.01f);
            }
        }
    }
}
