using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class ImageAttribute
{
    public Sprite image { get; private set; }
    [SerializeField]
    private Sprite baseImage;

    public void SetImage(Sprite newImage)
    {
        image = newImage;
    }

    public void RevertImage()
    {
        image = baseImage;
    }
}

public class Position : MonoBehaviour
{
    [SerializeField] private List<ImageAttribute> imageList;

    private Image image;

    
    private void Awake()
    {
        image = GetComponent<Image>();
    }


    public void SetTime(int time)
    {
        image.sprite = imageList[time].image;
    }
}
