using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CRTEffect : MonoBehaviour
{
    public Shader Shader;

    [Header("Distortion")]
    [Range(0, 1)]
    public float Distortion;

    [Header("Line")]
    [Range(0, 1)]
    public float Alpha;
    [Range(0,1)]
    public float Thick;
    [Range(0.2f, 5)]
    public float Speed;

    [Header("Flip")]
    [Range(0, 1)]
    public float FlipRatio;
    [Range(0, 1)]
    public float FlipProb;
    [Range(0, 1)]
    public float FlipKeep;

    private Material _material;

    private void Start()
    {
        _material = new Material(Shader);
        if (CRTEventSystem.System != null) CRTEventSystem.CRTMat = _material;
        StartCoroutine(RandFlip());
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        _material.SetFloat("_Distort",Distortion);
        _material.SetFloat("_Line", Alpha);
        _material.SetFloat("_Thick", Thick);
        _material.SetFloat("_Speed", Speed);
        _material.SetFloat("_FlipProb", FlipProb);
        Graphics.Blit(source, destination, _material);
    }

    IEnumerator RandFlip()
    {
        while (true) 
        {
            _material.SetFloat("_Rand", Random.Range(0, 1f));
            yield return new WaitForSeconds(FlipKeep);
            _material.SetFloat("_Rand", 0);
            yield return new WaitForSeconds(FlipRatio + 0.05f);
        }
    }
}
