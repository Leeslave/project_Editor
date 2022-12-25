using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PowerOn : MonoBehaviour
{
    public GameObject Logo;
    public GameObject LogoTop;
    public GameObject Info;
    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);
        
    }


    public void ScreenPowerOn()
    {
        gameObject.SetActive(true);
        Debug.Log("Power On");
        Invoke("MainLogoOnOff", 1);
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

    // Update is called once per frame
    void Update()
    {
        
    }
}
