using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.WSA;

public class OutlineTilemapScroller : MonoBehaviour
{
    [Header("이동 속도")] 
    public float speed;
    
    void Start() => InvokeRepeating(nameof(ScrollTileMap), 0f, 0.02f);

    void ScrollTileMap()
    {
        Vector3 pos = transform.position;
        pos.x += speed * 0.02f;
        switch (pos.x)
        {
            case <= -0.5f:
                pos.x += 1f;
                break;
            case >= 0.5f:
                pos.x -= 1f;
                break;
        }
        transform.position = pos;
    }
}
