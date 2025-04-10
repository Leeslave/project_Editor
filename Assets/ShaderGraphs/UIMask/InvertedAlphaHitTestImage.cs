using UnityEngine;
using UnityEngine.UI;

public class InvertedAlphaHitTestImage : Image
{
    [Range(0f, 1f)]
    public float invertedThreshold = 0.1f;

    public override bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera)
    {
        if (!raycastTarget || sprite == null || sprite.texture == null)
            return false;

        //스크린 좌표 → 로컬 좌표 → 텍스처 UV로 변환
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, screenPoint, eventCamera, out Vector2 localPoint);

        //텍스처 UV 좌표를 정규화 
        Rect rect = GetPixelAdjustedRect();
        float normalizedX = (localPoint.x - rect.x) / rect.width;
        float normalizedY = (localPoint.y - rect.y) / rect.height;

        //텍스처 좌표로 변환
        Texture2D tex = sprite.texture;
        int x = Mathf.Clamp(Mathf.RoundToInt(normalizedX * tex.width), 0, tex.width - 1);
        int y = Mathf.Clamp(Mathf.RoundToInt(normalizedY * tex.height), 0, tex.height - 1);

        try
        {
            //해당 텍스처 좌표 픽셀의 알파 값
            Color pixel = tex.GetPixel(x, y);
            
            //일반적인 alphaHitTestMinimumThreshold의 반전
            return pixel.a < invertedThreshold;
        }
        catch
        {
            return false;
        }
    }
}