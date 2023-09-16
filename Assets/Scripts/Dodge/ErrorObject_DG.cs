using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ErrorObject_DG : MonoBehaviour
{
    RectTransform[] TR = new RectTransform[4];

    WaitForSeconds wfs = new WaitForSeconds(0.5f);

    [SerializeField] Color[] cnts;

    private void Awake()
    {
        for (int i = 0; i < 4; i++) TR[i] = transform.GetChild(i).GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        
    }

    IEnumerator SS()
    {
        int i;
        for (i = 0; i < 2; i++) TR[Random.Range(0, 3)].SetAsLastSibling();


        yield return wfs;
    }


}
