using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OutLineReinf : MonoBehaviour
{
    [SerializeField] bool OnOffType = true;
    Material Mat;
    RectTransform Rect;
    private void Awake()
    {
        Mat = GetComponent<Image>().material;
        Rect = GetComponent<RectTransform>();
    }

    private void FixedUpdate()
    {
        if(!OnOffType) Mat.SetFloat("_Ratio", Rect.rect.width / Rect.rect.height);
    }

    private void OnEnable()
    {
        Mat.SetFloat("_Ratio",Rect.rect.width / Rect.rect.height);
    }
}
