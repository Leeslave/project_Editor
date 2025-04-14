using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class MyMaskableController : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private Image mask;
    
    
    private static readonly int MaskTex = Shader.PropertyToID("_MaskTex");
    private static readonly int MaskRect = Shader.PropertyToID("_MaskRect");
    private static readonly int Color = Shader.PropertyToID("_Color");

    private Material ImageMatForRendering { get; set; }

    private void OnValidate()
    {
        //이미지의 머터리얼
        if(!ImageMatForRendering && image)
            ImageMatForRendering = image.materialForRendering;
        
        //마스크의 텍스처 전달
        if(ImageMatForRendering && mask)
            ImageMatForRendering.SetTexture(MaskTex, mask.mainTexture);
    }

    private void LateUpdate()
    {
        if (!image || !mask)
            return;
        
        //상대 위치 및 크기 전달
        ImageMatForRendering?.SetVector(MaskRect, GetMaskRect());
        ImageMatForRendering?.SetVector(Color, image.color);
    }
    
    private Vector4 GetMaskRect()
    {
        //상대 위치
        Vector2 posDelta = mask.rectTransform.anchoredPosition - image.rectTransform.anchoredPosition;
        
        //이미지 크기
        Vector2 imageSize = image.rectTransform.rect.size;
        
        //마스크 크기
        Vector2 maskSize = mask.rectTransform.rect.size;
        
        float x = posDelta.x / maskSize.x;
        float y = posDelta.y / maskSize.y;
        float z = maskSize.x / imageSize.x;
        float w = maskSize.y / imageSize.y;
        return new Vector4(x, y, z, w);
    }
}
