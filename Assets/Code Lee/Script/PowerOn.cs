using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PowerOn : MonoBehaviour
{
    public GameObject Logo;
    public GameObject LogoTop;
    public GameObject Info;
    public bool status;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);
        status = false;
    }


    public void ScreenPowerOn()
    {
        if (status == false)        // 스크린 off에서 on으로 전환할 때 여러번 누르는 것 방지.
        {
            status = true;
            gameObject.SetActive(true);
            Invoke("MainLogoOnOff", 1);
        }
    }
    void MainLogoOnOff()
    {
        if (Logo.activeSelf == false)
        {
            Logo.SetActive(true);
            Invoke("MainLogoOnOff", 1);
        }
        else
        {
            Logo.SetActive(false);
            Invoke("TopInfoOn", 0.5f);
        }
    }

    void TopInfoOn()
    {
        LogoTop.SetActive(true);
        Info.SetActive(true);
        Info.GetComponent<AnimOrderController>().realStart();
    }

    public void TopInfoOff()
    {
        LogoTop.SetActive(false);
        Info.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
