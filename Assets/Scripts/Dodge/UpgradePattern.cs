using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class UpgradePattern : MonoBehaviour
{
    public List<GameObject> PlatL = new List<GameObject>();
    public GameObject Plat;
    public Transform SPCNT;

    Vector2[][] SP;
    Vector2[] SPT = new Vector2[26];
    Vector2[] SPB = new Vector2[26];
    Vector2[] SPL = new Vector2[19];
    Vector2[] SPR = new Vector2[19];

    Vector2[] DF =
    {
                      Vector2.up,
        Vector2.left,            Vector2.right,
                     Vector2.down
    };

    Vector2[][] DE =
    {
        new Vector2[] {Vector2.left,Vector2.up}, new Vector2[] {Vector2.zero,Vector2.up}, new Vector2[] { Vector2.right, Vector2.up },
        new Vector2[] {Vector2.left,Vector2.zero}, new Vector2[] {Vector2.zero,Vector2.zero}, new Vector2[] { Vector2.right, Vector2.zero },
        new Vector2[] {Vector2.left,Vector2.down}, new Vector2[] {Vector2.zero,Vector2.down}, new Vector2[] { Vector2.right, Vector2.down }
    };

    GameManager GM;
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
        GM = GetComponent<GameManager>();
    }

    public IEnumerator Pattern1()       // Play Time : 25s
    {
        GM.Pl.ChangeType("Ping");
        yield return new WaitForSeconds(1);

        int j = Random.Range(1, 3);
        int dk = -1;
        int k = 7;
        bool jk = true;
        float tm = 0;

        
        for (int i = 0; i < 110; i++)
        {
            if(i == 30) for (int y = 6; y <= 12; y++) GM.BM.MakeSmallBul(DF[2 / j] * 1.5f, Vector2.zero).transform.position = SP[2 / j][y];
            for (int y = 4; y <= 14; y++)
            {
                if (!(y >= k && y <= k + 3)) GM.BM.MakeSmallBul(DF[j] * 6, Vector2.zero).transform.position = SP[j][y];
            }
            if (k == 7 || k == 10)
            {
                GameObject cnt = Instantiate(Plat);
                PlatL.Add(cnt);
                if (k == 7) cnt.transform.position = SP[j][k];
                else cnt.transform.position = SP[j][k + 3];
                cnt.GetComponent<Rigidbody2D>().AddForce(DF[j] * 6, ForceMode2D.Impulse);
                if(jk) dk *= -1;
                jk = !jk;
            }
            if (jk) k += dk;
            yield return new WaitForSeconds(GM.BulletInterv * 1.4f);
        }
        yield break;
    }

    public IEnumerator Pattern2()
    {
        Debug.Log("1 On");
        GM.Pl.ChangeType("Normal");
        yield return new WaitForSeconds(1);

        for (int i = 0; i < 20; i++)
        {
            Debug.Log("1 Playing");
            int a = Random.Range(0, 4);
            int b = Random.Range(1, SP[a].Length - 1);
            GM.BM.MakeBigBul(DF[a] * 2, Vector2.zero, true).transform.position = SP[a][b];
            yield return new WaitForSeconds(3);
        }
        yield break;
    }

    public IEnumerator Pattern3()
    {
        Debug.Log("3 On");
        GM.Pl.ChangeType("Normal");
        yield return new WaitForSeconds(1);

        for (int i = 0; i < 30; i++)
        {
            Debug.Log("3 Playing");
            for (int x = 0; x < 9; x += 2) if (x != 4) GM.BM.MakeSmallBul(DE[x][0] * 5, DE[x][1] * 5).transform.position = new Vector2(SPT[12].x + 0.5f, SPL[9].y);
            for (int x = 0; x < SPT.Length / 2; x++) GM.BM.MakeSmallBul(Vector2.down * 5, Vector2.zero).transform.position = SPT[x];
            for (int x = 0; x < SPR.Length / 2; x++) GM.BM.MakeSmallBul(Vector2.left * 5, Vector2.zero).transform.position = SPR[x];
            for (int x = SPB.Length/2; x < SPB.Length; x++) GM.BM.MakeSmallBul(Vector2.up * 5, Vector2.zero).transform.position = SPB[x];
            for (int x = SPL.Length / 2; x < SPL.Length; x++) GM.BM.MakeSmallBul(Vector2.right * 5, Vector2.zero).transform.position = SPL[x];
            yield return new WaitForSeconds(1.5f);
        }
        yield break;
    }

    public IEnumerator Pattern4()
    {
        Debug.Log("4 On");
        GM.Pl.ChangeType("Normal");
        yield return new WaitForSeconds(1);

        Vector3 Cnt1 = new Vector3(SPT[12].x + 0.5f, SPL[9].y, 0);
        Vector3 Cnt2 = new Vector3(SPT[25].x, SPL[9].y, 0);
        Vector3 Cnt3 = new Vector3(SPT[0].x, SPL[9].y, 0);
        GM.BM.MakeBigBul(Vector2.zero, Vector2.zero, false).transform.position = Cnt1;
        GM.BM.MakeBigBul(Vector2.zero, Vector2.zero, false).transform.position = Cnt2;
        GM.BM.MakeBigBul(Vector2.zero, Vector2.zero, false).transform.position = Cnt3;

        for (int i = 0; i < 50; i++)
        {
            GM.BM.MakeSmallBul((GM.Pl.transform.position - Cnt1).normalized * 7, Vector2.zero).transform.position = Cnt1;
            GM.BM.MakeSmallBul((GM.Pl.transform.position - Cnt2).normalized * 7, Vector2.zero).transform.position = Cnt2;
            GM.BM.MakeSmallBul((GM.Pl.transform.position - Cnt3).normalized * 7, Vector2.zero).transform.position = Cnt3;
            yield return new WaitForSeconds(0.25f);
        }

        yield break;
    }
    public IEnumerator Pattern5()
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
            Vector2 s = new Vector2(a1,a2);
            GM.BM.MakeSmallBul(s * r2, Vector2.zero).transform.position = Cnt1;
            yield return new WaitForSeconds(0.01f);
        }
        
        yield break;
    }


    public void RandPT()
    {
        int cnt = 4;
        switch (cnt) 
        {
            case 0: CurPlay = Pattern1(); break;
            case 1: CurPlay = Pattern2(); break;
            case 2: CurPlay = Pattern3(); break;
            case 3: CurPlay = Pattern4(); break;
            case 4: CurPlay = Pattern5(); break;
        }
        StartCoroutine(CurPlay);
    }
    public void EndPT()
    {
        if (CurPlay != null) StopCoroutine(CurPlay);
        CurPlay = null;
        foreach (var a in PlatL) Destroy(a);
        GM.Pl.transform.GetChild(0).gameObject.SetActive(false);
    }
}
