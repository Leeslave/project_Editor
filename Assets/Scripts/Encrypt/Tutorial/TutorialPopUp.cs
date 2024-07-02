using System;
using System.Linq;
using TMPro;
using UnityEngine;
using Image = UnityEngine.UI.Image;

public class TutorialPopUp : MonoBehaviour
{
    private Image TargetArea { get; set; }
    private Image[] Background { get; set; } = new Image[8];
    private TextMeshProUGUI TargetName { get; set; }
    private TextMeshProUGUI Instruction { get; set; }
    private TextMeshProUGUI Speaker { get; set; }

    private void Awake()
    {
        TargetArea = transform.GetChild(0).GetComponent<Image>();
        Background = TargetArea.transform.GetComponentsInChildren<Image>().Skip(1).ToArray();
        TargetName = TargetArea.transform.GetComponentInChildren<TextMeshProUGUI>();
        Instruction = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        Speaker = Instruction.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        
        TargetArea.color = Color.clear;
        for (var i = 0; i < 8; i++)
            Background[i].rectTransform.sizeDelta = Vector2.zero;
        TargetName.color = Color.clear;
        Instruction.color = Color.clear;
        Speaker.color = Color.clear;
        
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        LJWConverter.Instance.GradientImageColor(false, 0f, 0.5f, new Color(1f, 0.2f, 0.2f), TargetArea);
        
        LJWConverter.Instance.SizeRectTransform(false, 0f, 0.5f, Vector2.up * 1000f, Background[0].rectTransform);
        LJWConverter.Instance.SizeRectTransform(false, 0f, 0.5f, Vector2.one * 1000f, Background[1].rectTransform);
        LJWConverter.Instance.SizeRectTransform(false, 0f, 0.5f, Vector2.right * 1000f, Background[2].rectTransform);
        LJWConverter.Instance.SizeRectTransform(false, 0f, 0.5f, Vector2.one * 1000f, Background[3].rectTransform);
        LJWConverter.Instance.SizeRectTransform(false, 0f, 0.5f, Vector2.up * 1000f, Background[4].rectTransform);
        LJWConverter.Instance.SizeRectTransform(false, 0f, 0.5f, Vector2.one * 1000f, Background[5].rectTransform);
        LJWConverter.Instance.SizeRectTransform(false, 0f, 0.5f, Vector2.right * 1000f, Background[6].rectTransform);
        LJWConverter.Instance.SizeRectTransform(false, 0f, 0.5f, Vector2.one * 1000f, Background[7].rectTransform);
        
        LJWConverter.Instance.GradientUGUIColor(false, 0.5f, 0.5f, new Color(1f, 0.2f, 0.2f), TargetName);
        LJWConverter.Instance.GradientUGUIColor(false, 0.5f, 0.5f, Color.white, Instruction);
        LJWConverter.Instance.GradientUGUIColor(false, 0.5f, 0.5f, Color.white, Speaker);
    }

    private void OnDisable()
    {
        TargetArea.color = Color.clear;
        for (var i = 0; i < 8; i++)
            Background[i].rectTransform.sizeDelta = Vector2.zero;
        TargetName.color = Color.clear;
        Instruction.color = Color.clear;
        Speaker.color = Color.clear;
    }
}
