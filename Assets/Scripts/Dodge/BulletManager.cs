using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class BulletManager : MonoBehaviour
{
    [SerializeField]
    public GameObject BR;
    public GameObject BL;
    static GameObject[] BRL;
    static GameObject[] BLL;
    static GameObject[] Pool;

    private void Awake()
    {
        BLL = new GameObject[100];
        BRL = new GameObject[100];
        for(int i = 0; i<BRL.Length; i++)
        {
            BRL[i] = Instantiate(BR);
            BRL[i].SetActive(false);
        }
        for(int i = 0; i<BLL.Length; i++)
        {
            BLL[i] = Instantiate(BL);
            BLL[i].SetActive(false);
        }
    }

    public GameObject MakeBul(string Dir)
    {
       if(Dir == "L")
        {
            Pool = BLL;
        }
        else
        {
            Pool = BRL;
        }
        for (int i = 0; i < Pool.Length; i++)
        {
            if (!Pool[i].activeSelf)
            {
                Pool[i].SetActive(true);
                return Pool[i];
            }
        }
        return null;
    }
    public void DelBul()
    {
        for(int i = 0; i< 100; i++)
        {
            BLL[i].SetActive(false);
            BRL[i].SetActive(false);
        }
    }

    public void EndBul()
    {
        DelBul();
        gameObject.SetActive(false);
    }
}
