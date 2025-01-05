using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

public enum PathTileMapColor
{
    Red,
    Blue,
    Yellow
}

public class PathTileMapController : MonoBehaviour
{
    public PathTileMapColor pathTileMapColor;
    
    [Header("가이드 타일 맵")]
    [SerializeField] private Tilemap guideTileMap;
    
    [Header("경로 타일 맵")]
    [SerializeField] private Tilemap pathTileMap;

    [Header("시작점과 완주점")] 
    [SerializeField] private GameObject startPoint;
    
    [Header("타 경로 타일 맵")] 
    [SerializeField] private Tilemap[] otherPathTileMap;
    
    [Header("메인 카메라")]
    [SerializeField] private Camera mainCamera;

    [Header("드래그 여부")]
    [SerializeField] private bool isDrag;
    
    [Header("경로 타일")]
    [SerializeField] private Tile[] pathTile;
    
    private readonly Stack<Vector3Int> _anchorGridPosStack = new();
    private int pathTileLength => _anchorGridPosStack.Count - 1;
    
    
    private Vector3Int _anchorGridPos;
    
    void Start()
    {
        isDrag = false;
        
        //시작점을 첫번째 앵커로 삼는다
        _anchorGridPos = pathTileMap.WorldToCell(startPoint.transform.position);
        startPoint.SetActive(false);
        _anchorGridPosStack.Push(_anchorGridPos);
        pathTileMap.SetTile(_anchorGridPos, pathTile[0]);
        
        //업데이트
        PatrolPathManager.Instance.UpdateLimit(pathTileLength, pathTileMapColor);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isDrag)
        {
            Vector3Int clicked = GetMouseGridPosition();
            if(clicked == _anchorGridPos)
                isDrag = true;
        }
        else if (Input.GetMouseButton(0) && isDrag)
        {
            //현재 좌표 계산
            Vector3Int currentGridPos = GetMouseGridPosition();
            
            //제자리에 있으므로 변화 없음
            if (_anchorGridPos == currentGridPos)
                return;

            //연속적인 드래그인지 확인
            Vector3Int differ = currentGridPos - _anchorGridPos;
            if (!Mathf.Approximately(differ.magnitude, 1))
                return;
            
            //현 위치에 대한 가이드 타일 확인
            if (guideTileMap.GetTile(currentGridPos))
                return;

            //현 위치에 대한 타 경로 타일 획득
            if (otherPathTileMap.Any(other => other.GetTile(currentGridPos)))
                return;
            
            //현 위치에 대한 경로 타일 획득
            TileBase currentPathTile = pathTileMap.GetTile(currentGridPos);
            if (!currentPathTile)
                AddPathTile(currentGridPos, differ);
            else if (currentPathTile && pathTile.Contains(currentPathTile) && _anchorGridPosStack.Peek() == currentGridPos)
                DeletePathTile(currentGridPos);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isDrag = false;
        }
    }
    
    //새로운 타일 추가
    private void AddPathTile(Vector3Int currentGridPos, Vector3Int differ)
    {
        //최대 길이 확인
        if (PatrolPathManager.Instance.IsOverLimit(pathTileLength, pathTileMapColor))
            return;
        
        //새로운 타일 추가
        if(differ == Vector3Int.down)
            pathTileMap.SetTile(currentGridPos, pathTile[1]);
        else if(differ == Vector3Int.left)
            pathTileMap.SetTile(currentGridPos, pathTile[2]);
        else if(differ == Vector3Int.up)
            pathTileMap.SetTile(currentGridPos, pathTile[3]);
        else if (differ == Vector3Int.right)
            pathTileMap.SetTile(currentGridPos, pathTile[4]);
                    
        //이전 타일을 룰에 따라 매치
        MatchTrailPathTile(currentGridPos, _anchorGridPos, _anchorGridPosStack.Peek());
                
        //앵커 타일 위치 업데이트
        _anchorGridPosStack.Push(_anchorGridPos);
        _anchorGridPos = currentGridPos;
        
        //업데이트
        PatrolPathManager.Instance.UpdateLimit(pathTileLength, pathTileMapColor);
    }

    //기존 타일 삭제
    private void DeletePathTile(Vector3Int currentGridPos)
    {
        //이전 타일을 삭제
        pathTileMap.SetTile(_anchorGridPos, null);
        _anchorGridPosStack.Pop();
                
        //앵커 타일 위치 업데이트
        _anchorGridPos = currentGridPos;
                
        //현재 타일을 룰에 따라 매치
        MatchAnchorPathTile(_anchorGridPos, _anchorGridPosStack.Peek());
        
        //업데이트
        PatrolPathManager.Instance.UpdateLimit(pathTileLength, pathTileMapColor);
    }
    
    //따라오는 타일을 룰에 따라서 변경
    private void MatchTrailPathTile(Vector3Int add, Vector3Int change, Vector3Int last)
    {
        Vector3Int forward = add - change;
        Vector3Int behind = last - change;

        bool c01 = forward == Vector3Int.up && behind == Vector3Int.right;
        bool c02 = forward == Vector3Int.right && behind == Vector3Int.up;
        
        bool c03 = forward == Vector3Int.down && behind == Vector3Int.right;
        bool c04 = forward == Vector3Int.right && behind == Vector3Int.down;
        
        bool c05 = forward == Vector3Int.left && behind == Vector3Int.down;
        bool c06 = forward == Vector3Int.down && behind == Vector3Int.left;
        
        bool c07 = forward == Vector3Int.up && behind == Vector3Int.left;
        bool c08 = forward == Vector3Int.left && behind == Vector3Int.up;
        
        bool c09 = forward == Vector3Int.down && behind == Vector3Int.up;
        bool c10 = forward == Vector3Int.up && behind == Vector3Int.down;
        
        bool c11 = forward == Vector3Int.left && behind == Vector3Int.right;
        bool c12 = forward == Vector3Int.right && behind == Vector3Int.left;
        
        if (c01 || c02)
            pathTileMap.SetTile(change, pathTile[5]);            
        else if(c03 || c04)
            pathTileMap.SetTile(change, pathTile[6]);
        else if(c05 || c06)
            pathTileMap.SetTile(change, pathTile[7]);
        else if(c07 || c08)
            pathTileMap.SetTile(change, pathTile[8]);
        else if(c09 || c10)
            pathTileMap.SetTile(change, pathTile[9]);
        else if(c11 || c12)
            pathTileMap.SetTile(change, pathTile[10]);

        bool c13 = forward == Vector3Int.up && behind == Vector3Int.zero;
        bool c14 = forward == Vector3Int.right && behind == Vector3Int.zero;
        bool c15 = forward == Vector3Int.down && behind == Vector3Int.zero;
        bool c16 = forward == Vector3Int.left && behind == Vector3Int.zero;
        
        if(c13)
            pathTileMap.SetTile(change, pathTile[1]);
        else if(c14)
            pathTileMap.SetTile(change, pathTile[2]);
        else if(c15)
            pathTileMap.SetTile(change, pathTile[3]);
        else if(c16)
            pathTileMap.SetTile(change, pathTile[4]);
    }
    
    //최전방의 타일을 룰에 따라서 변경
    private void MatchAnchorPathTile(Vector3Int change, Vector3Int last)
    {
        Vector3Int behind = last - change;

        bool c0 = behind == Vector3Int.zero;
        bool c1 = behind == Vector3Int.up;
        bool c2 = behind == Vector3Int.right;
        bool c3 = behind == Vector3Int.down;
        bool c4 = behind == Vector3Int.left;
        
        if(c0)
            pathTileMap.SetTile(change, pathTile[0]);
        else if(c1)
            pathTileMap.SetTile(change, pathTile[1]);
        else if(c2)
            pathTileMap.SetTile(change, pathTile[2]);
        else if(c3)
            pathTileMap.SetTile(change, pathTile[3]);
        else if(c4)
            pathTileMap.SetTile(change, pathTile[4]);
        
    }
    
    //타일맵 그리드에서 마우스의 위치를 반환
    private Vector3Int GetMouseGridPosition() => pathTileMap.WorldToCell(mainCamera.ScreenToWorldPoint(Input.mousePosition));
}
