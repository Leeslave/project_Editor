using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Position : MonoBehaviour
{
    [SerializeField] private List<Sprite> imageList;
    [SerializeField] private int dayImage;
    [SerializeField] private int nightImage;

    private Image image;

    
    private void Awake()
    {
        image = GetComponent<Image>();
    }


    public void SetDay()
    {
        image.sprite = imageList[nightImage];
    }


    public void SetNight()
    {
        image.sprite = imageList[dayImage];
    }
}
