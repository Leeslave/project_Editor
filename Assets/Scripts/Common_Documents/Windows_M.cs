using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Windows_M : MonoBehaviour
{
    [Header("- Window 크기")]
    [SerializeField] protected int Size_X;
    [SerializeField] protected int Size_Y;
    [Header("- Icon 크기")]
    [SerializeField] protected int Icon_Size_X;
    [SerializeField] protected int Icon_Size_Y;
    [Header("- 간격")]
    [SerializeField] protected int Space_X;
    [SerializeField] protected int Space_Y;

    protected int Start_X;
    protected int Start_Y;
    protected int NumX;
    protected int NumY;
    protected GraphicRaycaster GR;
    protected bool CanMove = true;

    protected RectTransform MyRect;

    protected bool[,] Occupied;
    [Header("따로 사전에 생성된 Icon들\n따로 안 넣어두면 버그 납니다.")]
    [SerializeField] List<GameObject> EarlyIcon;
    [Header("(주의) IconSample을 무조건 Field에 생성한 후\n기본적인 Prefab을 적용한 후 넣어주세요.")]
    
    [SerializeField] GameObject IconSample;
    [Header("풀링 수(최대 30)")]
    int PoolingNum;
    protected GameObject[] Icons;
    protected UIICons[] IconsScript;
    protected bool[] IconUseAble;

    [SerializeField] bool j = false;
    private void Awake()
    {
        Windows_M Window = GetComponent<Windows_M>();
        SizeChanger();
        Icons = new GameObject[PoolingNum];
        IconsScript = new UIICons[PoolingNum];
        IconUseAble = new bool[PoolingNum];
        for (int i = 0; i < EarlyIcon.Count; i++) Icons[i] = EarlyIcon[i];
        for (int i = EarlyIcon.Count; i < PoolingNum; i++) Icons[i] = Instantiate(IconSample, transform);
        for (int i = 0; i < PoolingNum; i++)
        {
            IconsScript[i] = Icons[i].GetComponent<UIICons>();
            IconsScript[i].PoolNum = i;
            IconsScript[i].Window = Window;
        }
        for (int i = 0; i < PoolingNum; i++) IconUseAble[i] = true;
        for (int i = 0; i < EarlyIcon.Count; i++) IconUseAble[i] = false;
        EarlyIcon.Clear();
        if(j) gameObject.SetActive(false);
    }

    /// <summary>
    /// Window 자체의 Size 변경.
    /// </summary>
    protected virtual void SizeChanger()
    {
        MyRect = GetComponent<RectTransform>();
        MyRect.sizeDelta = new Vector2(Size_X, Size_Y);
        // Icon 배치 공간 계산
        NumX = (Size_X + Space_X) / (Space_X + Icon_Size_X);
        NumY = (Size_Y + Space_Y) / (Space_Y + Icon_Size_Y);
        Occupied = new bool[NumY, NumX];
        PoolingNum = NumY * NumX;
        Start_X = (int)(-(Icon_Size_X * NumX + Space_X*(NumX-1)) * 0.5);
        Start_Y = (int)((Icon_Size_Y * NumY + Space_Y * (NumY- 1))* 0.5);
        for (int i = 0; i < NumY; i++) for (int z = 0; z < NumX; z++) Occupied[i, z] = false;
    }

    /// <summary>
    /// 드래그에 의한 ICon 이동.
    /// 이미 해당 위치에 배치된 Icon이 있으면 이동하지 못함.
    /// </summary>
    /// <param name="Call"> 이동을 요청한 Icon </param>
    /// <param name="Dragged"> 드래그로 따로 움직이는 Cnt </param>
    /// <param name="CallLay"> Icon이 자체적으로 가지고 있는 Occupied의 주소 </param>
    public virtual void BatchByMove(GameObject Call,Transform Dragged,ref Tuple<int,int>CallLay)
    {
        if (!CanMove) return;
        RectTransform cnt = Call.GetComponent<RectTransform>();
        int x = (int)((Dragged.position.x - Start_X - MyRect.anchoredPosition.x) / (Icon_Size_X + Space_X));
        int y = (int)((Start_Y - Dragged.position.y + MyRect.anchoredPosition.y) / (Icon_Size_Y + Space_Y));
        if (x >= NumX || x < 0 || y >= NumY || y < 0) return;
        if (Occupied[y, x] == true) return;
        Occupied[CallLay.Item2,CallLay.Item1] = false;
        Occupied[y, x] = true;
        CallLay = new Tuple<int, int>(x,y);
        cnt.anchoredPosition = new Vector3(
                         Start_X + x * (Icon_Size_X + Space_X) + cnt.pivot.x * Icon_Size_X,
                         Start_Y - y * (Icon_Size_Y + Space_Y) - cnt.pivot.y * Icon_Size_Y, 0);
    }

    /// <summary>
    /// Icon 생성이 될 때 Window에 배치해줌.
    /// </summary>
    /// <param name="a"> 생성되는 Icon </param>
    /// <returns></returns>
    public virtual Tuple<int,int> BatchByCreate(GameObject a)
    {
        for(int x = 0; x < NumX; x++) for(int y = 0; y < NumY; y++)
                if (Occupied[y,x] == false)
                {
                    Occupied[y, x] = true;
                    RectTransform cnt = a.GetComponent<RectTransform>();
                    cnt.anchoredPosition = new Vector3(
                         Start_X + x * (Icon_Size_X + Space_X) + cnt.pivot.x * Icon_Size_X,
                         Start_Y - y * (Icon_Size_Y + Space_Y) - cnt.pivot.y * Icon_Size_Y, 0);
                    cnt.sizeDelta = new Vector2(Icon_Size_X, Icon_Size_Y);
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
    public virtual GameObject NewIcon(string name, Sprite Image,int type = -1, bool AttatchAble = false, GameObject OpenProcess = null)
    {
        for(int i = 0; i < PoolingNum; i++)
        {
            if (IconUseAble[i])
            {
                IconUseAble[i] = false;
                Icons[i].SetActive(true);
                IconsScript[i].Init(AttatchAble,OpenProcess,name,Image,i,type);
                return Icons[i];
            }
        }
        return null;
    }
    /// <summary>
    /// 특정 작업을 통해 LayOut이 변경된 Icon의 기존 LayOut을 이용 가능하게 변경
    /// </summary>
    /// <param name="CallLay"> Icon의 현재 LayOut상에서의 위치 </param>
    public virtual void RemoveIcon(Tuple<int,int> CallLay)
    {
        Occupied[CallLay.Item2,CallLay.Item1] = false;
    }
    /// <summary>
    /// 해당 PoolNum의 Icon을 Pooling 가능하게 변경.
    /// </summary>
    /// <param name="ind"></param>
    public virtual void ClearIcon(int ind)
    {
        IconUseAble[ind] = true;
    }
}
