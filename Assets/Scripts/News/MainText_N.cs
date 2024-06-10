using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// News의 InputField 담당 Script
public class MainText_N : MonoBehaviour
{
    public TMP_Text Text;
    [SerializeField] RectTransform ReBuild;
    public int MyInd;                           // 현재 News에서 몇 번째 줄인지
    [NonSerialized] public bool OnAddButton = false;       // 줄 생성 버튼이 작동하게 함(InputField가 꺼지면서 작동이 안됨)
    [NonSerialized] public bool OnRemoveButton = false;     // 줄 삭제 버튼이 작동하게 함

    public TMP_InputField Field;
    [SerializeField] TMP_Text FieldText;
    [SerializeField] RectTransform MyRect;

    [SerializeField] MainText_Back Parent;

    Vector2 Sub = new Vector3(0, 20);

    private void Awake()
    {
        Field.onEndEdit.AddListener(Enter);
        Field.onValueChanged.AddListener(Delete);
    }
    /// <summary>
    /// Field의 수정이 끝났거나, Field 이외의 것을 마우스로 클릭한 경우 활성화
    /// </summary>
    /// <param name="text">현재 Field의 Text</param>
    public void Enter(string text)
    {
        Text.text = Field.text;
        if (text != MyLast) DB_M.DB_Docs.NewsManager.Commands_Back.Add(new Tuple<string, int, int>(MyLast, MyInd, 0));
        gameObject.SetActive(false);
        Text.gameObject.SetActive(true);
        
        //LayoutRebuilder.ForceRebuildLayoutImmediate(ReBuild);
        if (OnAddButton) AddUnder();
        if (OnRemoveButton) DelSelf();
    }

    public void ReBuildRect()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(ReBuild);
    }

    // 버튼 클릭시 일정 시간 이후 SetActvie(False)
    void ButtonSub()
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 줄을 삭제할 때 활성화
    /// </summary>
    /// <param name="text">현재 Field의 Text</param>
    void Delete(string text)
    {
        if (text.Length != 0)
        {
            MyRect.sizeDelta = new Vector2(MyRect.sizeDelta.x, Field.preferredHeight + 20);
            LayoutRebuilder.ForceRebuildLayoutImmediate(ReBuild);
        }
    }


    /// <summary>
    /// 현재 줄 아랫부분에 추가 및 Manager에 해당 사실을 알림
    /// </summary>
    public void AddUnder()
    {
        DB_M.DB_Docs.NewsManager.ActiveText("Empty", MyInd);
    }

    /// <summary>
    /// 줄 삭제(Object 비활성화 및 Manager에 해당 사실을 알림)
    /// </summary>
    public void DelSelf(bool IsRollBack = false)
    {
        DB_M.DB_Docs.NewsManager.RemoveText(Parent.MyInd, Text.text, IsRollBack);
    }

    /*
     * Add 및 Delete가 여러개인 이유는 PoolingIndex와, 현재 뉴스에서 몇 번째 줄인지 구분하는 Index가 다르며
     * 해당 Index를 각각 MainText_Back, MainText_N에 따로 저장해 두었기 때문
     */

    /// <summary>
    /// 줄 삭제(내용 지움)
    /// </summary>
    public void DelLine()
    {
        Text.text = "";
        Field.text = "";
        MyRect.sizeDelta = new Vector2(550, 40);
    }
    /// <summary>
    /// 줄 추가(내용 추가)
    /// </summary>
    /// <param name="text">추가할 내용</param>
    /// <param name="Ind"> 현재 몇번째 줄인지 </param>
    public void AddLine(string text, int Ind)
    {
        Text.text = text;
        MyInd = Ind;
    }

    string MyLast = "";
    private void OnEnable()
    {
        OnAddButton = false; OnRemoveButton = false;
        MyLast = Field.text = Text.text;
        MyRect.sizeDelta = new Vector2(MyRect.sizeDelta.x, Field.preferredHeight + 20);
        LayoutRebuilder.ForceRebuildLayoutImmediate(ReBuild);
        Field.MoveTextEnd(false);
    }

    private void OnDisable()
    {
        MyLast = Text.text = Field.text;
        Text.gameObject.SetActive(true);
        CheckMyText();
        LayoutRebuilder.ForceRebuildLayoutImmediate(ReBuild);
    }

    public void CheckMyText()
    {
        DB_M.DB_Docs.NewsManager.ValidText(true, transform.parent.GetSiblingIndex() - 4, Text.text);
    }
}
