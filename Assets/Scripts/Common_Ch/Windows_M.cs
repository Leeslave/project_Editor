using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Windows_M : MonoBehaviour
{
    [SerializeField] GameObject[] Icons;
    RectTransform[] Rects;
    Image[] Images;

    private void Awake()
    {
        int z = Icons.Length;
        Rects = new RectTransform[z];
        Images = new Image[z];
        for(int i = 0; i < Icons.Length; i++)
        {
            Rects[i] = Icons[i].GetComponent<RectTransform>();
            Images[i] = Icons[i].GetComponent<Image>();
        }
    }
}
