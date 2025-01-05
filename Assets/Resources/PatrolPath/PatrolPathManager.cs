using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class PatrolPathManager : MonoBehaviour
{
    public static PatrolPathManager Instance;
    
    [SerializeField] private int redLengthLimit;
    [SerializeField] private int blueLengthLimit;
    [SerializeField] private int yellowLengthLimit;
    [SerializeField] private TMP_Text redLimitText;
    [SerializeField] private TMP_Text blueLimitText;
    [SerializeField] private TMP_Text yellowLimitText;
    
    private void Awake()
    {
        if (!Instance)
            Instance = this;
    }

    public bool IsOverLimit(int length, PathTileMapColor color)
    {
        switch (color)
        {
            case PathTileMapColor.Red:
                return length >= redLengthLimit;
            case PathTileMapColor.Blue:
                return length >= blueLengthLimit;
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
            case PathTileMapColor.Red:
                redLimitText.text = $"<color=#CC3333>R:</color> {length:D2}/{redLengthLimit:D2}\n";
                break;
            case PathTileMapColor.Blue:
                blueLimitText.text = $"<color=#3333CC>B:</color> {length:D2}/{blueLengthLimit:D2}\n";
                break;
            case PathTileMapColor.Yellow:
                yellowLimitText.text = $"<color=#CCCC33>Y:</color> {length:D2}/{yellowLengthLimit:D2}\n";
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(color), color, null);            
        }
    }
    
    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
