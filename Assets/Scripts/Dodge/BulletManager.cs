using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;


//Bullet Object Pooling
public class BulletManager : MonoBehaviour
{
    [SerializeField]
    public GameObject Bm;
    public GameObject Bl;
    static GameObject[] BM;
    static GameObject[] Pool;
    static Rigidbody2D[] rigids = new Rigidbody2D[300];

    private void Awake()
    {
        BM = new GameObject[300];
        for(int i = 0; i<BM.Length; i++)
        {
            BM[i] = Instantiate(Bm);
            rigids[i] = BM[i].GetComponent<Rigidbody2D>();
            BM[i].SetActive(false);
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
                rigids[i].AddForce(Dir1 + Dir2, ForceMode2D.Impulse);
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
