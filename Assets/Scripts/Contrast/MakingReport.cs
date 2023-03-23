using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

public class MakingReport : MonoBehaviour
{
    public GameObject Contrast;
    public ContrastManager CM;

    public GameObject ChangeButton;

    public TMP_Text Text;

    private void Start()
    {
        CM = Contrast.GetComponent<ContrastManager>();
    }

    public void Making(Dictionary<string, object> Data)
    {
        TMP_Text CntText = Instantiate(Text);
        CntText.transform.SetParent(transform);
        CntText.name = "";
        ReportText tmp = CntText.GetComponent<ReportText>();
        tmp.Time = Data["Time"].ToString();
        ChangeButton.GetComponent<ReportChangeButton>().TimeList.Add(Data["Time"].ToString());
        tmp.Place = Data["Place"].ToString();
        tmp.Action = Data["Action"].ToString();
        tmp.ChangeText();

        tmp.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);    // 계속 Scale 커지는 버그 방지
    }
}
