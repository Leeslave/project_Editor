using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

// Make Pattern / End Pattern
public class PatternManager : MonoBehaviour
{
    public BulletManager BM;
    public Camera MainCam;
    public Timer TM;
    public Player Pl;

    public GameObject PlatTop;
    public GameObject PlatBottom;

    public GameObject Plat;
    public GameObject Razer;
    public GameObject GameEnd;
    public Transform SPCNT;
    public Transform[] SPRE;
    public Transform[] SPLE;

    public float RepeatInterv;      // Interval within the Pattern
    public float BulletInterv;      // Interval between Bullets
    public float PatternInterv;     // Interval between Patterns (When Pattern End)

    List<GameObject> PlatL = new List<GameObject>();
    int PatternNum = 0;             // Num of Pattern

    List<List<int[]>> PTLE = new List<List<int[]>>();
    GameObject CurRazer = null;

    // 좌우 소환 위치(히든용)
    Vector2[][] SP;
    Vector2[] SPT = new Vector2[26];
    Vector2[] SPB = new Vector2[26];
    Vector2[] SPL = new Vector2[19];
    Vector2[] SPR = new Vector2[19];

    // 4방
    public Vector2[] DF =
    {
                      Vector2.up,
        Vector2.left,            Vector2.right,
                     Vector2.down
    };
    // 8방 + 중앙
    public Vector2[][] DE =
    {
        new Vector2[] {Vector2.left,Vector2.up}, new Vector2[] {Vector2.zero,Vector2.up}, new Vector2[] { Vector2.right, Vector2.up },
        new Vector2[] {Vector2.left,Vector2.zero}, new Vector2[] {Vector2.zero,Vector2.zero}, new Vector2[] { Vector2.right, Vector2.zero },
        new Vector2[] {Vector2.left,Vector2.down}, new Vector2[] {Vector2.zero,Vector2.down}, new Vector2[] { Vector2.right, Vector2.down }
    };

    private void Awake()
    {
        for (int i = 0; i < 26; i++)
        {
            SPT[i] = new Vector2(SPCNT.position.x + i, SPCNT.position.y);

            SPB[i] = new Vector2(SPCNT.position.x + i, SPCNT.position.y - 18);

        }
        for (int i = 0; i < 19; i++)
        {
            SPL[i] = new Vector2(SPCNT.position.x, SPCNT.position.y - i);
            SPR[i] = new Vector2(SPCNT.position.x + 25, SPCNT.position.y - i);
        }
        SP = new Vector2[][] { SPB, SPR, SPL, SPT };
        ReadExternalPattern();
    }

    private void Start()
    {
        StartPT(0);
    }

    // EZ
    IEnumerator MakeEasyPattern()     // Make Normal Pattenr
    {
        PlatTop.SetActive(true); PlatTop.SetActive(true);
        if (TM.time == 0) yield return new WaitForSeconds(0.2f);
        List<int[]> CurPattern = PTLE[Random.Range(0, PatternNum)];
        for (int CurRepeat = 0; CurRepeat < 2; CurRepeat++)
        {
            for (int x = 0; x < CurPattern[0].Length; x++)
            {
                for (int y = 0; y < CurPattern.Count; y++)
                {
                    if (CurPattern[y][x] == 1)
                    {
                        if (CurRepeat % 2 == 0) BM.MakeSmallBul(Vector2.left * 10, Vector2.zero).transform.position = SPRE[y].position;
                        else BM.MakeSmallBul(Vector2.right * 10, Vector2.zero).transform.position = SPLE[y].position;
                    }
                }
                yield return new WaitForSeconds(BulletInterv);
            }
            yield return new WaitForSeconds(RepeatInterv);
        }
        yield return new WaitForSeconds(PatternInterv);
        StartCoroutine(ChangePT(MakeEasyPattern()));
        yield break;
    }

    // Normal                   N1 -> N2 -> Hard
    IEnumerator PatternN1()       // Play Time : 25s
    {
        MakeGlitch(0.1f, 0.5f, 0.7f);
        yield return new WaitForSeconds(1);
        MakeGlitch(0, 0, 0);

        yield return new WaitForSeconds(1);

        int j = Random.Range(1, 3);
        int dk = -1;
        int k = 7;
        bool jk = true;
        for (int i = 0; i < 110; i++)
        {
            if (i == 30) for (int y = 6; y <= 12; y++) BM.MakeSmallBul(DF[2 / j] * 1.5f, Vector2.zero).transform.position = SP[2 / j][y];
            for (int y = 4; y <= 14; y++)
            {
                if (!(y >= k && y <= k + 3)) BM.MakeSmallBul(DF[j] * 6, Vector2.zero).transform.position = SP[j][y];
            }
            if (k == 7 || k == 10)
            {
                GameObject cnt = Instantiate(Plat);
                PlatL.Add(cnt);
                if (k == 7) cnt.transform.position = SP[j][k];
                else cnt.transform.position = SP[j][k + 3];
                cnt.GetComponent<Rigidbody2D>().AddForce(DF[j] * 6, ForceMode2D.Impulse);
                if (jk) dk *= -1;
                jk = !jk;
            }
            if (jk) k += dk;
            yield return new WaitForSeconds(BulletInterv * 1.4f);
        }
        yield return new WaitForSeconds(10);
        StartCoroutine(ChangePT(PatternN2()));
        yield break;
    }

    IEnumerator PatternN2()
    {

        MakeGlitch(0.1f, 0.5f, 0.7f);
        yield return new WaitForSeconds(1.5f);
        MakeGlitch(0, 0, 0);
        yield return new WaitForSeconds(1);

        float MidY = (SPLE[4].position.y + SPLE[5].position.y) / 2;

        for (int i = 0; i < 3; i++)
        {
            int a1 = Random.Range(0, 2);
            StartCoroutine(CamShake());
            CurRazer = Instantiate(Razer);
            GameObject cnt = Instantiate(Plat);
            cnt.GetComponent<Transform>().localScale = new Vector3(1.5f,0.1f);
            PlatL.Add(cnt);
            if (a1 == 0)
            {
                BM.MakeSmallBul(Vector2.left * 3, Vector2.zero).transform.position = SPRE[Random.Range(0, 5)].position;
                BM.MakeSmallBul(Vector2.left * 3, Vector2.zero).transform.position = SPRE[Random.Range(0,5)].position;
                cnt.transform.position = new Vector3(SPLE[4].position.x + 1,MidY,0);
                cnt.GetComponent<Rigidbody2D>().AddForce(Vector2.right * 3, ForceMode2D.Impulse);
                CurRazer.transform.position = new Vector3(0, -3, 0);
            }
            else
            {
                BM.MakeSmallBul(Vector2.right * 3, Vector2.zero).transform.position = SPLE[Random.Range(5, 10)].position;
                BM.MakeSmallBul(Vector2.right * 3, Vector2.zero).transform.position = SPLE[Random.Range(5, 10)].position;
                cnt.transform.position = new Vector3(SPRE[4].position.x - 1, MidY, 0);
                cnt.GetComponent<Rigidbody2D>().AddForce(Vector2.left * 3, ForceMode2D.Impulse);
                CurRazer.transform.position = new Vector3(0, 2.4f, 0);
            }
            yield return new WaitForSeconds(9);
        }
        MakeGlitch(0.3f, 0.7f, 1);
        yield return new WaitForSeconds(5);
        MakeGlitch(0, 0, 0);
        TM.IsTimeFlow = true;
        TM.MaxTime = 10000;
        StartCoroutine(ChangePT(Pattern1()));
        yield break;
    }
    IEnumerator CamShake()
    {
        float Cx = MainCam.transform.position.x;
        float Cy = MainCam.transform.position.y;
        yield return new WaitForSeconds(2f);
        for (int i = 0; i < 24; i++)
        {
            MainCam.transform.position = new Vector3(Cx - 0.5f, Cy, -2);
            yield return new WaitForSeconds(0.05f);
            MainCam.transform.position = new Vector3(Cx+0.5f, Cy, -2);
            yield return new WaitForSeconds(0.05f);
            MainCam.transform.position = new Vector3(Cx, Cy+0.5f, -2);
            yield return new WaitForSeconds(0.05f);
            MainCam.transform.position = new Vector3(Cx, Cy-0.5f, -2);
            yield return new WaitForSeconds(0.05f);
            MainCam.transform.position = new Vector3(Cx, Cy, -2);
            yield return new WaitForSeconds(0.05f);
        }
    }

    // Hard                     1 -> 2 -> 3 -> 4
    IEnumerator Pattern1()
    {
        PlatTop.SetActive(false); PlatTop.SetActive(false);
        Pl.ChangeType("Normal");
        yield return new WaitForSeconds(1);

        for (int i = 0; i < 20; i++)
        {
            int a = Random.Range(0, 4);
            int b = Random.Range(1, SP[a].Length - 1);
            BM.MakeBigBul(DF[a] * 2, Vector2.zero, true).transform.position = SP[a][b];
            yield return new WaitForSeconds(3);
        }
        StartCoroutine(ChangePT(Pattern2()));
        yield break;
    }

    IEnumerator Pattern2()
    {
        Pl.ChangeType("Normal");
        yield return new WaitForSeconds(1);

        for (int i = 0; i < 30; i++)
        {
            Debug.Log("3 Playing");
            for (int x = 0; x < 9; x += 2) if (x != 4) BM.MakeSmallBul(DE[x][0] * 5, DE[x][1] * 5).transform.position = new Vector2(SPT[12].x + 0.5f, SPL[9].y);
            for (int x = 0; x < SPT.Length / 2; x++) BM.MakeSmallBul(Vector2.down * 5, Vector2.zero).transform.position = SPT[x];
            for (int x = 0; x < SPR.Length / 2; x++) BM.MakeSmallBul(Vector2.left * 5, Vector2.zero).transform.position = SPR[x];
            for (int x = SPB.Length / 2; x < SPB.Length; x++) BM.MakeSmallBul(Vector2.up * 5, Vector2.zero).transform.position = SPB[x];
            for (int x = SPL.Length / 2; x < SPL.Length; x++) BM.MakeSmallBul(Vector2.right * 5, Vector2.zero).transform.position = SPL[x];
            yield return new WaitForSeconds(1.5f);
        }
        StartCoroutine(ChangePT(Pattern3()));
        yield break;
    }

    IEnumerator Pattern3()
    {
        Pl.ChangeType("Normal");
        yield return new WaitForSeconds(1);

        Vector3 Cnt1 = new Vector3(SPT[12].x + 0.5f, SPL[9].y, 0);
        Vector3 Cnt2 = new Vector3(SPT[25].x, SPL[9].y, 0);
        Vector3 Cnt3 = new Vector3(SPT[0].x, SPL[9].y, 0);
        BM.MakeBigBul(Vector2.zero, Vector2.zero, false).transform.position = Cnt1;
        BM.MakeBigBul(Vector2.zero, Vector2.zero, false).transform.position = Cnt2;
        BM.MakeBigBul(Vector2.zero, Vector2.zero, false).transform.position = Cnt3;

        for (int i = 0; i < 50; i++)
        {
            BM.MakeSmallBul((Pl.transform.position - Cnt1).normalized * 7, Vector2.zero).transform.position = Cnt1;
            BM.MakeSmallBul((Pl.transform.position - Cnt2).normalized * 7, Vector2.zero).transform.position = Cnt2;
            BM.MakeSmallBul((Pl.transform.position - Cnt3).normalized * 7, Vector2.zero).transform.position = Cnt3;
            yield return new WaitForSeconds(0.25f);
        }

        StartCoroutine(ChangePT(Pattern4()));
        yield break;
    }

    IEnumerator Pattern4()
    {
        Pl.ChangeType("Normal");
        Pl.transform.GetChild(0).gameObject.SetActive(true);
        yield return new WaitForSeconds(1);

        Vector3 Cnt1 = new Vector3(SPT[12].x + 0.5f, SPL[9].y, 0);

        for (int i = 0; i < 1000; i++)
        {
            int r2 = Random.Range(10, 20);
            float a1 = Random.Range(-1f, 1f); if (a1 == 0) a1 = 1;
            float a2 = Random.Range(-1f, 1f); if (a2 == 0) a2 = -1;
            Vector2 s = new Vector2(a1, a2);
            BM.MakeSmallBul(s * r2, Vector2.zero).transform.position = Cnt1;
            yield return new WaitForSeconds(0.01f);
        }
        yield return new WaitForSeconds(10);
        EndPT(false);
        Pl.gameObject.SetActive(false);
        Pl.EndG.SetActive(true);
        Pl.EndG.GetComponent<RealEnd>().Ending("Game Clear", "당신의 의지가 풍만해진다.");
    }

    public void EasyEndPattern() // End Of Easy Pattern(Not Hidden)
    {
        BM.DelBul();
        StopAllCoroutines();
        for (int i = 0; i < SPL.Length; i++)
        {
            BM.MakeSmallBul(Vector2.left * 2.5f, Vector2.zero).transform.position = SPRE[i].position;
            BM.MakeSmallBul(Vector2.right * 2.5f, Vector2.zero).transform.position = SPLE[i].position;
        }
    }
    public void MakeGlitch(float a, float b, float c)
    {
        GlitchEffect MC = MainCam.GetComponent<GlitchEffect>();
        MC.intensity = a; MC.flipIntensity = b; MC.colorIntensity = c;
    }

    IEnumerator ChangePT(IEnumerator a)
    {
        StartCoroutine(a);
        yield break;
    }

    public void StartPT(int i)
    {
        EndPT(false);
        switch (i) 
        {
            case 0: StartCoroutine(ChangePT(MakeEasyPattern())); break;
            case 1: StartCoroutine(ChangePT(PatternN1())); break;
        }
    }

    public void EndPT(bool IsEnd)     // End All Pattern
    {
        if (!IsEnd)
        {
            StopAllCoroutines();
            if (CurRazer != null) Destroy(CurRazer);
            foreach (var a in PlatL) Destroy(a);
            Pl.transform.GetChild(0).gameObject.SetActive(false);
            BM.DelBul();
        }
        else StopAllCoroutines();
    }

    void ReadExternalPattern()  // Read Pattern In Resources Folder. Name of The Pattern File Must be Pattern_X ( ex) Pattern_1, Pattern_2)
    {
        for (int i = 1; i < 5; i++)
        {
            string tmp = "Text/Dodge/Pattern_" + i.ToString();
            TextAsset textFile = Resources.Load(tmp) as TextAsset;
            if (textFile == null)
            {
                return;
            }
            StringReader stringReader = new StringReader(textFile.text);
            List<int[]> Data = new List<int[]>();

            while (stringReader != null)
            {
                string line = stringReader.ReadLine();
                if (line == null) break;
                int[] cnt = Array.ConvertAll(line.Split(' '), int.Parse);
                Data.Add(cnt);
            }
            PTLE.Add(Data);
            PatternNum += 1;
            stringReader.Close();
        }
    }
}
