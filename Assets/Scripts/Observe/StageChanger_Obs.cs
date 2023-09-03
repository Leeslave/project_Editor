using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class StageChanger_Obs : MonoBehaviour
{
    [SerializeField] StageObject_Obs[] Stages;
    [SerializeField] GameObject Border;
    [SerializeField] ShaderEffect_CRT Glitcher1;
    [SerializeField] ShaderEffect_Unsync Glitcher2;
    [SerializeField] Detect_Obs Detecter;
    RectTransform[] Rects;
    int CurStage = 0;

    [Header("변경 시간")]
    [SerializeField] float ChangeTime;
    WaitForSeconds WFS;

    private void Awake()
    {
        WFS = new WaitForSeconds(ChangeTime*0.05f);
        Rects = new RectTransform[Stages.Length];
        for (int i = 0; i < Rects.Length; i++) Rects[i] = Stages[i].GetComponent<RectTransform>();
        Detecter.CurStage = Rects[0];
        Detecter.MaxX = Rects[0].sizeDelta.x * 0.5f;
    }

    public void StageChanger(int ind)
    {
        Detecter.CurStage = Rects[ind];
        Detecter.MaxX = Rects[ind].sizeDelta.x * 0.5f;
        StartCoroutine(StageChange(ind));
    }

    IEnumerator StageChange(int ind)
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
        Rects[ind].anchoredPosition = Vector3.zero;
        Detecter.transform.position = Vector3.zero;
        Stages[CurStage].StageOff();
        Stages[ind].StageOn();
        CurStage = ind;
    }

}
