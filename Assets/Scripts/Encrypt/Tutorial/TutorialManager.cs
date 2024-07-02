using System.Collections;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private GameObject blocker;
    [SerializeField] private TutorialPopUp decryptLoad;
    [SerializeField] private TutorialPopUp decryptTranspose;
    [SerializeField] private TutorialPopUp decryptSubstitution;

    private bool _playingDecrypt;
    private bool _playingEncrypt;
    private bool _waitingComplete;
    private float _shiftingTime;
    
    public void StartDecryptTutorial() => StartCoroutine(StartDecryptTutorial_IE());
    private IEnumerator StartDecryptTutorial_IE()
    {
        //튜토리얼 시작
        ADFGVXGameManager.Instance.decryptTargetText = "SONG-OF-YESTERDAY";
        ADFGVXGameManager.Instance.decryptResultText = "HELLO-MR-MY-YESTERDAY";
        _playingDecrypt = true;
        blocker.gameObject.SetActive(true);
        
        //페이즈 1
        decryptLoad.gameObject.SetActive(true);

        _waitingComplete = true;
        yield return new WaitForSecondsRealtime(0.6f);
        blocker.gameObject.SetActive(false);
        while (_waitingComplete) yield return new WaitForSecondsRealtime(0.1f);
        
        //페이즈 1 종료
        blocker.gameObject.SetActive(true);
        decryptLoad.gameObject.SetActive(false);
        yield return new WaitForSecondsRealtime(_shiftingTime);
        
        //페이즈 2
        decryptTranspose.gameObject.SetActive(true);
        
        _waitingComplete = true;
        yield return new WaitForSecondsRealtime(0.6f);
        blocker.gameObject.SetActive(false);
        while (_waitingComplete) yield return new WaitForSecondsRealtime(0.1f);

        //페이즈 2 종료
        blocker.gameObject.SetActive(true);
        decryptTranspose.gameObject.SetActive(false);
        yield return new WaitForSecondsRealtime(_shiftingTime);
        
        //페이즈 3
        decryptSubstitution.gameObject.SetActive(true);

        _waitingComplete = true;
        yield return new WaitForSecondsRealtime(0.6f);
        blocker.gameObject.SetActive(false);
        while (_waitingComplete) yield return new WaitForSecondsRealtime(0.1f);
        
        //페이즈 3 종료
        blocker.gameObject.SetActive(true);
        decryptSubstitution.gameObject.SetActive(false);
        yield return new WaitForSecondsRealtime(_shiftingTime);
        
        //튜토리얼 종료
        blocker.gameObject.SetActive(false);
        _playingDecrypt = false;
    }

    public bool IsDecryptPlaying() => _playingDecrypt;
    public bool IsEncryptPlaying() => _playingEncrypt;

    public void MoveToNextPhase(float wait)
    {
        _waitingComplete = false;
        _shiftingTime = wait;
    }
}
