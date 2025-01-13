using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CRTEffect : MonoBehaviour
{
    public Shader Shader;

    [Header("Distortion Intensity")]
    [Range(0, 1)]
    public float Distortion;
    [Header("Line Intensity")]
    [Range(0, 1)]
    public float Line;

    private Material _material;

    private void Start()
    {
        _material = new Material(Shader);
        if (CRTEventSystem.System != null) CRTEventSystem.CRTMat = _material;
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        _material.SetFloat("_Distort",Distortion);
        _material.SetFloat("_Line", Line);
        Graphics.Blit(source, destination, _material);
    }
}
