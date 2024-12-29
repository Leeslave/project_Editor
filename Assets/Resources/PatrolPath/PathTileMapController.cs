using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PathTileMapController : MonoBehaviour
{
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
    private Vector3Int _anchorGridPos;
    
    void Start()
    {
        isDrag = false;
        
        _anchorGridPos = pathTileMap.WorldToCell(startPoint.transform.position);
        startPoint.SetActive(false);
        pathTileMap.SetTile(_anchorGridPos, pathTile[0]);
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
            
            //현 위치에 대한 가이드 타일 획득
            TileBase currentGuideTile = guideTileMap.GetTile(currentGridPos);
            if (currentGuideTile)
                return;

            //현 위치에 대한 타 경로 타일 획득
            foreach (Tilemap other in otherPathTileMap)
            {
                TileBase currentOtherPathTile = other.GetTile(currentGridPos);
                if (currentOtherPathTile)
                    return;
            }
            
            //현 위치에 대한 경로 타일 획득
            TileBase currentPathTile = pathTileMap.GetTile(currentGridPos);
            if (!currentPathTile)
            {
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
                MatchPathTile(_anchorGridPos);
                    
                //앵커 타일 위치 업데이트
                _anchorGridPosStack.Push(_anchorGridPos);
                _anchorGridPos = currentGridPos;
            }
            else if (currentPathTile && pathTile.Contains(currentPathTile) && _anchorGridPosStack.Peek() == currentGridPos)
            {
                //이전 타일을 삭제
                pathTileMap.SetTile(_anchorGridPos, null);
                        
                //현재 타일을 룰에 따라 매치
                MatchPathTile(currentGridPos);
                        
                //앵커 타일 위치 업데이트
                _anchorGridPosStack.Pop();
                _anchorGridPos = currentGridPos;
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isDrag = false;
        }
    }

    private void MatchPathTile(Vector3Int pos)
    {
        bool down = pathTile.Contains(pathTileMap.GetTile(pos + Vector3Int.down));
        bool left = pathTile.Contains(pathTileMap.GetTile(pos + Vector3Int.left));
        bool up = pathTile.Contains(pathTileMap.GetTile(pos + Vector3Int.up));
        bool right = pathTile.Contains(pathTileMap.GetTile(pos + Vector3Int.right));

        switch (down)
        {
            case false when !left && !up && !right:
                pathTileMap.SetTile(pos, pathTile[0]);
                break;
            case false when !left && up && !right:
                pathTileMap.SetTile(pos, pathTile[1]);
                break;
            case false when !left && !up:
                pathTileMap.SetTile(pos, pathTile[2]);
                break;
            case true when !left && !up && !right:
                pathTileMap.SetTile(pos, pathTile[3]);
                break;
            case false when left && !up && !right:
                pathTileMap.SetTile(pos, pathTile[4]);
                break;
            case false when !left:
                pathTileMap.SetTile(pos, pathTile[5]);
                break;
            case true when !left && !up:
                pathTileMap.SetTile(pos, pathTile[6]);
                break;
            case true when left && !up && !right:
                pathTileMap.SetTile(pos, pathTile[7]);
                break;
            case false when up && !right:
                pathTileMap.SetTile(pos, pathTile[8]);
                break;
            case true when !left && !right:
                pathTileMap.SetTile(pos, pathTile[9]);
                break;
            case false when !up:
                pathTileMap.SetTile(pos, pathTile[10]);
                break;
        }

        // {
        //     if(!down && !left && !up && !right)
        //         pathTileMap.SetTile(pos, pathTile[0]);
        //     else if(!down && !left && up && !right)
        //         pathTileMap.SetTile(pos, pathTile[1]);
        //     else if(!down && !left && !up && right)
        //         pathTileMap.SetTile(pos, pathTile[2]);
        //     else if(down && !left && !up && !right)
        //         pathTileMap.SetTile(pos, pathTile[3]);
        //     else if(!down && left && !up && !right)
        //         pathTileMap.SetTile(pos, pathTile[4]);
        //     else if(!down && !left && up && right)
        //         pathTileMap.SetTile(pos, pathTile[5]);
        //     else if(down && !left && !up && right)
        //         pathTileMap.SetTile(pos, pathTile[6]);
        //     else if(down && left && !up && !right)
        //         pathTileMap.SetTile(pos, pathTile[7]);
        //     else if(!down && left && up && !right)
        //         pathTileMap.SetTile(pos, pathTile[8]);
        //     else if (down && !left && up && !right)
        //         pathTileMap.SetTile(pos, pathTile[9]);
        //     else if (!down && left && !up && right)
        //         pathTileMap.SetTile(pos, pathTile[10]);
        // }
    }
    private Vector3Int GetMouseGridPosition() => pathTileMap.WorldToCell(mainCamera.ScreenToWorldPoint(Input.mousePosition));
}
