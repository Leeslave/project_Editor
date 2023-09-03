using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageChanger_Obs : MonoBehaviour
{
    [SerializeField] StageDB_Obs DB;
    [SerializeField] GameObject Border;
    [SerializeField] ShaderEffect_CRT Glitcher1;
    [SerializeField] ShaderEffect_Unsync Glitcher2;
    [SerializeField] Detect_Obs Detecter;    
    int CurStage = 0;
    int CurArea = 0;

    [Header("변경 시간")]
    [SerializeField] float ChangeTime;
    WaitForSeconds WFS;

    private void Awake()
    {
        WFS = new WaitForSeconds(ChangeTime*0.05f);
        
    }
    private void Start()
    {
        Detecter.CurStage = DB.Rects[0][0];
        Detecter.MaxX = DB.Rects[0][0].sizeDelta.x * 0.5f;
    }

    public void StageChanger(int Area, int Sub)
    {
        Detecter.CurStage = DB.Rects[Area][Sub];
        Detecter.MaxX = DB.Rects[Area][Sub].sizeDelta.x * 0.5f;
        StartCoroutine(StageChange(Area,Sub));
    }

    IEnumerator StageChange(int Area, int Sub)
    {
        Border.SetActive(true);
        Glitcher1.scanlineIntensity = 200;
        for(int i = 0; i < 20; i++)
        {
            Glitcher2.speed = Random.Range(0, 2);
            yield return WFS;
        }
        Glitcher1.scanlineIntensity = 0;
        Glitcher2.speed = 0.01f;
        Border.SetActive(false);
        DB.Rects[CurArea][CurStage].anchoredPosition = Vector3.zero;
        Detecter.transform.position = Vector3.zero;
        DB.Stages[CurArea][CurStage].StageOff();
        DB.Stages[Area][Sub].StageOn();
        CurArea = Area;
        CurStage = Sub;
    }

}
