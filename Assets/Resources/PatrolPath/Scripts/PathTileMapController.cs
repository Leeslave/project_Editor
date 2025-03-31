using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum PathTileMapColor
{
    Magenta,
    Cyan,
    Yellow
}

public class PathTileMapController : MonoBehaviour
{
    public PathTileMapColor pathTileMapColor;
    
    private Tilemap _pathTileMap;
    private GameObject _startPoint;
    private bool _isDrag;
    
    [Header("경로 타일")]
    [SerializeField] private Tile[] pathTile;

    [Header("진동 애니메이션 커브")]
    [SerializeField] private AnimationCurve shake;

    [Header("드래그 아이콘")] 
    private SpriteRenderer _dragIcon;
    [SerializeField] private Sprite possible;
    [SerializeField] private Sprite impossible;
    
    /// <summary> 지나온 경로 타일의 격자 위치를 저장하는 스택 </summary>
    private readonly Stack<Vector3Int> _anchorGridPosStack = new();
    
    /// <summary> 현재 경로 타일의 길이 </summary>
    private int PathTileLength => _anchorGridPosStack.Count - 1;
    
    /// <summary> 현재 경로의 마지막 격자 위치 </summary>
    private Vector3Int _anchorGridPos;
    
    void Start()
    {
        _isDrag = false;
        
        //컴포넌트 획득
        _pathTileMap = transform.GetComponent<Tilemap>();
        _startPoint = transform.GetChild(0).gameObject;
        _dragIcon = transform.GetChild(1).GetComponent<SpriteRenderer>();
        
        //시작점을 첫번째 앵커로 삼는다        
        _anchorGridPos = _pathTileMap.WorldToCell(_startPoint.transform.position);
        _startPoint.SetActive(false);
        _anchorGridPosStack.Push(_anchorGridPos);
        _pathTileMap.SetTile(_anchorGridPos, pathTile[0]);
        _dragIcon.gameObject.SetActive(true);
        _dragIcon.transform.position = _pathTileMap.CellToWorld(_anchorGridPos) + new Vector3(0.75f, 0.75f);
        
        //업데이트
        PatrolPathManager.Instance.UpdateLimit(PathTileLength, pathTileMapColor);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !_isDrag)
        {
            Vector3Int clicked = GetMouseGridPosition();
            if (clicked == _anchorGridPos)
            {
                _dragIcon.color = Color.white;
                _isDrag = true;
            }
        }
        else if (Input.GetMouseButton(0) && _isDrag)
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
            if (PatrolPathManager.Instance.guideTileMap.GetTile(currentGridPos))
                return;
            
            //현 위치에 대한 블로킹 타일 확인
            if (PatrolPathManager.Instance.blockingTileMap.GetTile(currentGridPos))
                return;

            //현 위치에 대한 타 경로 타일 획득
            if (PatrolPathManager.Instance.pathTileMap.Any(other => other != _pathTileMap && other.GetTile(currentGridPos)))
                return;
            
            //현 위치에 대한 경로 타일 획득
            TileBase currentPathTile = _pathTileMap.GetTile(currentGridPos);
            if (!currentPathTile)
                AddPathTile(currentGridPos, differ);
            else if (currentPathTile && pathTile.Contains(currentPathTile) && _anchorGridPosStack.Peek() == currentGridPos)
                DeletePathTile(currentGridPos);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            _dragIcon.color = Color.clear;
            _isDrag = false;
        }
    }
    
    //새로운 타일 추가
    private void AddPathTile(Vector3Int currentGridPos, Vector3Int differ)
    {
        //최대 길이 확인
        if (PatrolPathManager.Instance.IsOverLimit(PathTileLength, pathTileMapColor))
            return;

        //아이콘 이동
        _dragIcon.transform.position = _pathTileMap.CellToWorld(currentGridPos) + new Vector3(0.75f, 0.75f);
        LJWConverter.Instance.ConvertSpriteRendererSize(false, 0f, 0.5f, new Vector2(0.5f, 0.5f), _dragIcon, shake);
        
        //새로운 타일 추가
        if(differ == Vector3Int.down)
            _pathTileMap.SetTile(currentGridPos, pathTile[1]);
        else if(differ == Vector3Int.left)
            _pathTileMap.SetTile(currentGridPos, pathTile[2]);
        else if(differ == Vector3Int.up)
            _pathTileMap.SetTile(currentGridPos, pathTile[3]);
        else if (differ == Vector3Int.right)
            _pathTileMap.SetTile(currentGridPos, pathTile[4]);
                    
        //이전 타일을 룰에 따라 매치
        MatchTrailPathTile(currentGridPos, _anchorGridPos, _anchorGridPosStack.Peek());
                
        //앵커 타일 위치 업데이트
        _anchorGridPosStack.Push(_anchorGridPos);
        _anchorGridPos = currentGridPos;
        
        //업데이트
        PatrolPathManager.Instance.UpdateLimit(PathTileLength, pathTileMapColor);

        if (PatrolPathManager.Instance.IsOverLimit(PathTileLength, pathTileMapColor))
        {
            _dragIcon.sprite = impossible;
            LJWConverter.Instance.ConvertSpriteRendererSize(false, 0f, 0.5f, new Vector2(0.7f, 0.7f), _dragIcon, shake);
        }
    }

    //기존 타일 삭제
    private void DeletePathTile(Vector3Int currentGridPos)
    {
        //아이콘 이동
        _dragIcon.transform.position = _pathTileMap.CellToWorld(currentGridPos) + new Vector3(0.75f, 0.75f);
        _dragIcon.sprite = possible;
        LJWConverter.Instance.ConvertSpriteRendererSize(false, 0f, 0.5f, new Vector2(0.5f, 0.5f), _dragIcon, shake);
        
        //이전 타일을 삭제
        _pathTileMap.SetTile(_anchorGridPos, null);
        _anchorGridPosStack.Pop();
                
        //앵커 타일 위치 업데이트
        _anchorGridPos = currentGridPos;
                
        //현재 타일을 룰에 따라 매치
        MatchAnchorPathTile(_anchorGridPos, _anchorGridPosStack.Peek());
        
        //업데이트
        PatrolPathManager.Instance.UpdateLimit(PathTileLength, pathTileMapColor);
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
            _pathTileMap.SetTile(change, pathTile[5]);            
        else if(c03 || c04)
            _pathTileMap.SetTile(change, pathTile[6]);
        else if(c05 || c06)
            _pathTileMap.SetTile(change, pathTile[7]);
        else if(c07 || c08)
            _pathTileMap.SetTile(change, pathTile[8]);
        else if(c09 || c10)
            _pathTileMap.SetTile(change, pathTile[9]);
        else if(c11 || c12)
            _pathTileMap.SetTile(change, pathTile[10]);

        bool c13 = forward == Vector3Int.up && behind == Vector3Int.zero;
        bool c14 = forward == Vector3Int.right && behind == Vector3Int.zero;
        bool c15 = forward == Vector3Int.down && behind == Vector3Int.zero;
        bool c16 = forward == Vector3Int.left && behind == Vector3Int.zero;
        
        if(c13)
            _pathTileMap.SetTile(change, pathTile[1]);
        else if(c14)
            _pathTileMap.SetTile(change, pathTile[2]);
        else if(c15)
            _pathTileMap.SetTile(change, pathTile[3]);
        else if(c16)
            _pathTileMap.SetTile(change, pathTile[4]);
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
            _pathTileMap.SetTile(change, pathTile[0]);
        else if(c1)
            _pathTileMap.SetTile(change, pathTile[1]);
        else if(c2)
            _pathTileMap.SetTile(change, pathTile[2]);
        else if(c3)
            _pathTileMap.SetTile(change, pathTile[3]);
        else if(c4)
            _pathTileMap.SetTile(change, pathTile[4]);
        
    }
    
    //타일맵 그리드에서 마우스의 위치를 반환
    private Vector3Int GetMouseGridPosition() => _pathTileMap.WorldToCell(PatrolPathManager.Instance.mainCamera.ScreenToWorldPoint(Input.mousePosition));
}
