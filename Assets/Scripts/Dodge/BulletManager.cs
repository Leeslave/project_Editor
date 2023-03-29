using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class BulletManager : MonoBehaviour
{
    [SerializeField]
    public GameObject Bm;
    public GameObject Bl;
    static GameObject[] BM;
    static GameObject[] BL;
    static GameObject[] Pool;

    private void Awake()
    {
        BM = new GameObject[300];
        BL = new GameObject[50];
        for(int i = 0; i<BM.Length; i++)
        {
            BM[i] = Instantiate(Bm);
            BM[i].SetActive(false);
        }
        for(int i =0; i < BL.Length; i++)
        {
            BL[i] = Instantiate(Bl);
            BL[i].SetActive(false);
        }
    }

    public GameObject MakeSmallBul(Vector2 Dir1, Vector2 Dir2)
    {
        Pool = BM;
        for (int i = 0; i < Pool.Length; i++)
        {
            if (!Pool[i].activeSelf)
            {
                Pool[i].SetActive(true);
                Pool[i].GetComponent<Rigidbody2D>().AddForce(Dir1, ForceMode2D.Impulse);
                Pool[i].GetComponent<Rigidbody2D>().AddForce(Dir2, ForceMode2D.Impulse);
                return Pool[i];
            }
        }
        return null;
    }
    public GameObject MakeBigBul(Vector2 Dir1, Vector2 Dir2)
    {
        Pool = BL;
        for (int i = 0; i < Pool.Length; i++)
        {
            if (!Pool[i].activeSelf)
            {
                Pool[i].SetActive(true);
                Pool[i].GetComponent<Rigidbody2D>().AddForce(Dir1, ForceMode2D.Impulse);
                Pool[i].GetComponent<Rigidbody2D>().AddForce(Dir2, ForceMode2D.Impulse);
                Pool[i].GetComponent<BulletMove>().BigBullet(GetComponent<BulletManager>());
                return Pool[i];
            }
        }
        return null;
    }
    public void DelBul()
    {
        foreach(var a in BM)
        {
            a.SetActive(false);
        }
    }

    public void EndBul()
    {
        DelBul();
        gameObject.SetActive(false);
    }
}
