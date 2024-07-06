using System.Collections;
using UnityEngine;

public class ADFGVXTutorialManager : TutorialManager
{
    [SerializeField] private UI_TargetTouchPopUp decryptStart;
    [SerializeField] private UI_TargetAreaPopUp decryptLoad;
    [SerializeField] private UI_TargetAreaPopUp decryptTranspose;
    [SerializeField] private UI_TargetAreaPopUp decryptSubstitution;
    
    private bool _playingDecrypt;
    private bool _playingEncrypt;
    
    public bool IsDecryptPlaying() => _playingDecrypt;
    public bool IsEncryptPlaying() => _playingEncrypt;
    
    public void StartDecryptTutorial() => StartCoroutine(StartDecryptTutorial_IE());
    private IEnumerator StartDecryptTutorial_IE()
    {
        //튜토리얼 시작
        ADFGVXGameManager.Instance.decryptTargetText = "SONG-OF-YESTERDAY";
        ADFGVXGameManager.Instance.decryptResultText = "HELLO-MR-MY-YESTERDAY";
        _playingDecrypt = true;
        
        //페이즈 0
        decryptStart.gameObject.SetActive(true);
        yield return WaitUntilActiveSelf(decryptStart.gameObject);
        
        //페이즈 1
        decryptLoad.gameObject.SetActive(true);
        yield return WaitUntilActiveSelf(decryptLoad.gameObject);
        
        //페이즈 2
        decryptTranspose.gameObject.SetActive(true);
        yield return WaitUntilActiveSelf(decryptTranspose.gameObject);
        
        //페이즈 3
        decryptSubstitution.gameObject.SetActive(true);
        yield return WaitUntilActiveSelf(decryptSubstitution.gameObject);
        
        //튜토리얼 종료
        ADFGVXGameManager.Blocker.gameObject.SetActive(false);
        _playingDecrypt = false;
    }

    // public void StartEncryptTutorial() => StartCoroutine(StartEncryptTutorial_IE());
    // private IEnumerator StartEncryptTutorial_IE()
    // {
    //     //튜토리얼 시작
    //     _playingEncrypt = true;
    //     blocker.gameObject.SetActive(true);
    //     
    //     //튜토리얼 종료
    //     blocker.gameObject.SetActive(false);
    //     _playingEncrypt = false;
    // }
}
