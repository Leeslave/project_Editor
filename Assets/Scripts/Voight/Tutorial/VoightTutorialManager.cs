using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class VoightTutorialManager : TutorialManager
{
    [SerializeField] private GameObject voightStart;
    [SerializeField] private GameObject voightEye;
    [SerializeField] private GameObject voightControl;
    [SerializeField] private GameObject voightAnswer;
    [SerializeField] private GameObject voightAll;
    [SerializeField] private GameObject voightEnd;

    private VK_ManagerScript Manager { get; set; }
    
    private bool _playingVoight;

    public bool IsVoightPlaying => _playingVoight;

    private void Awake() => Manager = FindObjectOfType<VK_ManagerScript>();

    public void StartVoightTutorial() => StartCoroutine(StartVoightTutorial_IE());    
    private IEnumerator StartVoightTutorial_IE()
    {
        //튜토리얼 시작
        _playingVoight = true;

        //페이즈 시작
        yield return ShowPopUp(voightStart);
        yield return UpEyeDisplay();
        yield return ShowPopUp(voightEye);
        yield return ShowPopUp(voightControl);
        yield return DownEyeDisplay();
        yield return UpAnswerScreen();
        yield return ShowPopUp(voightAnswer);
        yield return DownAnswerScreen();
        yield return UpAll();
        yield return ShowPopUp(voightAll);
        yield return DownAll();
        yield return ShowPopUp(voightEnd);
        
        //튜토리얼 종료
        blocker.SetActive(false);
        _playingVoight = false;
    }

    private IEnumerator UpEyeDisplay()
    {
        Manager.PlayMotor();
        LJWConverter.Instance.PositionTransform(false, 0.5f, 1.5f, new Vector3(0.2f, -5.4f, 0f), Manager.EyeDisplay.transform, Manager.curve);
        yield return new WaitForSeconds(3f);
    }
    private IEnumerator DownEyeDisplay()
    {
        Manager.PlayMotor();
        LJWConverter.Instance.PositionTransform(false, 0.5f, 1.5f, new Vector3(0.2f, -14.5f, 0f), Manager.EyeDisplay.transform, Manager.curve);
        yield return new WaitForSeconds(3f);
    }

    private IEnumerator UpAnswerScreen()
    {
        Manager.PlayMotor();
        Manager.SpawnArrows(5);
        LJWConverter.Instance.PositionTransform(false, 0.5f, 1.5f, new Vector3(3.4f, -7f, 0f), Manager.AnswerScreen.transform, Manager.curve);
        yield return new WaitForSeconds(3f);
    }
    private IEnumerator DownAnswerScreen()
    {
        Manager.PlayMotor();
        LJWConverter.Instance.PositionTransform(false, 0.5f, 1.5f, new Vector3(3.4f, -11f, 0f), Manager.AnswerScreen.transform, Manager.curve);
        yield return new WaitForSeconds(3f);
        Manager.DespawnArrows();
    }

    private IEnumerator UpAll()
    { 
        Manager.PlayMotor();
        Manager.SpawnArrows(5);
        LJWConverter.Instance.PositionTransform(false, 0.5f, 1.5f, new Vector3(0.2f, -5.4f, 0f), Manager.EyeDisplay.transform, Manager.curve);
        LJWConverter.Instance.PositionTransform(false, 0.5f, 1.5f, new Vector3(3.4f, -7f, 0f), Manager.AnswerScreen.transform, Manager.curve);
        yield return new WaitForSeconds(3f);
    }
    private IEnumerator DownAll()
    {
        Manager.PlayMotor();
        LJWConverter.Instance.PositionTransform(false, 0.5f, 1.5f, new Vector3(0.2f, -14.5f, 0f), Manager.EyeDisplay.transform, Manager.curve);
        LJWConverter.Instance.PositionTransform(false, 0.5f, 1.5f, new Vector3(3.4f, -11f, 0f), Manager.AnswerScreen.transform, Manager.curve);
        yield return new WaitForSeconds(3f);
        Manager.DespawnArrows();
    }
}
