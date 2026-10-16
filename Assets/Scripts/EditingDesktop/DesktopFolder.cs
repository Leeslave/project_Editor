using System;
using System.Collections.Generic;
using UnityEngine;

public class DesktopFolder : MonoBehaviour
{
    [Header("- Window 크기")]
    [SerializeField] protected int _width;
    [SerializeField] protected int _height;
    [Header("- Icon 크기")]
    [SerializeField] protected int _iconWidth;
    [SerializeField] protected int _iconHeight;
    [Header("- 간격")]
    [SerializeField] protected int _horizontalSpacing;
    [SerializeField] protected int _verticalSpacing;

    protected int _startX;
    protected int _startY;
    protected int _columnCount;
    protected int _rowCount;

    protected bool _canMove = true;

    protected RectTransform _rectTransform;

    protected bool[,] _occupied;
    [Header("따로 사전에 생성된 Icon들\n따로 안 넣어두면 버그 납니다.")]
    [SerializeField] private List<GameObject> _existingIcons;
    [Header("(주의) IconSample을 무조건 Field에 생성한 후\n기본적인 Prefab을 적용한 후 넣어주세요.")]

    [SerializeField] private GameObject _iconPrefab;
    [Header("풀링 수(최대 30)")]
    private int _poolSize;
    protected GameObject[] _icons;
    protected DesktopIcon[] _iconComponents;
    protected bool[] _availableIcons;

    [SerializeField] private bool _hideOnStart = false;
    // 아이콘 풀과 격자 배치 정보를 초기화한다.
    private void Awake()
    {
        DesktopFolder window = GetComponent<DesktopFolder>();
        UpdateGridSize();
        _icons = new GameObject[_poolSize];
        _iconComponents = new DesktopIcon[_poolSize];
        _availableIcons = new bool[_poolSize];
        for (int i = 0; i < _existingIcons.Count; i++) _icons[i] = _existingIcons[i];
        for (int i = _existingIcons.Count; i < _poolSize; i++) _icons[i] = Instantiate(_iconPrefab, transform);
        for (int i = 0; i < _poolSize; i++)
        {
            _iconComponents[i] = _icons[i].GetComponent<DesktopIcon>();
            _iconComponents[i].PoolIndex = i;
            _iconComponents[i].Folder = window;
        }
        for (int i = 0; i < _poolSize; i++) _availableIcons[i] = true;
        for (int i = 0; i < _existingIcons.Count; i++) _availableIcons[i] = false;
        _existingIcons.Clear();
        if(_hideOnStart) gameObject.SetActive(false);
    }

    /// <summary>
    /// Window 자체의 Size 변경.
    /// </summary>
    protected virtual void UpdateGridSize()
    {
        _rectTransform = GetComponent<RectTransform>();
        _rectTransform.sizeDelta = new Vector2(_width, _height);
        // Icon 배치 공간 계산
        _columnCount = (_width + _horizontalSpacing) / (_horizontalSpacing + _iconWidth);
        _rowCount = (_height + _verticalSpacing) / (_verticalSpacing + _iconHeight);
        _occupied = new bool[_rowCount, _columnCount];
        _poolSize = _rowCount * _columnCount;
        _startX = (int)(-(_iconWidth * _columnCount + _horizontalSpacing*(_columnCount-1)) * 0.5);
        _startY = (int)((_iconHeight * _rowCount + _verticalSpacing * (_rowCount- 1))* 0.5);
        for (int i = 0; i < _rowCount; i++) for (int z = 0; z < _columnCount; z++) _occupied[i, z] = false;
    }

    /// <summary>
    /// 드래그에 의한 ICon 이동.
    /// 이미 해당 위치에 배치된 Icon이 있으면 이동하지 못함.
    /// </summary>
    /// <param name="Call"> 이동을 요청한 Icon </param>
    /// <param name="Dragged"> 드래그로 따로 움직이는 Cnt </param>
    /// <param name="CallLay"> Icon이 자체적으로 가지고 있는 Occupied의 주소 </param>
    public virtual void MoveIcon(GameObject call,Transform dragged,ref Tuple<int,int>callLay)
    {
        if (!_canMove) return;
        RectTransform cnt = call.GetComponent<RectTransform>();
        int x = (int)((dragged.position.x - _startX - _rectTransform.anchoredPosition.x) / (_iconWidth + _horizontalSpacing));
        int y = (int)((_startY - dragged.position.y + _rectTransform.anchoredPosition.y) / (_iconHeight + _verticalSpacing));
        if (x >= _columnCount || x < 0 || y >= _rowCount || y < 0) return;
        if (_occupied[y, x] == true) return;
        _occupied[callLay.Item2,callLay.Item1] = false;
        _occupied[y, x] = true;
        callLay = new Tuple<int, int>(x,y);
        cnt.anchoredPosition = new Vector3(
                         _startX + x * (_iconWidth + _horizontalSpacing) + cnt.pivot.x * _iconWidth,
                         _startY - y * (_iconHeight + _verticalSpacing) - cnt.pivot.y * _iconHeight, 0);
    }

    /// <summary>
    /// Icon 생성이 될 때 Window에 배치해줌.
    /// </summary>
    /// <param name="a"> 생성되는 Icon </param>
    /// <returns></returns>
    public virtual Tuple<int,int> PlaceIcon(GameObject a)
    {
        for(int x = 0; x < _columnCount; x++) for(int y = 0; y < _rowCount; y++)
                if (_occupied[y,x] == false)
                {
                    _occupied[y, x] = true;
                    RectTransform cnt = a.GetComponent<RectTransform>();
                    cnt.anchoredPosition = new Vector3(
                         _startX + x * (_iconWidth + _horizontalSpacing) + cnt.pivot.x * _iconWidth,
                         _startY - y * (_iconHeight + _verticalSpacing) - cnt.pivot.y * _iconHeight, 0);
                    cnt.sizeDelta = new Vector2(_iconWidth, _iconHeight);
                    return new Tuple<int,int>(x, y);
                }
        return null;
    }

    /// <summary>
    /// Icon 생성. 기본적으로 사용되는 UIIcons이외의 다른 것을 사용 시 수정 필요.
    /// </summary>
    /// <param name="AttatchAble"> 메세지에 첨부 가능 여부 </param>
    /// <param name="OpenProcess"> 클릭 시 실행되는 프로세스 </param>
    /// <param name="name"> 이름 </param>
    /// <param name="Image"> 이미지 </param>
    public virtual GameObject CreateIcon(string name, Sprite image,int type = -1, bool canAttach = false, GameObject openProcess = null)
    {
        for(int i = 0; i < _poolSize; i++)
        {
            if (_availableIcons[i])
            {
                _availableIcons[i] = false;
                _icons[i].SetActive(true);
                _iconComponents[i].Init(canAttach,openProcess,name,image,i,type);
                return _icons[i];
            }
        }
        return null;
    }
    /// <summary>
    /// 특정 작업을 통해 LayOut이 변경된 Icon의 기존 LayOut을 이용 가능하게 변경
    /// </summary>
    /// <param name="CallLay"> Icon의 현재 LayOut상에서의 위치 </param>
    public virtual void RemoveIcon(Tuple<int,int> callLay)
    {
        _occupied[callLay.Item2,callLay.Item1] = false;
    }
    /// <summary>
    /// 해당 PoolNum의 Icon을 Pooling 가능하게 변경.
    /// </summary>
    /// <param name="ind"></param>
    public virtual void ClearIcon(int ind)
    {
        _availableIcons[ind] = true;
    }
}
