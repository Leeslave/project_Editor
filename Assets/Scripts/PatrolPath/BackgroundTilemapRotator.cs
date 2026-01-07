using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundTilemapRotator : MonoBehaviour
{
    [Header("회전 속도")] 
    public Vector3 speed;
    
    void Start() => InvokeRepeating(nameof(RotateTileMap), 0f, 0.02f);

    void RotateTileMap()
    {
        transform.Rotate(speed * 0.02f);
    }
}
