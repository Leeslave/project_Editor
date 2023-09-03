using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageDB_Obs : MonoBehaviour
{
    public List<List<StageObject_Obs>> Stages;
    public List<List<RectTransform>> Rects;
    [SerializeField] List<Transform> Areas;
    [SerializeField] List<string> AreaConnected;
    [SerializeField] List<int> AreaDistance;
    string[,] Route;
    int[,] RouteCost;
    int RouteLen;


    private void Awake()
    {
        InitCalc();
        CalcRoute();
        int z;
        Stages = new List<List<StageObject_Obs>>(RouteLen);
        Rects = new List<List<RectTransform>>(RouteLen);
        for(int i = 0; i < RouteLen; i++)
        {
            z = Areas[i].childCount;
            Stages.Add(new List<StageObject_Obs>(z));
            Rects.Add(new List<RectTransform>(z));
            for (int x = 0; x < z; x++)
            {
                Stages[i].Add(Areas[i].GetChild(x).GetComponent<StageObject_Obs>());
                Rects[i].Add(Areas[i].GetChild(x).GetComponent<RectTransform>());
            }
        }
    }

    void InitCalc()
    {
        RouteLen = Areas.Count;
        Route = new string[RouteLen,RouteLen];
        RouteCost = new int[RouteLen, RouteLen];
        for (int i = 0; i < RouteLen; i++) for (int x = 0; x < RouteLen; x++) 
            { 
                Route[i, x] = "";
                if (i == x)
                    RouteCost[i, x] = 0;
                else
                    RouteCost[i, x] = 2147483646;
            }

        for(int i = 0; i < AreaConnected.Count; i++)
        {
            string z = AreaConnected[i].ToLower();
            int a = z[0] - 'a';
            int b = z[1] - 'a';
            Route[a, b] = $"{z[1]}";
            Route[b, a] = $"{z[0]}";
            RouteCost[a, b] = AreaDistance[i];
            RouteCost[b, a] = AreaDistance[i];
        }
    }

    void CalcRoute()
    {
        for(int m = 0; m < RouteLen; m++) for(int s = 0; s < RouteLen; s++) for(int g = 0; g < RouteLen; g++)
                {
                    if (Route[s,m] != "" && Route[m,g] != "" && s != g)
                    {
                        if (Route[s, g] == "" || RouteCost[s, m] + RouteCost[m, g] < RouteCost[s, g])
                        {
                            Route[s, g] = Route[s, m] + Route[m, g];
                            RouteCost[s, g] = RouteCost[s, m] + RouteCost[m, g];
                        }
                    }
                }
    }
}
