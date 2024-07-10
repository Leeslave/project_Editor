using System.Linq;
using TMPro;
using UnityEngine;
using Image = UnityEngine.UI.Image;

public class UI_TargetAreaPopUpV2 : MonoBehaviour
{
    private Image TargetArea { get; set; }
    private Image[] Background { get; set; }
    private TMP_Text TargetName { get; set; }
    private TMP_Text[] InstructionText { get; set; }
    private Image[] InstructionImage { get; set; }

    private void Awake()
    {
        TargetArea = transform.GetChild(0).GetComponent<Image>();
        Background = TargetArea.transform.GetComponentsInChildren<Image>();
        TargetName = TargetArea.transform.GetComponentInChildren<TMP_Text>();
        InstructionText = transform.GetChild(1).GetComponentsInChildren<TMP_Text>();
        InstructionImage = transform.GetChild(2).GetComponentsInChildren<Image>();
        
        TargetArea.color = Color.clear;
        TargetName.color = Color.clear;
        foreach (var i in Background)
            i.color = Color.clear;
        foreach (var i in InstructionText)
            i.color = Color.clear;
        foreach (var i in InstructionImage)
            i.color = Color.clear;
    }

    private void OnEnable()
    {
        LJWConverter.Instance.GradientImageColor(false, 0f, 0.5f, new Color(1f, 0.2f, 0.2f), TargetArea);
        LJWConverter.Instance.GradientTMPColor(false, 0.5f, 0.5f, new Color(1f, 0.2f, 0.2f), TargetName);

        foreach (var i in Background)
            LJWConverter.Instance.GradientImageColor(false, 0.5f, 0.5f, new Color(0f, 0f, 0f, 0.8f), i);
        foreach(var i in InstructionText)
            LJWConverter.Instance.GradientTMPColor(false, 0.5f, 0.5f, Color.white, i);
        foreach (var i in InstructionImage)
            LJWConverter.Instance.GradientImageColor(false, 0.5f, 0.5f, Color.white, i);
    }

    private void OnDisable()
    {
        TargetArea.color = Color.clear;
        TargetName.color = Color.clear;
        foreach (var i in Background)
            i.color = Color.clear;
        foreach (var i in InstructionText)
            i.color = Color.clear;
        foreach (var i in InstructionImage)
            i.color = Color.clear;
    }
}
