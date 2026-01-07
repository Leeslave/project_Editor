using UnityEngine;
using TMPro;


// 박스의 정보를 저장
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
    /// Box Object 내구도를 부여
    /// </summary>

    public void SetBreak()
    {
        CurDurability = MaxDurability;
        Dura = transform.GetChild(0).GetComponent<TMP_Text>();
        Dura.text = CurDurability.ToString();
        Dura.gameObject.SetActive(true);
        IsBreakAble = true;
    }

    /// <summary>
    /// 현재 박스를 B박스 위에 놓을 수 있는가 여부 반환.
    /// </summary>
    /// <param name="B">현재 놓으려는 Container 최상단의 Box</param>
    /// <returns></returns>
    public bool CanAdd(Box B)
    {
        if (B.Weight < Weight) return false;
        return true;
    }
    /// <summary>
    /// 현재 박스를 집을 수 있는지 여부 반환(내구도)
    /// </summary>
    /// <returns></returns>
    public bool CanPick()
    {
        if (IsBreakAble) return CurDurability != 0;
        return true;
    }
    /// <summary>
    /// 현재 박스의 내구도를 변환함
    /// </summary>
    /// <param name="change">변화량</param>
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
