using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class CustomUIMaskable : MonoBehaviour, IMaterialInstantiate
{
    #region 머터리얼 관련
    [SerializeField] private Image image;
    [SerializeField] private Image mask;
    [SerializeField] private Material baseMaterial;

    private static readonly int MaskTex = Shader.PropertyToID("_MaskTex");
    private static readonly int MaskRect = Shader.PropertyToID("_MaskRect");
    private static readonly int Color = Shader.PropertyToID("_Color");

    public void AssignInstanceMaterial()
    {
        if (baseMaterial == null)
        {
            Debug.LogWarning("베이스 머터리얼이 설정되어 있지 않음.");
            return;
        }

        var component = transform.GetComponent<Image>();
        if (component == null)
        {
            Debug.LogWarning("머터리얼 인스턴스를 할당할 이미지 컴포넌트 없음.");
            return;
        }

        var objName = gameObject.name;
        var objId = gameObject.GetInstanceID();
        if (component.material.name.Contains($"{baseMaterial.name} [Instance:{objName}_{objId}]"))
        {
            Debug.LogWarning("이미 베이스 머터리얼의 인스턴스가 할당되어 있음.");
            return;
        }

        var inst = new Material(baseMaterial)
        {
            name = $"{baseMaterial.name} [Instance:{objName}_{objId}]"
        };
        component.material = inst;
        Debug.Log("이미지에 인스턴스 머터리얼 할당 완료.");
    }

    private Material ImageMatForRendering { get; set; }

    #endregion

    private void Start()
    {
        //머터리얼 인스턴스 할당
        AssignInstanceMaterial();

        //할당받은 머터리얼 접근
        ImageMatForRendering = image.materialForRendering;

        //마스크의 텍스처 전달
        if (ImageMatForRendering && mask)
            ImageMatForRendering.SetTexture(MaskTex, mask.mainTexture);
    }

    private void LateUpdate()
    {
        if (!image || !mask)
            return;
        
        //상대 위치 및 크기 전달
        ImageMatForRendering?.SetVector(MaskRect, GetMaskRect());
        ImageMatForRendering?.SetColor(Color, image.color);
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
