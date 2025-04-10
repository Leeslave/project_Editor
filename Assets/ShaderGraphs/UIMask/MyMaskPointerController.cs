using System;
using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class MyMaskPointerController : MonoBehaviour
{
    private RectTransform PopUpRect { get; set; }
    private Image Mask { get; set; }
    private Image Up { get; set; }
    private Image UpRight { get; set; }
    private Image Right { get; set; }
    private Image DownRight { get; set; }
    private Image Down { get; set; }
    private Image DownLeft { get; set; }
    private Image Left { get; set; }
    private Image UpLeft { get; set; }

    [SerializeField] private bool show;
    
    private void OnValidate()
    {
        PopUpRect = transform.parent.GetComponent<RectTransform>();
        Mask = transform.GetComponent<Image>();
        Up = transform.GetChild(0).GetComponent<Image>();
        UpRight = transform.GetChild(1).GetComponent<Image>();
        Right = transform.GetChild(2).GetComponent<Image>();
        DownRight = transform.GetChild(3).GetComponent<Image>();
        Down = transform.GetChild(4).GetComponent<Image>();
        DownLeft = transform.GetChild(5).GetComponent<Image>();
        Left = transform.GetChild(6).GetComponent<Image>();
        UpLeft = transform.GetChild(7).GetComponent<Image>();

        Mask.alphaHitTestMinimumThreshold = 0.2f;

        if (show)
        {
            Color red = new(0.5f, 0f, 0f, 0.5f);
            Up.color = red;
            UpRight.color = red;
            Right.color = red;
            DownRight.color = red;
            Down.color = red;
            DownLeft.color = red;
            Left.color = red;
            UpLeft.color = red;
        }
        else
        {
            Up.color = Color.clear;
            UpRight.color = Color.clear;
            Right.color = Color.clear;
            DownRight.color = Color.clear;
            Down.color = Color.clear;
            DownLeft.color = Color.clear;
            Left.color = Color.clear;
            UpLeft.color = Color.clear;
        }
    }

    private void Update()
    {
        RectTransform maskRect = Mask.rectTransform;
        float upHeight = (PopUpRect.rect.height / 2 - maskRect.anchoredPosition.y) - maskRect.rect.height / 2;
        float rightWidth = (PopUpRect.rect.width / 2 - maskRect.anchoredPosition.x) - maskRect.rect.width / 2;
        float downHeight = (PopUpRect.rect.height / 2 + maskRect.anchoredPosition.y) - maskRect.rect.height / 2;
        float leftWidth = (PopUpRect.rect.width / 2 + maskRect.anchoredPosition.x) - maskRect.rect.width / 2;
        
        SetSize(Up.rectTransform, new Vector2(maskRect.rect.width, upHeight) );
        SetSize(UpRight.rectTransform, new Vector2(rightWidth, upHeight));
        SetSize(Right.rectTransform, new Vector2(rightWidth, maskRect.rect.height));
        SetSize(DownRight.rectTransform, new Vector2(rightWidth, downHeight));
        SetSize(Down.rectTransform, new Vector2(maskRect.rect.width, downHeight));
        SetSize(DownLeft.rectTransform, new Vector2(leftWidth, downHeight));
        SetSize(Left.rectTransform, new Vector2(leftWidth, maskRect.rect.height));
        SetSize(UpLeft.rectTransform, new Vector2(leftWidth, upHeight));
    }

    private static void SetSize(RectTransform trans, Vector2 newSize) {
        Vector2 oldSize = trans.rect.size;
        Vector2 deltaSize = newSize - oldSize;
        trans.offsetMin -= new Vector2(deltaSize.x * trans.pivot.x, deltaSize.y * trans.pivot.y);
        trans.offsetMax += new Vector2(deltaSize.x * (1f - trans.pivot.x), deltaSize.y * (1f - trans.pivot.y));
    }
}
