using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Random = UnityEngine.Random;

// Make Pattern / End Pattern
public class PatternManager : MonoBehaviour
{
    public GameManager GM;
    public BulletManager BM;

    public GameObject Plat;
    public GameObject Razer;
    public Transform SPCNT;
    public Transform[] SPRE;
    public Transform[] SPLE;

    public float RepeatInterv;      // Interval within the Pattern
    public float BulletInterv;      // Interval between Bullets
    public float PatternInterv;     // Interval between Patterns (When Pattern End)

    List<GameObject> PlatL = new List<GameObject>();
    int PatternNum = 0;             // Num of Pattern

    List<List<int[]>> PTLE = new List<List<int[]>>();

    // 좌우 소환 위치(히든용)
    Vector2[][] SP;
    Vector2[] SPT = new Vector2[26];
    Vector2[] SPB = new Vector2[26];
    Vector2[] SPL = new Vector2[19];
    Vector2[] SPR = new Vector2[19];

    // 4방
    Vector2[] DF =
    {
                      Vector2.up,
        Vector2.left,            Vector2.right,
                     Vector2.down
    };
    // 8방 + 중앙
    Vector2[][] DE =
    {
        new Vector2[] {Vector2.left,Vector2.up}, new Vector2[] {Vector2.zero,Vector2.up}, new Vector2[] { Vector2.right, Vector2.up },
        new Vector2[] {Vector2.left,Vector2.zero}, new Vector2[] {Vector2.zero,Vector2.zero}, new Vector2[] { Vector2.right, Vector2.zero },
        new Vector2[] {Vector2.left,Vector2.down}, new Vector2[] {Vector2.zero,Vector2.down}, new Vector2[] { Vector2.right, Vector2.down }
    };


    IEnumerator CurPlay = null;

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

    // EZ
    IEnumerator MakeEasyPattern()     // Make Normal Pattenr
    {
        if (GM.Timer.time == 0) yield return new WaitForSeconds(0.1f);
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
        yield break;
    }

    // Normal
    IEnumerator PatternN1()       // Play Time : 25s
    {
        GM.Pl.ChangeType("Ping");
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
        yield break;
    }

    IEnumerator PatternN2()
    {
        GM.Pl.ChangeType("Ping");
        yield return new WaitForSeconds(1);

        float MidY = (SPLE[4].position.y + SPLE[5].position.y) / 2;

        for (int i = 0; i < 3; i++)
        {
            int a1 = Random.Range(0, 2);
            if (a1 == 0) Instantiate(Razer).transform.position = new Vector3(2.6f, -4, 0);
            else Instantiate(Razer).transform.position = new Vector3(2.6f, 1.4f, 0);

        }
        yield break;
    }

    // Hard
    IEnumerator Pattern1()
    {
        GM.Pl.ChangeType("Normal");
        yield return new WaitForSeconds(1);

        for (int i = 0; i < 20; i++)
        {
            int a = Random.Range(0, 4);
            int b = Random.Range(1, SP[a].Length - 1);
            BM.MakeBigBul(DF[a] * 2, Vector2.zero, true).transform.position = SP[a][b];
            yield return new WaitForSeconds(3);
        }
        yield break;
    }

    IEnumerator Pattern2()
    {
        GM.Pl.ChangeType("Normal");
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
        yield break;
    }

    IEnumerator Pattern3()
    {
        GM.Pl.ChangeType("Normal");
        yield return new WaitForSeconds(1);

        Vector3 Cnt1 = new Vector3(SPT[12].x + 0.5f, SPL[9].y, 0);
        Vector3 Cnt2 = new Vector3(SPT[25].x, SPL[9].y, 0);
        Vector3 Cnt3 = new Vector3(SPT[0].x, SPL[9].y, 0);
        BM.MakeBigBul(Vector2.zero, Vector2.zero, false).transform.position = Cnt1;
        BM.MakeBigBul(Vector2.zero, Vector2.zero, false).transform.position = Cnt2;
        BM.MakeBigBul(Vector2.zero, Vector2.zero, false).transform.position = Cnt3;

        for (int i = 0; i < 50; i++)
        {
            BM.MakeSmallBul((GM.Pl.transform.position - Cnt1).normalized * 7, Vector2.zero).transform.position = Cnt1;
            BM.MakeSmallBul((GM.Pl.transform.position - Cnt2).normalized * 7, Vector2.zero).transform.position = Cnt2;
            BM.MakeSmallBul((GM.Pl.transform.position - Cnt3).normalized * 7, Vector2.zero).transform.position = Cnt3;
            yield return new WaitForSeconds(0.25f);
        }

        yield break;
    }

    IEnumerator Pattern4()
    {
        GM.Pl.ChangeType("Normal");
        GM.Pl.transform.GetChild(0).gameObject.SetActive(true);
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

        yield break;
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

    public void EndPT()     // End All Pattern
    {
        if (CurPlay != null) StopCoroutine(CurPlay);
        CurPlay = null;
        foreach (var a in PlatL) Destroy(a);
        GM.Pl.transform.GetChild(0).gameObject.SetActive(false);
        BM.EndBul();
    }

    void ReadExternalPattern()  // Read Pattern In Resources Folder. Name of The Pattern File Must be Pattern_X ( ex) Pattern_1, Pattern_2)
    {
        for (int i = 1; i < 4; i++)
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
