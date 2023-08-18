using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DumpedIcon : MonoBehaviour
{
    GameObject BIcon;
    Image MyImage;
    TMP_Text MyName;
    [SerializeField] RectTransform MyRect;

    private void Awake()
    {
        MyImage = transform.GetChild(0).GetComponent<Image>();
        MyName = transform.GetChild(1).GetComponent<TMP_Text>();
    }

    public void Dumped(GameObject Before,Sprite Image, string Name)
    {
        BIcon = Before;
        MyImage.sprite = Image;
        MyName.text = Name;
        gameObject.SetActive(true);
        LayoutRebuilder.ForceRebuildLayoutImmediate(MyRect);
    }
    void DumpedCancle()
    {
        gameObject.SetActive(false);
        BIcon.SetActive(true);
    }
}
