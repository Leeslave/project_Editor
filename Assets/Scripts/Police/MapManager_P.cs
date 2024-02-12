using UnityEngine.UI;
using UnityEngine;
using System.IO;
using System.Linq;

public class MapManager_P : MonoBehaviour
{
    [Header("- 맵 크기는 무조건 4 : 3")]
    [Header("또한 너비가 800의 약수일 것")]
    [SerializeField] public string MapName;
    
    [SerializeField] Color[] WayColor;

    [Header("- Don't Touch -")]
    [SerializeField] Image PlatformImage;
    [SerializeField] RectTransform Platform;
    [SerializeField] GameObject TilePrefab;

    string NormalPath;

    private void Awake()
    {
        NormalPath = Directory.GetCurrentDirectory() + "\\Assets\\Resources\\GameData\\Police\\";
        byte[] fileData = System.IO.File.ReadAllBytes(NormalPath + MapName + ".png");
        Texture2D image = new Texture2D(2, 2); image.LoadImage(fileData);
        PlatformImage.sprite = Sprite.Create(image, new Rect(0, 0, image.width, image.height), Vector2.one);

        float ratioX = Platform.sizeDelta.x / image.width;
        float ratioY = Platform.sizeDelta.y / image.height;
        Vector2 SizeCnt = new Vector2(ratioX, ratioY);


        for(int y = 0; y < image.height; y++) for(int x = 0; x < image.width; x++)
            {
                Color j = image.GetPixel(x, y);
                if (!WayColor.Contains(j)) continue;
                GameObject cnt = Instantiate(TilePrefab, Platform);
                cnt.GetComponent<Image>().color = image.GetPixel(x, y);
                RectTransform cntRect = cnt.GetComponent<RectTransform>();
                cntRect.anchoredPosition = new Vector3(ratioX * x, ratioY * y, 0);
                cntRect.sizeDelta = SizeCnt;
            }
                


        //Texture2D image = Resources.Load<Texture2D>(NormalPath + "\\" + MapName + ".png");

    }

}
