using Newtonsoft.Json;
using System;
using UnityEngine;

public class ADFGVXGameManager : MonoBehaviour
{
    public static ADFGVXGameManager Instance { get; private set; }
    public static LoadEncrypted LoadEncrypted;
    public static KeyPriorityTranspose KeyPriorityTranspose;
    public static BilateralSubstitute BilateralSubstitute;
    public static DisplayDecrypted DisplayDecrypted;
    public static WritePlain WritePlain;
    public static DisplayEncrypted DisplayEncrypted;
    public static ResultPanel ResultPanel;
    public static CurrentModePanel CurrentModePanel;
    public static TutorialManager TutorialManager;
    
    public enum SystemMode { Encryption, Decryption }
    public static SystemMode CurrentSystemMode { get; private set; } = SystemMode.Decryption;
    
    [SerializeField] public string encryptTargetText;
    [SerializeField] public string encryptResultText;
    [SerializeField] public string decryptTargetText;
    [SerializeField] public string decryptResultText;
    [SerializeField] public bool encryptClear;
    [SerializeField] public bool decryptClear;
    
    private void Awake()
    {
        Instance = this;
        LoadEncrypted = FindObjectOfType<LoadEncrypted>();
        KeyPriorityTranspose = FindObjectOfType<KeyPriorityTranspose>();
        BilateralSubstitute = FindObjectOfType<BilateralSubstitute>();
        DisplayDecrypted = FindObjectOfType<DisplayDecrypted>();
        WritePlain = FindObjectOfType<WritePlain>();
        DisplayEncrypted = FindObjectOfType<DisplayEncrypted>();
        ResultPanel = FindObjectOfType<ResultPanel>();
        CurrentModePanel = FindObjectOfType<CurrentModePanel>();
        TutorialManager = FindObjectOfType<TutorialManager>();

        TryGetStageData();
    }

    private void TryGetStageData()
    {
        //스테이지 정보 로드
        TextAsset stageText = Resources.Load<TextAsset>("GameData/Encrypt/ADFGVXStageData"); 
        ADFGVXStageData stageData = JsonConvert.DeserializeObject<ADFGVXStageData>(stageText.text);
        int stageNum = GameSystem.Instance.GetTask("ADFGVX");
        Debug.Log(stageNum);
        if (stageData.Decrypt.TryGetValue(stageNum.ToString(), out var decryptData))
        {
            Debug.Log($"이번 날짜의 Decrypt Task[targetText:{decryptData["targetText"]}, decryptKey:{decryptData["decryptKey"]}, resultText:{decryptData["resultText"]}");
            decryptTargetText = decryptData["targetText"];
            decryptResultText = decryptData["resultText"];
        }
        else
        {
            Debug.Log("이번 날짜의 Decrypt Task는 없음!");
            decryptClear = true;
        }
            
        
        if (stageData.Encrypt.TryGetValue(stageNum.ToString(), out var encryptData))
        {
            Debug.Log($"이번 날짜의 Encrypt Task[targetText:{encryptData["targetText"]}, encryptKey:{encryptData["encryptKey"]}, resultText:{encryptData["resultText"]}");
            encryptTargetText = encryptData["targetText"];
            encryptResultText = encryptData["resultText"];
        }
        else
        {
            Debug.Log("이번 날짜의 Encrypt Task는 없음!"); 
            encryptClear = true;
        }
    }

    public static void SwitchSystemMode()
    {
        switch(CurrentSystemMode)
        {
            case SystemMode.Decryption:
                CurrentSystemMode = SystemMode.Encryption;
                LJWConverter.Instance.PositionTransform(false, 0.0f, 0.5f, new Vector3(120f, 50f, 5f), DisplayDecrypted.transform);
                LJWConverter.Instance.PositionTransform(false, 0.5f, 0.5f, new Vector3(-15f, 50f, 5f), WritePlain.transform);
                LJWConverter.Instance.PositionTransform(false, 0.5f, 0.5f, new Vector3(120f, 0f, 5f), LoadEncrypted.transform);
                LJWConverter.Instance.PositionTransform(false, 1.0f, 0.5f, new Vector3(-15f, 0f, 5f), DisplayEncrypted.transform);
                CutAvailabilityInputForWhile(0f, 1.5f);
                break;
            case SystemMode.Encryption:
                CurrentSystemMode = SystemMode.Decryption;
                LJWConverter.Instance.PositionTransform(false, 0.5f, 0.5f, new Vector3(-15f, 50f, 5f), DisplayDecrypted.transform);
                LJWConverter.Instance.PositionTransform(false, 0.0f, 0.5f, new Vector3(-15f, 100f, 5f), WritePlain.transform);
                LJWConverter.Instance.PositionTransform(false, 1.0f, 0.5f, new Vector3(-15f, 0f, 5f), LoadEncrypted.transform);
                LJWConverter.Instance.PositionTransform(false, 0.5f, 0.5f, new Vector3(-15f, -62f, 5f),DisplayEncrypted.transform);
                CutAvailabilityInputForWhile(0f, 1.5f);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public static void CutAvailabilityInputForWhile(float wait, float duration)
    {
        LoadEncrypted.CutAvailabilityInputForWhile(wait, duration);
        
        KeyPriorityTranspose.KeyInputField.CutAvailabilityForWhile(wait, duration);
        if(CurrentSystemMode != SystemMode.Decryption)
            KeyPriorityTranspose.ReverseTransposeLines.CutAvailabilityForWhile(wait, duration);
        else
            KeyPriorityTranspose.ReverseTransposeLines.SetAvailability(false);
        
        DisplayDecrypted.CutAvailabilityInputForWhile(wait, duration);
        BilateralSubstitute.CutAvailabilityInputForWhile(wait, duration);
        WritePlain.CutAvailabilityInputForWhile(wait, duration);
        DisplayEncrypted.CutAvailabilityInputForWhile(wait, duration);
        CurrentModePanel.CutAvailabilityInputForWhile(wait, duration);
    }

    public static void SetAvailable(bool value)
    {
        LoadEncrypted.SetAvailable(value);
        KeyPriorityTranspose.SetAvailable(value);
        DisplayDecrypted.SetAvailable(value);
        BilateralSubstitute.SetAvailable(value);
        WritePlain.SetAvailable(value);
        DisplayEncrypted.SetAvailable(value);
        CurrentModePanel.SetAvailable(value);
    }
}
