using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TutorialCoordUtil
{
    /// <summary>
    /// 월드 포인트를 카메라 pixelRect 기준 스크린 좌표로 변환
    /// </summary>
    public static Vector2 WorldToScreenInCameraRect(Camera cam, Vector3 worldPos, bool clampToRect = true)
    {
        Vector3 v = cam.WorldToViewportPoint(worldPos); // 월드 좌표를 카메라 내부 기준 좌표로 변환
        if (clampToRect) // 타겟이 화면 밖으로 나가는 경우를 위해 clamp를 이용
        {
            v.x = Mathf.Clamp01(v.x);
            v.y = Mathf.Clamp01(v.y);
        }

        Rect pr = cam.pixelRect; // 카메라가 실제로 렌더링 하는 스크린 픽셀 영역
        float sx = pr.xMin + v.x * pr.width;
        float sy = pr.yMin + v.y * pr.height;

        return new Vector2(sx,sy);
    }

    /// <summary>
    /// Overlay Canvas에서 ScreenPoint -> Canvas Local로 변환
    /// </summary>
    public static bool ScreenToCanvasLocal(RectTransform canvasRect, Vector2 screenPos, out Vector2 canvasLocal)
    {
        return RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPos, null, out canvasLocal); // 이때 Canvas가 World Space가 아니라 위에 그려지는 Overlay이기 때문에 매개변수에 null 이다.
    }

    /// <summary>
    /// 월드 포인트를 바로 Overlay Canvas anchoredPosition으로 변환
    /// </summary>
    public static bool WorldToOverlayCanvasLocal(Camera worldCam, RectTransform canvasRect, Vector3 worldPos, out Vector2 canvasLocal, bool clampToRect = true)
    {
        Vector2 sp = WorldToScreenInCameraRect(worldCam, worldPos, clampToRect);
        return ScreenToCanvasLocal(canvasRect, sp, out canvasLocal);
    }
}
