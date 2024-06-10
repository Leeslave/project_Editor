using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class BasicInputField : MonoBehaviour
{
    public TextMeshPro InputFieldTMP { get; set; }
    public SpriteRenderer InputFieldFrame { get; set; }
    public SpriteRenderer InputFieldFill { get; set; }
    public BoxCollider2D InputFieldCollider { get; set; }
    private bool IsMouseOver { get; set; } = false;
    private bool IsActive { get; set; } = true;

    public Color Enter = new Color(1f, 1f, 1f, 0.5f);
    public Color Exit = new Color(0.5f, 0.5f, 0.5f, 0.0f);
    public Color Down = new Color(0.5f, 0.5f, 0.5f, 0.5f);
    public Color Up = new Color(1f, 1f, 1f, 0.5f);
    public Color Writing = new Color(0.1764f, 1f, 0.1764f, 0.5f);

    public UnityEvent OnMouseDownEvent;
    public UnityEvent OnMouseUpEvent;
    public UnityEvent OnMouseEnterEvent;
    public UnityEvent OnMouseExitEvent;
    public UnityEvent OnAddInputField;
    public UnityEvent OnDeleteInputField;
    public UnityEvent OnReturnInputField;
   
    public string DefaultText;

    public int MaxInputLength = 20;

    public String AddAbleCharSet = "ABCDEFGHIJKLMNOPQRSTUWVXYZ-123456789";

    public bool AddSpaceBehindChar;
    
    public string StringBuffer { get; set; } = "";
    public bool IsReadyForInput { get; set; }
    public bool IsFlash { get; set; } = false;
    private bool SkipFlash { get; set; }
    
    private void Awake()
    {
        InputFieldTMP = transform.GetChild(0).GetComponent<TextMeshPro>();
        InputFieldFrame = transform.GetChild(1).GetComponent<SpriteRenderer>();
        InputFieldFill = transform.GetChild(2).GetComponent<SpriteRenderer>();
        InputFieldCollider = transform.GetComponent<BoxCollider2D>();

        InputFieldFill.color = Exit;
        
        CheckInputField();
        
        StartCoroutine(StartFlashInputField());
    }
    
    /// <summary>
    /// 인풋 필드 초기화
    /// </summary>
    public void Initialize()
    {
        StringBuffer = "";
        InputFieldTMP.text = DefaultText;
    }

    /// <summary>
    /// 인풋 필드에 대한 접근성을 설정한다
    /// </summary>
    /// <param name="value"> 사용 가능 여부 </param>
    public void SetAvailability(bool value)
    {
        //초기화
        InputFieldFill.color = Exit;
        //value에 따라서 인풋 필드는 입력 가능 여부가 결정된다
        IsActive = value;
        //항상 false로 설정해야 차단이 풀리는 순간 새롭게 Enter 이벤트가 발생한다
        IsMouseOver = false;
    }

    /// <summary>
    /// wait 이후에 인풋 필드에 대한 접근성을 차단하고, 차단 시점에서 duration 이후에 회복한다
    /// </summary>
    /// <param name="wait"></param>
    /// <param name="duration"></param>
    public void CutAvailabilityForWhile(float wait, float duration)
    {
        StartCoroutine(CutAvailabilityForWhile_IE(wait, duration));
    }
    private IEnumerator CutAvailabilityForWhile_IE(float wait, float duration)
    {
        yield return new WaitForSeconds(wait);
        SetAvailability(false);
        yield return new WaitForSeconds(duration);
        SetAvailability(true);
    }
    
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {            
            if(IsMouseOver)
                return;

            InputFieldTMP.text = StringBuffer;
            CheckInputField(); 
            IsReadyForInput = false;
            IsFlash = false;
            BasicKeyboard.Instance.ConnectedInputField = null;
            LJWConverter.Instance.GradientSpriteRendererColor(false, 0.0f, 0.2f, Exit, InputFieldFill);
        }
    }

    protected virtual void OnMouseDown()
    {
        if (!IsActive)
            return;
        if (!IsMouseOver)
            return;
        LJWConverter.Instance.GradientSpriteRendererColor(false, 0.0f, 0f, Down, InputFieldFill);
        OnMouseDownEvent.Invoke();
    }

    protected virtual void OnMouseUp()
    {
        if (!IsActive)
            return;
        if (!IsMouseOver)
            return;

        InputFieldTMP.text = StringBuffer;
        IsReadyForInput = true;
        IsFlash = false;
        BasicKeyboard.Instance.ConnectedInputField = this;
        LJWConverter.Instance.GradientSpriteRendererColor(false, 0.0f, 0.2f, Writing, InputFieldFill);
        OnMouseUpEvent.Invoke();
    }

    protected virtual void OnMouseOver()
    {
        if (!IsActive)
            return;
        if (IsMouseOver)
            return;
        
        IsMouseOver = true;
        
        if (IsReadyForInput)
            return;
        
        LJWConverter.Instance.GradientSpriteRendererColor(false, 0.0f, 0f, Enter, InputFieldFill);
        OnMouseEnterEvent.Invoke();
    }

    protected virtual void OnMouseExit()
    {
        if (!IsActive)
            return;
        
        IsMouseOver = false;
        
        if(IsReadyForInput)
            return;
        
        LJWConverter.Instance.GradientSpriteRendererColor(false, 0.0f, 0.2f, Exit, InputFieldFill);
        OnMouseExitEvent.Invoke();
    }

    public virtual void AddInputField(string value)
    {
        if (StringBuffer.Length >= MaxInputLength)
            return;

        if (!AddAbleCharSet.Contains(value))
            return;

        StringBuffer += AddSpaceBehindChar ? value + " " : value;
        InputFieldTMP.text = StringBuffer;
        SkipFlash = true;

        OnAddInputField.Invoke();
    }

    public virtual void DeleteInputField()
    {
        if (StringBuffer.Length == 0)
            return;

        var subLength = AddSpaceBehindChar ? 2 : 1;
        StringBuffer = StringBuffer.Substring(0, StringBuffer.Length - subLength);
        InputFieldTMP.text = StringBuffer;
        SkipFlash = true;

        OnDeleteInputField.Invoke();
    }

    public virtual void ReturnInputField()
    {
        OnReturnInputField.Invoke();
        
        InputFieldTMP.text = StringBuffer;
        CheckInputField();

        IsReadyForInput = false;
        IsFlash = false;
        BasicKeyboard.Instance.ConnectedInputField = null;
        
        LJWConverter.Instance.GradientSpriteRendererColor(false, 0.0f, 0.2f, Exit, InputFieldFill);
    }

    private IEnumerator StartFlashInputField()
    {
        if (StringBuffer.Length < MaxInputLength && IsReadyForInput && !SkipFlash)
        {
            if (IsFlash)
            {
                InputFieldTMP.text = StringBuffer;
                IsFlash = false;
            }
            else
            {
                InputFieldTMP.text = StringBuffer + "_";
                IsFlash = true;
            }
        }
        else if (!IsReadyForInput && StringBuffer != "")
        {
            InputFieldTMP.text = StringBuffer;
        }
        
        SkipFlash = false;

        yield return new WaitForSeconds(0.5f);
        StartCoroutine(StartFlashInputField());
        yield break;
    }

    public void CheckInputField()
    {
        if (StringBuffer == "")
            InputFieldTMP.text = DefaultText;
    }
    
}
