using UnityEngine;
using TMPro;


// 박스의 정보를 저장(아직 무게만 사용)
public class Box : MonoBehaviour
{
    public int MaxDurability;
    public int Weight;
    public int BoxNum;
    public HanoiManager HM;

    bool IsBreakAble = false;
    int CurDurability = 0;
    TMP_Text Dura;

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

    public bool CanAdd(Box B)
    {
        if (B.Weight < Weight) return false;
        return true;
    }
    public bool CanPick()
    {
        if (IsBreakAble) return CurDurability != 0;
        return true;
    }

    public void DuraChange(int change)
    {
        if (IsBreakAble)
        {
            CurDurability += change;
            Dura.text = CurDurability.ToString();
            if (CurDurability == 0) HM.ErrorEvent(BoxNum);
        }
    }


}
