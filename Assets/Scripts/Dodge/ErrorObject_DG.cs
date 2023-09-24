using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ErrorObject_DG : MonoBehaviour
{
    RectTransform[] TR = new RectTransform[4];
    Image[] IM = new Image[4];

    WaitForSeconds wfs = new WaitForSeconds(0.3f);

    [SerializeField] Color[] cnts;

    private void Awake()
    {
        for (int i = 0; i < 4; i++)
        {
            TR[i] = transform.GetChild(i).GetComponent<RectTransform>();
            IM[i] = TR[i].GetComponent<Image>();
        }
    }

    private void OnEnable()
    {
        transform.position = new Vector3(Random.Range(-11,11),Random.Range(-9,-1),0);
        StartCoroutine(SS());
    }

    IEnumerator SS()
    {
        int i,s;
        while (true)
        {
            s = Random.Range(0, 4);
            for (i = 0; i < 2; i++)TR[Random.Range(0, 4)].SetAsLastSibling();
            for(i = 0; i < 4; i++)
            {
                TR[i].sizeDelta = new Vector2(Random.Range(0.2f,0.4f),Random.Range(0.2f,0.4f));
                IM[i].color = cnts[(i + s) % 4];
            }

            yield return wfs;
        }
    }
 }
