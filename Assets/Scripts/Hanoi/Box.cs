using UnityEngine;
using TMPro;


// �ڽ��� ������ ����
public class Box : MonoBehaviour
{
    public int MaxDurability;
    public int Weight;
    public int BoxNum;
    public HanoiManager HM;

    bool IsBreakAble = false;
    int CurDurability = 0;
    TMP_Text Dura;

    /// <summary>
    /// Box Object Awake�� �������� �������� �ο��� ������ ����
    /// TODO : �������� �� ������ �������� ������ �ο�
    /// </summary>
    private void Awake()
    {
        if (Random.Range(0,2)== 0)
        {
            CurDurability = MaxDurability;
            Dura = transform.GetChild(0).GetComponent<TMP_Text>();
            Dura.text = CurDurability.ToString();
            Dura.gameObject.SetActive(true);
            IsBreakAble = true;
        }
    }

    /// <summary>
    /// ���� �ڽ��� B�ڽ� ���� ���� �� �ִ°� ���� ��ȯ.
    /// </summary>
    /// <param name="B">���� �������� Container �ֻ���� Box</param>
    /// <returns></returns>
    public bool CanAdd(Box B)
    {
        if (B.Weight < Weight) return false;
        return true;
    }
    /// <summary>
    /// ���� �ڽ��� ���� �� �ִ��� ���� ��ȯ(������)
    /// </summary>
    /// <returns></returns>
    public bool CanPick()
    {
        if (IsBreakAble) return CurDurability != 0;
        return true;
    }
    /// <summary>
    /// ���� �ڽ��� �������� ��ȯ��
    /// </summary>
    /// <param name="change">��ȭ��</param>
    public void DuraChange(int change)
    {
        if (IsBreakAble)
        {
            if (change > 0 && HM.NextBox < BoxNum) HM.NextBox = BoxNum;
            CurDurability += change;
            Dura.text = CurDurability.ToString();
            if (CurDurability == 0) HM.ErrorEvent(BoxNum);
        }
    }


}
