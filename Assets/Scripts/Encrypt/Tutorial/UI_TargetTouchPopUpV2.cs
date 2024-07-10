using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_TargetTouchPopUpV2 : MonoBehaviour, IPointerClickHandler
{
    private Image Target { get; set; }
    private Image Background { get; set; }
    private Image Outline { get; set; }
    private TMP_Text TargetName { get; set; }
    private TMP_Text[] InstructionText { get; set; }
    private Image[] InstructionImage { get; set; }

    [SerializeField] private float waitingTime;
    private bool _readyToGo;

    private void Awake()
    {
        Target = transform.GetChild(0).GetComponent<Image>();
        Background = Target.transform.GetChild(0).GetComponent<Image>();
        Outline = transform.GetChild(1).GetComponent<Image>();
        TargetName = Outline.transform.GetChild(0).GetComponent<TMP_Text>();
        InstructionText = transform.GetChild(2).GetComponentsInChildren<TMP_Text>();
        InstructionImage = transform.GetChild(3).GetComponentsInChildren<Image>();
        
        Background.color = Color.clear;
        Outline.color = Color.clear;
        TargetName.color = Color.clear;
        foreach(var i in InstructionText)
            i.color = Color.clear;
        foreach (var i in InstructionImage)
            i.color = Color.clear;
    }
    
    private void OnEnable()
    {
        _readyToGo = false;
        LJWConverter.Instance.GradientImageColor(false, 0f, 0.5f, new Color(0f, 0f, 0f, 0.8f), Background);
        LJWConverter.Instance.GradientImageColor(false, 0f, 0.5f, new Color(1f, 0.2f, 0.2f, 1f), Outline);
        LJWConverter.Instance.GradientTMPColor(false, 0.5f, 0.5f, new Color(1f, 0.2f, 0.2f, 1f), TargetName);
        foreach (var i in InstructionText)
            LJWConverter.Instance.GradientTMPColor(false, 0.5f, 0.5f, Color.white, i);
        foreach (var i in InstructionImage)
            LJWConverter.Instance.GradientImageColor(false, 0.5f, 0.5f, Color.white, i);
        StartCoroutine(Delay());
    }

    private void OnDisable()
    {
        Background.color = Color.clear;
        Outline.color = Color.clear;
        TargetName.color = Color.clear;
        foreach(var i in InstructionText)
            i.color = Color.clear;
        foreach (var i in InstructionImage)
            i.color = Color.clear;
    }

    private IEnumerator Delay()
    {
        yield return new WaitForSeconds(waitingTime);
        _readyToGo = true;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(_readyToGo)
            gameObject.SetActive(false);
    }
}