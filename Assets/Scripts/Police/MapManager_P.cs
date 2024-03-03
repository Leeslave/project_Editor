using UnityEngine.UI;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using TMPro;

public class MapManager_P : MonoBehaviour
{
    [Header("- �� ũ��� ������ 800 / 600")]
    [Header("- 4 : 3 ������ ���� �׸� �� Ȯ���� ��")]
    [SerializeField] public string MapName;
    [SerializeField] public string MinMap;
    [Header("- Ȯ���� ����")]
    [SerializeField] public int UpScaling;
    [Header("- �ش� Pixel�� ��\n   ���� ���� ���� 0.8�� ������ ��\n   0:�ε�,1:����,2:����(������),3:����(ȸ����)\n   4:Ⱦ��,5:������,6:������")]
    [SerializeField] Color[] WayColor;
    [Header("- ���ð��� ������ ��Ÿ�� ��")]
    [SerializeField] Color[] TraceColor;

    [Header("- Don't Touch -")]
    [SerializeField] Image PlatformImage;
    [SerializeField] RectTransform Platform;
    [SerializeField] GameObject TilePrefab;
    [SerializeField] GameObject Police1;
    [SerializeField] GameObject Police2;

    [Header("- Test -")]
    [SerializeField] string Date;
    
    string NormalPath;


    public List<List<Polices_P>> PoliceInOffice = new List<List<Polices_P>>();
    List<Tuple<int, int>> Office = new List<Tuple<int, int>>();
    int[,] MapInfo;
    int[,] OfficeInfo;
    int MapX;
    int MapY;

    private void Awake()
    {
        //print($"GameData\Police\\{MapName}.png");

        // Load Map Info
        PlatformImage.sprite = Resources.Load<Sprite>($"GameData/Police/{MapName}");
        Sprite image = Resources.Load<Sprite>($"GameData/Police/{MinMap}");
        Texture2D pixel = image.texture;
        Vector2 SizeCnt = new Vector2(UpScaling,UpScaling);
        MapInfo = new int[pixel.width,pixel.height];
        OfficeInfo = new int[pixel.width, pixel.height];
        MapX = pixel.width;
        MapY = pixel.height;
        for (int y = 0; y < pixel.height; y ++)
        {
            for (int x = 0; x < pixel.width; x ++)
            {
                Color j = pixel.GetPixel(x, y);
                if (j.a != 0.8f) continue;

                for (int i = 0; i < 7; i++) if (j.Equals(WayColor[i])) 
                    {
                        if (i == 6)
                        {
                            OfficeInfo[x, y] = Office.Count;
                            Office.Add(new Tuple<int, int>(x, y));
                            PoliceInOffice.Add(new List<Polices_P>());
                        }
                        MapInfo[x, y] = i+1; break; 
                    }
            }
        }

        // Load Police Info
        Polices PoliceCnt = JsonConvert.DeserializeObject<Polices>(File.ReadAllText($"{Directory.GetCurrentDirectory()}\\Assets\\Resources\\GameData\\Police\\{Date}.json"));
        int k = 0;
        foreach (var tmp in PoliceCnt.PoliceList)
        {
            GameObject cnt;
            if (tmp.IsCar) cnt = Instantiate(Police2, Platform);
            else cnt = Instantiate(Police1, Platform);
            Polices_P Inf = cnt.GetComponent<Polices_P>();
            Inf.Info = tmp;
            Inf.LineColor = TraceColor[k++];
            cnt.transform.position = TilePos(Office[tmp.Start].Item1, Office[tmp.Start].Item2);
            PoliceInOffice[tmp.Start].Add(Inf);
        }
    }

    public int TileType(int x, int y)
    {
        if (x >= MapX || x < 0 || y >= MapY || y < 0) return 0;
        else return MapInfo[x,y];
    }

    public Vector3 TilePos(int x, int y)
    {
        return new Vector3((x+0.5f) * UpScaling - 400,(y+0.5f) * UpScaling - 300,0);
    }

    public Polices_P GetPolice(int x, int y)
    {
        return PoliceInOffice[OfficeInfo[x, y]][0];
    }

    public Tuple<int,int> PosToInd(Vector3 Pos)
    {
        return new Tuple<int, int>((int)(Pos.x / UpScaling),(int)(Pos.y / UpScaling));
    }
}
