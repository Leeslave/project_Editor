using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
        StageChanger(0, 0);
    }

    public void StageChanger(int Area, int Sub)
    {
        StartCoroutine(StageChange(Area,Sub));
    }

    IEnumerator StageChange(int Area, int Sub)
    {
        Border.SetActive(true);

        var ChangeSprite = StageDB_Obs.DB.Stages[Area].Sprites[Sub];
        StageDB_Obs.DB.Stage.sprite = ChangeSprite;
        StageDB_Obs.DB.Stage.transform.position = Vector3.zero;
        
        float XRatio = ChangeSprite.bounds.size.x / ChangeSprite.bounds.size.y;
        Detecter.transform.position = Vector3.zero;
        Detecter.MaxX = 450 * XRatio;
        // Start Glitch
        Glitcher1.scanlineIntensity = 200;
        for(int i = 0; i < 20; i++)
        {
            Glitcher2.speed = Random.Range(0, 2);
            yield return WFS;
        }
        Glitcher1.scanlineIntensity = 0;
        Glitcher2.speed = 0.01f;
        Border.SetActive(false);
        //
        
        CurArea = Area;
        CurStage = Sub;
    }

}
