using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class BasicButton : MonoBehaviour
{
    public TextMeshPro ButtonTMP { get; set; }
    public SpriteRenderer ButtonFrame { get; set; }
    private SpriteRenderer ButtonFill { get; set; }
    public BoxCollider2D ButtonCollider { get; set; }
    private bool IsMouseOver { get; set; } = false;
    public bool IsActive { get; set; } = true;

    public Color Enter = new Color(1f, 1f, 1f, 0.5f);
    public Color Exit = new Color(0.5f, 0.5f, 0.5f, 0.0f);
    public Color Down = new Color(0.5f, 0.5f, 0.5f, 0.5f);
    public Color Up = new Color(1f, 1f, 1f, 0.5f);

    public UnityEvent OnMouseDownEvent;
    public UnityEvent OnMouseUpEvent;
    public UnityEvent OnMouseEnterEvent;
    public UnityEvent OnMouseExitEvent;

    protected virtual void Awake()
    {
        ButtonTMP = this.transform.GetChild(0).GetComponent<TextMeshPro>();
        ButtonFrame = this.transform.GetChild(1).GetComponent<SpriteRenderer>();
        ButtonFill = this.transform.GetChild(2).GetComponent<SpriteRenderer>();
        ButtonCollider = this.transform.GetComponent<BoxCollider2D>();

        ButtonFill.color = Exit;
    }
    
    /// <summary>
    /// 버튼에 대한 접근성을 설정한다
    /// </summary>
    /// <param name="value"> 사용 가능 여부 </param>
    public void SetAvailability(bool value)
    {
        ButtonFill.color = Exit;
        IsActive = value;
        IsMouseOver = false;
    }

    /// <summary>
    /// wait 이후에 버튼에 대한 접근성을 차단하고, 차단 시점에서 duration 이후에 회복한다
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
    
    /// <summary>
    /// 마우스 Down
    /// </summary>
    protected virtual void OnMouseDown()
    {
        if (!IsActive)
            return;
        if (!IsMouseOver)
            return;
        
        LJWConverter.Instance.ConvertSpriteRendererColor(false, 0.0f, 0f, Down, ButtonFill);
        OnMouseDownEvent.Invoke();
    }

    /// <summary>
    /// 마우스 Up
    /// </summary>
    protected virtual void OnMouseUp()
    {
        if (!IsActive)
            return;
        if (!IsMouseOver)
            return;
        
        LJWConverter.Instance.ConvertSpriteRendererColor(false, 0.0f, 0.2f, Up, ButtonFill);
        OnMouseUpEvent.Invoke();
    }

    /// <summary>
    /// 마우스 Over
    /// </summary>
    protected virtual void OnMouseOver()
    {
        if (!IsActive)
            return;
        if (IsMouseOver)
            return;
        
        IsMouseOver = true;
        LJWConverter.Instance.ConvertSpriteRendererColor(false, 0.0f, 0f, Enter, ButtonFill);
        OnMouseEnterEvent.Invoke();
    }

    /// <summary>
    /// 마우스 Exit
    /// </summary>
    protected virtual void OnMouseExit()
    {
        if (!IsActive)
            return;
        
        IsMouseOver = false;
        LJWConverter.Instance.ConvertSpriteRendererColor(false, 0.0f, 0.2f, Exit, ButtonFill);
        OnMouseExitEvent.Invoke();
    }

}
