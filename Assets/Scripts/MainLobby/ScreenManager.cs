using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenManager : MonoBehaviour
{
    /**
    * 스크린 관리 스크립트
    * - 스크린 전원 키고 끄기 (애니메이션)
    * - 업무 착수 및 관리
    */
    
    public GameObject bootPanel;
    public AnimationController bootAnimation;
    public float logoOnSeconds;
    private GameObject bootLogo;
    private GameObject bootCLI;
    private bool isBoot;


    private void Start() {
        bootLogo = bootPanel.transform.GetChild(1).gameObject;
        bootCLI = bootPanel.transform.GetChild(0).gameObject;
        isBoot = false;
        bootPanel.SetActive(true);
        bootLogo.SetActive(false);
        bootCLI.SetActive(false);
    }

    public void OnPowerClicked()
    {
        if (isBoot == false)
        {
            StartCoroutine("BootScreen");
        }
        else
        {
            // TODO: 전원 끄기
        }
    }

    IEnumerator BootScreen()
    {
        bootLogo.SetActive(true);
        yield return new WaitForSeconds(logoOnSeconds);
        bootLogo.SetActive(false);
        bootCLI.SetActive(true);
        bootAnimation.Play();

        // yield return bootAnimation.isFinished == true
        yield return new WaitUntil(() => bootAnimation.isFinished == true);
        yield return new WaitForSeconds(0.6f);

        bootPanel.SetActive(false);
        isBoot = true;
    }
}
