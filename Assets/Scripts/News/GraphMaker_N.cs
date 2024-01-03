using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GraphMaker_N : MonoBehaviour
{
    [SerializeField] Button Cap;
    [SerializeField] Image Img;
    RectTransform ImgRect;
    string root = "Assets\\Resources\\GameData\\News\\ss.png";
    public Camera mainCam;

    private void Start()
    {
        ImgRect = Img.GetComponent<RectTransform>();
        Cap.onClick.AddListener(Capture);
    }

    Vector3 SizeRatio = new Vector3(1,1,1);
    void Capture()
    {
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        SizeRatio.x = screenWidth / 1333;
        SizeRatio.y = screenHeight / 1000;
        StartCoroutine(CaptureAndSaveCoroutine());
    }
    IEnumerator CaptureAndSaveCoroutine()
    {
        // 대기 프레임이 필요할 수 있습니다.
        yield return new WaitForEndOfFrame();

        // 스크린샷 캡쳐
        Texture2D screenshot = ScreenCapture.CaptureScreenshotAsTexture();

        // 캡쳐된 영역을 추출
        Vector2 screenPos = Camera.main.WorldToScreenPoint(ImgRect.position);
        Vector3 Size = ImgRect.sizeDelta * SizeRatio;
        Rect rect = new Rect(screenPos.x - Size.x * 0.5f, screenPos.y - Size.y *0.5f, Size.x,Size.y);
        int x = Mathf.FloorToInt(rect.x);
        int y = Mathf.FloorToInt(rect.y);
        int width = Mathf.FloorToInt(rect.width);
        int height = Mathf.FloorToInt(rect.height);
        Texture2D croppedTexture = new Texture2D(width, height);
        croppedTexture.SetPixels(screenshot.GetPixels(x, y, width, height));
        croppedTexture.Apply();

        // 캡쳐된 영역을 파일로 저장
        byte[] bytes = croppedTexture.EncodeToPNG();
        System.IO.File.WriteAllBytes(root, bytes);
    }


}
