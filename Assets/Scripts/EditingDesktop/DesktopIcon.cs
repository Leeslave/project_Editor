using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DesktopIcon : UiDragHandler
{
    /*
     * Icon의 경우 Icon Object 밑에 Image 부분, Text 부분으로 구성되어 있다.
     */

    // Icon의 image 부분
    protected Image _iconImage;
    // Icon의 Image 부분의 Transform
    protected RectTransform _imageRect;
    // Icon의 Text
    protected TMP_Text _nameLabel;
    // Icon.의 Text 부분의 Transform
    protected RectTransform _textRect;
    // 드래그 시 사용될 Image
    protected Image _dragImage;
    // 드래그 시 이동되는 임시 Object의 Transform
    protected RectTransform _dragRect;
    // Icon 자체 Object의 Transform
    protected RectTransform _rectTransform;
    // Drag시 DoubleCheck에 카운터 되는 것을 방지하기 위해 사용
    protected bool _draggedDuringDoubleClick = false;
    // LayOut에 맞춰서 배치되는지 여부
    [SerializeField] protected bool _snapToGrid = true;
    // Icon을 통해 실행되는 프로세스
    [SerializeField] protected GameObject _openedProcess;
    // 폴더, 인물 파일, 뉴스 파일을 구분한다.
    public int FileType;
    // WindowsManager.cs
    [SerializeField] public DesktopFolder Folder;
    // Window상에서 배치되어 있는 LayOut의 위치 정보 저장에 사용.
    protected Tuple<int, int> _gridCell = null;
    // Pooling용.
    public int PoolIndex;

    protected bool _initializing = true;

    protected override void Awake()
    {
        base.Awake();
        _imageRect = transform.GetChild(0).GetComponent<RectTransform>();
        _iconImage = transform.GetChild(0).GetComponent<Image>();
        _nameLabel = transform.GetChild(1).GetComponent<TMP_Text>();
        _textRect = _nameLabel.GetComponent<RectTransform>();
        _dragImage = _dragTarget.GetComponent<Image>();
        _dragRect = _dragTarget.GetComponent<RectTransform>();
        _rectTransform = GetComponent<RectTransform>();
        UiEventUtility.AddEvent(GetComponent<EventTrigger>(), EventTriggerType.PointerUp,OpenIcon);
    }
    protected virtual void Start()
    {
        _gridCell = Folder.PlaceIcon(gameObject);
        _imageRect.sizeDelta = _rectTransform.sizeDelta * 0.8f;
        float cnt = _imageRect.sizeDelta.x - _imageRect.sizeDelta.y;
        _imageRect.anchoredPosition = new Vector2(0,(_imageRect.sizeDelta.x - _imageRect.sizeDelta.y)*0.5f);
        _textRect.anchoredPosition = new Vector2(0, -(_imageRect.sizeDelta.y) * 0.5f - 10);
        _nameLabel.fontSize = cnt;
        _initializing = false;
    }
    protected virtual void OnEnable()
    {
        if(!_initializing)_gridCell = Folder.PlaceIcon(gameObject);
    }
    protected virtual void OnDisable()
    {
        if(_gridCell!=null)Folder.RemoveIcon(_gridCell);
    }
    public virtual void ClearIcon()
    {
        Folder.ClearIcon(PoolIndex);
        _openedProcess = null;
        _gridCell = null;
    }

    /// <summary>
    /// Icon을 초기화
    /// </summary>
    /// <param name="AttatchAble">첨부 가능 여부</param>
    /// <param name="OpenProcess">클릭을 통해 실행 될 Process</param>
    /// <param name="name">Icon의 이름(Text에 사용)</param>
    /// <param name="Image">Icon의 이미지(Image에 사용)</param>
    /// <param name="num">PoolingNumber</param>
    public virtual void Init(bool canAttach, GameObject openProcess, string name, Sprite image, int num,int type)
    {
        this.name = name;
        _openedProcess = openProcess;
        _nameLabel.text = name;
        _iconImage.sprite = image;
        PoolIndex = num;
        FileType = type;
    }
    /// <summary>
    /// Icon 더블 클릭 탐지.
    /// 탐지 시 해당 Icon의 프로그램 실행.
    /// </summary>
    /// <param name="Data"></param>
    protected virtual void OpenIcon(PointerEventData data)
    {
        if (_openedProcess != null)
        {
            if (_openedProcess.activeSelf) return;
        }
        if (_draggedDuringDoubleClick)
        {
            _draggedDuringDoubleClick = false;
            return;
        }
        if (data.clickCount == 2)
        {
            if (_openedProcess != null)
            {
                _openedProcess.SetActive(true);
                _openedProcess.transform.SetAsLastSibling();
            }
            ClickEvent();
        }
    }
    /// <summary>
    /// Icon 더블 클릭시 발생하는 이벤트.
    /// </summary>
    protected virtual void ClickEvent()
    {

    }

    protected override void Click(PointerEventData data)
    {
        base.Click(data);
        _dragRect.position = _rectTransform.position;
    }

    /// <summary>
    /// Drag시 임시 Object를 활성화하며, 임시 Object의 Image를 Icon의 Image로 변환
    /// </summary>
    /// <param name="Data"></param>
    protected override void DragOn(PointerEventData data)
    {
        base.DragOn(data);
        if (data.clickCount == 2) _draggedDuringDoubleClick = true;
        _dragTarget.transform.position = transform.position - _pointerOffset;
        _dragTarget.gameObject.SetActive(true);
        EditingDesktopController.Instance.DragState.IsDragged = true;
        EditingDesktopController.Instance.DragState.FileType = FileType;
        EditingDesktopController.Instance.DragState.IconName = name;
        _dragImage.sprite = _iconImage.sprite;
        EditingDesktopController.Instance.DragState.DraggedObject = gameObject;
        _dragRect.sizeDelta = _imageRect.sizeDelta;

    }

    /// <summary>
    /// 드래그 종료 시 Icon의 첨부, 이동, 삭제를 구분.
    /// </summary>
    /// <param name="Data"></param>
    protected override void DragEnd(PointerEventData data)
    {
        if (EditingDesktopController.Instance.DragState.DropAction!=null) EditingDesktopController.Instance.DragState.DropAction();
        else if (_snapToGrid) Folder.MoveIcon(gameObject, _dragTarget, ref _gridCell);
        else _rectTransform.position = _dragRect.position;
        _dragTarget.gameObject.SetActive(false);
    }
}
