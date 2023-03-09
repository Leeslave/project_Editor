using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PowerOn : MonoBehaviour
{
    public GameObject Logo;
    public GameObject LogoTop;
    public GameObject Info;
    public GameObject CMD;
    public bool status;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);
        status = false;
    }

    // 스크린 전원 키기
    public void ScreenPowerOn()
    {
        if (status == false)        // ��ũ�� off���� on���� ��ȯ�� �� ������ ������ �� ����.
        {
            status = true;
            gameObject.SetActive(true);
            Invoke("MainLogoOnOff", 1);
        }
    }
    // 스크린 전원 끄기
    public void ScreenPowerOff()
    {
        if (status == true)
        {
            CMD.SetActive(false);
            status = false;
        }
    }

    // 부팅 과정 메인 로고 활성화 (1초간 활성화)
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

    // 부팅 정보 출력 시작 (Animation 실행)
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
        Invoke("TurnOnCMD", 1.0f);
    }

    void TurnOnCMD()
    {
        CMD.GetComponent<CMDManager>().CMDStart();
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
