using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UpgradePattern : MonoBehaviour
{
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

    private void Start()
    {
        for(int i = 0; i < 26; i++)
        {
            SPT[i] = new Vector2(SPCNT.position.x + i, SPCNT.position.y);
            
            SPB[i] = new Vector2(SPCNT.position.x + i, SPCNT.position.y - 18);
            
        }
        for(int i = 0; i < 19; i++)
        {
            SPL[i] = new Vector2(SPCNT.position.x, SPCNT.position.y - i);
            SPR[i] = new Vector2(SPCNT.position.x + 25, SPCNT.position.y - i);
        }
        SP = new Vector2[][] { SPB, SPR, SPL, SPT };
        GM = GetComponent<GameManager>();
    }

    public IEnumerator Pattern1()
    {
        GM.Pl.ChangeType();
        yield return new WaitForSeconds(1);
        for (int i = 0; i < 20; i++)
        {
            int a = Random.Range(0,4);
            int b = Random.Range(1, SP[a].Length-1);
            GM.BM.MakeBigBul(DF[a] * 2, Vector2.zero).transform.position = SP[a][b];


            yield return new WaitForSeconds(3);
        }
    }

    public IEnumerator Pattern2()
    {
        int j = Random.Range(1, 3);

        yield return new WaitForSeconds(1);
        int dk = -1;
        int k = 6;
        bool jk = true;
        for (int i = 0; i < 100; i++)
        {
            for(int y = 3; y <= 15; y++)
            {
                if (!(y >= k && y <= k + 3)) GM.BM.MakeSmallBul(DF[j] * 8, Vector2.zero).transform.position = SP[j][y];
            }
            if (k == 6 || k == 11)
            {
                GameObject cnt = Instantiate(Plat);
                if (k == 6) cnt.transform.position = SP[j][k];
                else cnt.transform.position = SP[j][k + 3];
                cnt.GetComponent<Rigidbody2D>().AddForce(DF[j] * 8, ForceMode2D.Impulse);
                if(jk) dk *= -1;
                jk = !jk;
            }
            if (jk) k += dk;
            yield return new WaitForSeconds(GM.BulletInterv);
        }
    }
    public IEnumerator Pattern3()
    {
        GM.Pl.ChangeType();
        yield return new WaitForSeconds(1);
        for (int i = 0; i < 30; i++)
        {
            for (int x = 0; x < 9; x += 2) if (x != 4) GM.BM.MakeSmallBul(DE[x][0] * 5, DE[x][1] * 5).transform.position = new Vector2(SPT[12].x + 0.5f, SPL[9].y);
            for (int x = 0; x < SPT.Length / 2; x++) GM.BM.MakeSmallBul(Vector2.down * 5, Vector2.zero).transform.position = SPT[x];
            for (int x = 0; x < SPR.Length / 2; x++) GM.BM.MakeSmallBul(Vector2.left * 5, Vector2.zero).transform.position = SPR[x];
            for (int x = SPB.Length/2; x < SPB.Length; x++) GM.BM.MakeSmallBul(Vector2.up * 5, Vector2.zero).transform.position = SPB[x];
            for (int x = SPL.Length / 2; x < SPL.Length; x++) GM.BM.MakeSmallBul(Vector2.right * 5, Vector2.zero).transform.position = SPL[x];
            yield return new WaitForSeconds(1.5f);
        }
    }
}
