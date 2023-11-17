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
    /// 사용 가능 여부를 결정한다
    /// </summary>
    /// <param name="value"> 사용 가능 여부 </param>
    public void SetAvailable(bool value)
    {
        //초기화
        ButtonFill.color = Exit;
        //value에 따라서 버튼 사용 가능 여부가 결정된다
        IsActive = value;
        //항상 false로 설정해야 차단이 풀리는 순간 새롭게 Enter 이벤트가 발생한다
        IsMouseOver = false;
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
