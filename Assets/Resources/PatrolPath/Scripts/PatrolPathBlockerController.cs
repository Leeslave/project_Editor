using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PatrolPathBlockerController : MonoBehaviour
{
    [SerializeField] private Vector3Int anchorGridPos;
    
    [SerializeField] private List<Vector2Int> blocks;

    [SerializeField] private TileBase blockTile;

    private SpriteRenderer[] _guideSprites;

    private void Start()
    {
        _guideSprites = transform.GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer guideSprite in _guideSprites)
        {
            guideSprite.color = Color.clear;
            Vector2Int temp = new((int)guideSprite.transform.localPosition.x, (int)guideSprite.transform.localPosition.y); 
            blocks.Add(temp);
        }

        Vector3Int currentGridPos = PatrolPathManager.Instance.blockingTileMap.WorldToCell(transform.position);
        foreach(Vector3Int block in blocks)
            PatrolPathManager.Instance.blockingTileMap.SetTile(currentGridPos + block, blockTile);
        anchorGridPos = currentGridPos;
    }

    public void OnMouseDownEvent()
    {
        foreach(Vector3Int block in blocks)
            PatrolPathManager.Instance.blockingTileMap.SetTile(anchorGridPos + block, null);
    }
    public void OnMouseDragEvent()
    {
        Vector3 selectedWorldPos = PatrolPathManager.Instance.mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int selectedGridPos = PatrolPathManager.Instance.blockingTileMap.WorldToCell(selectedWorldPos);
        transform.position = new Vector3(selectedGridPos.x + 0.5f, selectedGridPos.y + 0.5f, 0f);
        
        bool condition1 = blocks.Any(block =>
            PatrolPathManager.Instance.guideTileMap.GetTile(selectedGridPos + (Vector3Int)block));
        bool condition2 = blocks.Any(block =>
            PatrolPathManager.Instance.blockingTileMap.GetTile(selectedGridPos + (Vector3Int)block));
        bool condition3 = blocks.Any(block =>
            PatrolPathManager.Instance.pathTileMap.Any(map => map.GetTile(selectedGridPos + (Vector3Int)block)));

        if (condition1 || condition2 || condition3)
            foreach (SpriteRenderer guideSprite in _guideSprites)
                guideSprite.color = new Color(1f, 0f, 0f, 0.25f);
        else
            foreach (SpriteRenderer guideSprite in _guideSprites)
                guideSprite.color = new Color(1f, 1f, 1f, 0.25f);
    }
    public void OnMouseUpEvent()
    {
        Vector3 selectedWorldPos = PatrolPathManager.Instance.mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int selectedGridPos = PatrolPathManager.Instance.blockingTileMap.WorldToCell(selectedWorldPos);
        
        foreach (SpriteRenderer guideSprite in _guideSprites)
            guideSprite.color = Color.clear;

        bool condition1 = blocks.Any(block =>
            PatrolPathManager.Instance.guideTileMap.GetTile(selectedGridPos + (Vector3Int)block));
        bool condition2 = blocks.Any(block =>
            PatrolPathManager.Instance.blockingTileMap.GetTile(selectedGridPos + (Vector3Int)block));
        bool condition3 = blocks.Any(block =>
            PatrolPathManager.Instance.pathTileMap.Any(map => map.GetTile(selectedGridPos + (Vector3Int)block)));

        if (condition1 || condition2 || condition3)
        {
            Vector3Int originalGridPos = PatrolPathManager.Instance.blockingTileMap.WorldToCell(anchorGridPos);
            transform.position = new Vector3(originalGridPos.x + 0.5f, originalGridPos.y + 0.5f, 0f);
            foreach(Vector3Int block in blocks)
                PatrolPathManager.Instance.blockingTileMap.SetTile(anchorGridPos + block, blockTile);
            return;
        }
        
        foreach(Vector3Int block in blocks)
            PatrolPathManager.Instance.blockingTileMap.SetTile(selectedGridPos + block, blockTile);
        anchorGridPos = selectedGridPos;
    }
}
