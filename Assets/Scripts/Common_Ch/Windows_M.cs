using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Windows_M : MonoBehaviour
{
    [Header("- Window ũ��")]
    [SerializeField] protected int Size_X;
    [SerializeField] protected int Size_Y;
    [Header("- Icon ũ��")]
    [SerializeField] protected int Icon_Size_X;
    [SerializeField] protected int Icon_Size_Y;
    [Header("- ����")]
    [SerializeField] protected int Space_X;
    [SerializeField] protected int Space_Y;

    protected int Start_X;
    protected int Start_Y;
    protected int NumX;
    protected int NumY;
    protected GraphicRaycaster GR;
    protected bool CanMove = false;

    protected RectTransform MyRect;

    protected bool[,] Occupied;
    [Header("���� ������ ������ Icon��\n���� �� �־�θ� ���� ���ϴ�.")]
    [SerializeField] List<GameObject> EarlyIcon;
    [Header("(����) IconSample�� ������ Field�� ������ ��\n�⺻���� Prefab�� ������ �� �־��ּ���.")]
    [Header("Ǯ�� ���� 30��")]
    [SerializeField] GameObject IconSample;
    protected GameObject[] Icons = new GameObject[30];
    protected UIICons[] IconsScript = new UIICons[30];
    protected bool[] IconUseAble = new bool[30];

    private void Awake()
    {
        for (int i = 0; i < EarlyIcon.Count; i++) Icons[i] = EarlyIcon[i];
        for (int i = EarlyIcon.Count; i < 30; i++) Icons[i] = Instantiate(IconSample, transform);
        for (int i = 0; i < 30; i++) IconsScript[i] = Icons[i].GetComponent<UIICons>();
        for (int i = 0; i < 30; i++) IconsScript[i].PoolNum = i;
        for (int i = 0; i < 30; i++) IconUseAble[i] = true;
        for (int i = 0; i < EarlyIcon.Count; i++) IconUseAble[i] = false;
        SizeChanger();

        EarlyIcon.Clear();
        GR = GetComponent<GraphicRaycaster>();
    }

    /// <summary>
    /// �ٸ� ���μ��� ���� ���콺�� ���� ��, �巡��&����� �Ұ����ϰ� ��
    /// </summary>
    private void FixedUpdate()
    {
        if (MyUi.GRay(GR).Count != 0) CanMove = false;
        else CanMove = true;
    }

    /// <summary>
    /// Window ��ü�� Size ����.
    /// </summary>
    protected virtual void SizeChanger()
    {
        MyRect = GetComponent<RectTransform>();
        MyRect.sizeDelta = new Vector2(Size_X, Size_Y);
        // Icon ��ġ ���� ���
        NumX = (Size_X + Space_X) / (Space_X + Icon_Size_X);
        NumY = (Size_Y + Space_Y) / (Space_Y + Icon_Size_Y);
        Occupied = new bool[NumY, NumX];
        Start_X = (int)(-(Icon_Size_X * NumX + Space_X*(NumX-1)) * 0.5);
        Start_Y = (int)((Icon_Size_Y * NumY + Space_Y * (NumY- 1))* 0.5);
        for (int i = 0; i < NumY; i++) for (int z = 0; z < NumX; z++) Occupied[i, z] = false;
    }

    /// <summary>
    /// �巡�׿� ���� ICon �̵�.
    /// �̹� �ش� ��ġ�� ��ġ�� Icon�� ������ �̵����� ����.
    /// </summary>
    /// <param name="Call"> �̵��� ��û�� Icon </param>
    /// <param name="Dragged"> �巡�׷� ���� �����̴� Cnt </param>
    /// <param name="CallLay"> Icon�� ��ü������ ������ �ִ� Occupied�� �ּ� </param>
    public virtual void BatchByMove(GameObject Call,Transform Dragged,ref Tuple<int,int>CallLay)
    {
        if (!CanMove) return;
        RectTransform cnt = Call.GetComponent<RectTransform>();
        int x = (int)((Dragged.position.x - Start_X) / (Icon_Size_X + Space_X)); if (x < 0) x = 0;
        int y = (int)((Start_Y - Dragged.position.y) / (Icon_Size_Y + Space_Y)); if (y < 0) y = 0;
        if (Occupied[y, x] == true) return;
        Occupied[CallLay.Item2,CallLay.Item1] = false;
        Occupied[y, x] = true;
        CallLay = new Tuple<int, int>(x,y);
        cnt.position = new Vector3(
                         Start_X + x * (Icon_Size_X + Space_X) + cnt.pivot.x * Icon_Size_X,
                         Start_Y - y * (Icon_Size_Y + Space_Y) - cnt.pivot.y * Icon_Size_Y, 0);
    }

    /// <summary>
    /// Icon ������ �� �� Window�� ��ġ����.
    /// </summary>
    /// <param name="a"> �����Ǵ� Icon </param>
    /// <returns></returns>
    public virtual Tuple<int,int> BatchByCreate(GameObject a)
    {
        for(int x = 0; x < NumX; x++) for(int y = 0; y < NumY; y++)
                if (Occupied[y,x] == false)
                {
                    Occupied[y, x] = true;
                    RectTransform cnt = a.GetComponent<RectTransform>();
                    cnt.position = new Vector3(
                         Start_X + x * (Icon_Size_X + Space_X) + cnt.pivot.x * Icon_Size_X,
                         Start_Y - y * (Icon_Size_Y + Space_Y) - cnt.pivot.y * Icon_Size_Y, 0);
                    cnt.sizeDelta = new Vector2(Icon_Size_X, Icon_Size_Y);
                    print(cnt.sizeDelta);
                    return new Tuple<int,int>(x, y);
                }
        return null;
    }

    /// <summary>
    /// Icon ����. �⺻������ ���Ǵ� UIIcons�̿��� �ٸ� ���� ��� �� ���� �ʿ�.
    /// </summary>
    /// <param name="AttatchAble"> �޼����� ÷�� ���� ���� </param>
    /// <param name="OpenProcess"> Ŭ�� �� ����Ǵ� ���μ��� </param>
    /// <param name="name"> �̸� </param>
    /// <param name="Image"> �̹��� </param>
    public virtual GameObject NewIcon(bool AttatchAble, GameObject OpenProcess, string name, Sprite Image)
    {
        for(int i = 0; i < 30; i++)
        {
            if (IconUseAble[i])
            {
                IconUseAble[i] = false;
                Icons[i].SetActive(true);
                IconsScript[i].Init(AttatchAble,OpenProcess,name,Image,i);
                return Icons[i];
            }
        }
        return null;
    }
    /// <summary>
    /// Ư�� �۾��� ���� LayOut�� ����� Icon�� ���� LayOut�� �̿� �����ϰ� ����
    /// </summary>
    /// <param name="CallLay"> Icon�� ���� LayOut�󿡼��� ��ġ </param>
    public virtual void RemoveIcon(Tuple<int,int> CallLay)
    {
        Occupied[CallLay.Item2,CallLay.Item1] = false;
    }
    /// <summary>
    /// �ش� PoolNum�� Icon�� Pooling �����ϰ� ����.
    /// </summary>
    /// <param name="ind"></param>
    public virtual void ClearIcon(int ind)
    {
        IconUseAble[ind] = true;
    }
}
