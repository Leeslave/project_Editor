using System.Collections.Generic;
using System.Linq;
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
        //블로커 오브젝트의 형태를 나타내는 스프라이트 렌더러
        _guideSprites = transform.GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer guideSprite in _guideSprites)
        {
            guideSprite.color = Color.clear;
            Vector2Int temp = new((int)guideSprite.transform.localPosition.x, (int)guideSprite.transform.localPosition.y); 
            blocks.Add(temp);
        }

        //블로커 오브젝트의 현재 타일 위치 저장
        Vector3Int currentGridPos = PatrolPathManager.Instance.blockingTileMap.WorldToCell(transform.position);
        foreach(Vector3Int block in blocks)
            PatrolPathManager.Instance.blockingTileMap.SetTile(currentGridPos + block, blockTile);
        anchorGridPos = currentGridPos;
    }
    
    public void OnMouseDownEvent()
    {
        //적용 중이던 블로커 타일 제거 
        foreach(Vector3Int block in blocks)
            PatrolPathManager.Instance.blockingTileMap.SetTile(anchorGridPos + block, null);
    }
    public void OnMouseDragEvent()
    {
        //마우스 드래그 위치로 블로커 오브젝트를 이동
        Vector3 selectedWorldPos = PatrolPathManager.Instance.mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int selectedGridPos = PatrolPathManager.Instance.blockingTileMap.WorldToCell(selectedWorldPos);
        transform.position = new Vector3(selectedGridPos.x + 0.5f, selectedGridPos.y + 0.5f, 0f);

        //가이드 타일, 타 블로커 타일, 경로 타일과의 중첩 여부 확인
        bool condition1 = blocks.Any(block =>
            PatrolPathManager.Instance.guideTileMap.GetTile(selectedGridPos + (Vector3Int)block));
        bool condition2 = blocks.Any(block =>
            PatrolPathManager.Instance.blockingTileMap.GetTile(selectedGridPos + (Vector3Int)block));
        bool condition3 = blocks.Any(block =>
            PatrolPathManager.Instance.pathTileMap.Any(map => map.GetTile(selectedGridPos + (Vector3Int)block)));

        //블로커를 현 위치에 설치 가능한지 여부를 표시
        if (condition1 || condition2 || condition3)
            foreach (SpriteRenderer guideSprite in _guideSprites)
                guideSprite.color = new Color(1f, 0f, 0f, 0.25f);
        else
            foreach (SpriteRenderer guideSprite in _guideSprites)
                guideSprite.color = new Color(1f, 1f, 1f, 0.25f);
    }
    public void OnMouseUpEvent()
    {
        //마우스 업 위치
        Vector3 selectedWorldPos = PatrolPathManager.Instance.mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int selectedGridPos = PatrolPathManager.Instance.blockingTileMap.WorldToCell(selectedWorldPos);
        
        //블로커 오브젝트 투명화
        foreach (SpriteRenderer guideSprite in _guideSprites)
            guideSprite.color = Color.clear;

        //가이드 타일, 타 블로커 타일, 경로 타일과의 중첩 여부 확인
        bool condition1 = blocks.Any(block =>
            PatrolPathManager.Instance.guideTileMap.GetTile(selectedGridPos + (Vector3Int)block));
        bool condition2 = blocks.Any(block =>
            PatrolPathManager.Instance.blockingTileMap.GetTile(selectedGridPos + (Vector3Int)block));
        bool condition3 = blocks.Any(block =>
            PatrolPathManager.Instance.pathTileMap.Any(map => map.GetTile(selectedGridPos + (Vector3Int)block)));

        //불가능하다면 블로커 타일을 원위치에 재설치 
        if (condition1 || condition2 || condition3)
        {
            Vector3Int originalGridPos = PatrolPathManager.Instance.blockingTileMap.WorldToCell(anchorGridPos);
            transform.position = new Vector3(originalGridPos.x + 0.5f, originalGridPos.y + 0.5f, 0f);
            foreach(Vector3Int block in blocks)
                PatrolPathManager.Instance.blockingTileMap.SetTile(anchorGridPos + block, blockTile);
            return;
        }
        
        //가능하다면 블로커 타일을 해당 위치에 설치
        foreach(Vector3Int block in blocks)
            PatrolPathManager.Instance.blockingTileMap.SetTile(selectedGridPos + block, blockTile);
        anchorGridPos = selectedGridPos;
    }
}
