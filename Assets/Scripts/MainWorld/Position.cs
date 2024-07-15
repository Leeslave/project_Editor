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
    private int nightShiftTime = 2;

    
    private void Awake()
    {
        image = GetComponent<Image>();
        foreach (var iter in imageList)
        {
            iter.RevertImage();
        }
        SetTime(0);
    }

    private void OnEnable()
    {
        if (GameSystem.Instance.gameData.time < nightShiftTime)
        {
            SetTime(0);
        }
        else
        {
            SetTime(1);
        }
    }


    public void SetTime(int time)
    {
        if (time >= imageList.Count) time = 0;
        image.sprite = imageList[time].image;
    }
}
