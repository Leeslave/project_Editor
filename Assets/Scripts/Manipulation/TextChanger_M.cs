using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

// 인물 수정 사항에서 Text뒷부분에 빨간 강조선 생성을 위한 Object에 사용

public class TextChanger_M : MonoBehaviour
{
    
    [SerializeField] int Ind;

    RectTransform s;
    RectTransform S;
    TMP_Text j;


    private void Awake()
    {
        S = GetComponent<RectTransform>();
        s = transform.GetChild(0).GetComponent<RectTransform>();
        j = transform.GetChild(0).GetComponent<TMP_Text>();
        
    }

    /// <summary>
    /// 변경된 Text의 크기에 맞춰 Size 변경
    /// </summary>
    /// <param name="a">변경될 Text</param>
    public void Changer(string a)
    {
        j.text = a;
        DB_M.DB_Docs.PersonDataManager.ValidData(Ind, a);
        Invoke("SizeChange", 0.05f);
    }

    void SizeChange()
    {
        S.sizeDelta = s.sizeDelta;
    }
}
