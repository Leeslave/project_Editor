using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.SocialPlatforms;
using UnityEngine.Tilemaps;

public class PatrolPathManager : MonoBehaviour
{
    public static PatrolPathManager Instance;
    
    [SerializeField] private int cyanLengthLimit;
    [SerializeField] private int magentaLengthLimit;
    [SerializeField] private int yellowLengthLimit;
    [SerializeField] private TMP_Text cyanLimitText;
    [SerializeField] private TMP_Text magentaLimitText;
    [SerializeField] private TMP_Text yellowLimitText;
    
    [Header("메인 카메라")]
    public Camera mainCamera;
    
    [Header("가이드 타일 맵")]
    public Tilemap guideTileMap;

    [Header("블로킹 타일 맵")] 
    public Tilemap blockingTileMap;

    [Header("해답 타일 맵")] 
    public Tilemap answerTileMap;
    
    [Header("경로 타일 맵")] 
    public Tilemap[] pathTileMap;
    
    private void Awake()
    {
        if (!Instance)
            Instance = this;
    }

    public bool IsOverLimit(int length, PathTileMapColor color)
    {
        switch (color)
        {
            case PathTileMapColor.Magenta:
                return length >= magentaLengthLimit;
            case PathTileMapColor.Cyan:
                return length >= cyanLengthLimit;
            case PathTileMapColor.Yellow:
                return length >= yellowLengthLimit;
            default:
                throw new ArgumentOutOfRangeException(nameof(color), color, null);
        }
    }
    public void UpdateLimit(int length, PathTileMapColor color)
    {
        switch (color)
        {
            case PathTileMapColor.Cyan:
                {
                    int diff = cyanLengthLimit - length;
                    string text = (diff == 0) ? $"<color=#FF0000>{diff:D2}" : $"{diff:D2}";
                    cyanLimitText.text = $"<color=#00FFFF>C:</color> {text}/{cyanLengthLimit:D2}\n";
                    break;   
                }
            case PathTileMapColor.Magenta:
                {
                    int diff = magentaLengthLimit - length;
                    string text = (diff == 0) ? $"<color=#FF0000>{diff:D2}" : $"{diff:D2}";
                    magentaLimitText.text = $"<color=#FF00FF>M:</color> {text}/{magentaLengthLimit:D2}\n";
                    break;   
                }
            case PathTileMapColor.Yellow:
                {
                    int diff = yellowLengthLimit - length;
                    string text = (diff == 0) ? $"<color=#FF0000>{diff:D2}" : $"{diff:D2}";
                    yellowLimitText.text = $"<color=#FFFF00>Y:</color> {text}/{yellowLengthLimit:D2}\n";
                    break;   
                }
            default:
                throw new ArgumentOutOfRangeException(nameof(color), color, null);            
        }
    }
    private void IsMapFullFilled()
    {
        List<Vector3Int> location = new();
        foreach (Vector3Int pos in answerTileMap.cellBounds.allPositionsWithin)
            if(answerTileMap.HasTile(pos))
                location.Add(pos);
        if(location.All(p => pathTileMap.Any(m => m.HasTile(p)) || blockingTileMap.HasTile(p)))
            answerTileMap.color = new Color(0f, 0.5f, 0f, 0.2f);
        else
            answerTileMap.color = answerTileMap.color = new Color(0f, 0.5f, 0.5f, 0f);
    }
    
    void Start()
    {
        answerTileMap.color = new Color(0f, 0.5f, 0f, 0f);
        InvokeRepeating(nameof(IsMapFullFilled), 0f, 0.1f);        
    }

    void Update()
    {

    }
}
