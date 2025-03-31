using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
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

    [Header("차단 타일 맵")] 
    public Tilemap blockingTileMap;

    [Header("채우기 타일 맵")] 
    public Tilemap fillTileMap;
    
    [Header("경로 타일 맵")] 
    public Tilemap[] pathTileMap;

    [Header("체크포인트 타일 맵")] 
    public Tilemap checkPointTileMap;
    
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
    private void IsStageClear()
    {
        //채우기 위치 획득
        List<Vector3Int> answerLocation = new();
        foreach (Vector3Int pos in fillTileMap.cellBounds.allPositionsWithin)
            if(fillTileMap.HasTile(pos))
                answerLocation.Add(pos);
        
        //경로 타일과 차단 타일로 꽉 차있는지 확인
        bool c1 = answerLocation.All(p => pathTileMap.Any(m => m.HasTile(p)) || blockingTileMap.HasTile(p));
        
        //체크포인트 위치 획득
        List<Vector3Int> checkLocation = new();
        foreach(Vector3Int pos in checkPointTileMap.cellBounds.allPositionsWithin)
            if(checkPointTileMap.HasTile(pos))
                checkLocation.Add(pos);
        
        //경로 타일이 체크포인트를 지나가는지 확인
        bool c2 = checkLocation.All(p => pathTileMap.Any(m => m.HasTile(p)));
        
        Debug.Log($"{c1} {c2}");
        
        //클리어 여부 표시
        fillTileMap.color = c1 && c2 ? new Color(0f, 0.5f, 0f, 0.2f) : new Color(0f, 0.5f, 0.5f, 0f);
    }
    
    void Start()
    {
        fillTileMap.color = new Color(0f, 0.5f, 0f, 0f);
        InvokeRepeating(nameof(IsStageClear), 0f, 0.1f);        
    }
}
