using System.Collections;
using UnityEngine;

public class ADFGVXTutorialManager : TutorialManager
{
    [SerializeField] private GameObject decryptStart;
    [SerializeField] private GameObject decryptLoad;
    [SerializeField] private GameObject decryptTranspose;
    [SerializeField] private GameObject decryptTable;
    [SerializeField] private GameObject decryptSubstitution;

    [SerializeField] private GameObject encryptStart;
    [SerializeField] private GameObject encryptWrite;
    [SerializeField] private GameObject encryptTable;
    [SerializeField] private GameObject encryptKey;
    [SerializeField] private GameObject encryptTranspose;
    [SerializeField] private GameObject encryptResult;
    
    private bool _playingDecrypt;
    private bool _playingEncrypt;
    
    public bool IsDecryptPlaying() => _playingDecrypt;
    public bool IsEncryptPlaying() => _playingEncrypt;
    
    public void StartDecryptTutorial() => StartCoroutine(StartDecryptTutorial_IE());
    private IEnumerator StartDecryptTutorial_IE()
    {
        //튜토리얼 시작
        ADFGVXGameManager.Instance.decryptTargetTitle = "RENDEZVOUS-POINT";
        ADFGVXGameManager.Instance.decryptTargetText = "XFGVDGFGAAVFAAAGFVGGVFVGXAFXDAAF";
        ADFGVXGameManager.Instance.decryptTransposeKey = "WAIT";
        ADFGVXGameManager.Instance.decryptTransposeTable = "0";
        ADFGVXGameManager.Instance.decryptTransposeText = "XXAFAFAVFGVGXVFGDDAVAGAFAFAVFGGG";
        ADFGVXGameManager.Instance.decryptResultText = "MEET-ON-STREET-6";
        ADFGVXGameManager.Instance.decryptSaveTitle = "USER621-DECRYPT-QUALIFICATION";
        ADFGVXGameManager.BilateralSubstitute.SetTable(2);
        ADFGVXGameManager.DisplayDecrypted.DecryptedTextTitle.TextTMP.text = ADFGVXGameManager.Instance.decryptSaveTitle;
        _playingDecrypt = true;
        
        //페이즈
        yield return ShowPopUp(decryptStart);
        yield return ShowPopUp(decryptLoad);
        yield return ShowPopUp(decryptTranspose);
        yield return ShowPopUp(decryptTable);
        yield return ShowPopUp(decryptSubstitution);
        
        //튜토리얼 종료
        blocker.gameObject.SetActive(false);
        _playingDecrypt = false;
    }

    public void StartEncryptTutorial() => StartCoroutine(StartEncryptTutorial_IE());
    private IEnumerator StartEncryptTutorial_IE()
    {
        //튜토리얼 시작
        ADFGVXGameManager.Instance.encryptTargetText = "MEET-ON-STREET-6";
        ADFGVXGameManager.Instance.encryptTransposeKey = "WAIT";
        ADFGVXGameManager.Instance.encryptTransposeTable = "0";
        ADFGVXGameManager.Instance.encryptTransposeText = "XXAFAFAVFGVGXVFGDDAVAGAFAFAVFGGG";
        ADFGVXGameManager.Instance.encryptResultText = "XFGVDGFGAAVFAAAGFVGGVFVGXAFXDAAF";
        ADFGVXGameManager.Instance.encryptSaveTitle = "USER621-ENCRYPT-QUALIFICATION";
        ADFGVXGameManager.BilateralSubstitute.SetTable(2);
        ADFGVXGameManager.DisplayEncrypted.EncryptedTextTitle.TextTMP.text = ADFGVXGameManager.Instance.encryptSaveTitle;
        _playingEncrypt = true;

        //페이즈
        yield return ShowPopUp(encryptStart);
        yield return ShowPopUp(encryptWrite);
        yield return ShowPopUp(encryptTable);
        yield return ShowPopUp(encryptKey);
        yield return ShowPopUp(encryptTranspose);
        yield return ShowPopUp(encryptResult);
        
        //튜토리얼 종료
        blocker.gameObject.SetActive(false);
        _playingEncrypt = false;
    }
}
