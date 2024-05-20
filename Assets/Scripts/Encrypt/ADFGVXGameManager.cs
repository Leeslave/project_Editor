using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Serialization;

public class ADFGVXGameManager : MonoBehaviour
{
    private ADFGVXGameManager GameManager { get; set; }
    public LoadEncrypted LoadEncrypted { get; private set; }
    public KeyPriorityTranspose KeyPriorityTranspose { get; private set; }
    public BilateralSubstitute BilateralSubstitute { get; private set; }
    public DisplayDecrypted DisplayDecrypted { get; private set; }
    public WritePlain WritePlain { get; private set; }
    public DisplayEncrypted DisplayEncrypted { get; private set; }
    public ResultPanel ResultPanel { get; private set; }
    public CurrentModePanel CurrentModePanel { get; set; }
    
    public enum SystemMode { Encryption, Decryption }
    public SystemMode CurrentSystemMode { get; set; } = SystemMode.Decryption;
    
    [SerializeField] public string encryptTargetText;
    [SerializeField] public string encryptResultText;
    [SerializeField] public string decryptTargetText;
    [SerializeField] public string decryptResultText;
    [SerializeField] public bool encryptClear;
    [SerializeField] public bool decryptClear;
    
    private void Awake()
    {
        LoadEncrypted = transform.GetChild(0).GetComponent<LoadEncrypted>();
        KeyPriorityTranspose = transform.GetChild(1).GetComponent<KeyPriorityTranspose>();
        DisplayDecrypted = transform.GetChild(2).GetComponent<DisplayDecrypted>();
        BilateralSubstitute = transform.GetChild(3).GetComponent<BilateralSubstitute>();
        WritePlain = transform.GetChild(4).GetComponent<WritePlain>();
        DisplayEncrypted = transform.GetChild(5).GetComponent<DisplayEncrypted>();
        ResultPanel = transform.GetChild(6).GetComponent<ResultPanel>();
        CurrentModePanel = transform.GetChild(7).GetComponent<CurrentModePanel>();
        GameManager = FindObjectOfType<ADFGVXGameManager>();

        TextAsset stageText = Resources.Load<TextAsset>("GameData/Encrypt/ADFGVXStageData"); 
        ADFGVXStageData stageData = JsonConvert.DeserializeObject<ADFGVXStageData>(stageText.text);
        int stageNum = GameSystem.Instance.GetTask("ADFGVX");
        //stageNum = 0;

        if (stageData.Decrypt.TryGetValue(stageNum.ToString(), out var decryptData))
        {
            Debug.Log($"이번 날짜의 Decrypt Task[targetText:{decryptData["targetText"]}, decryptKey:{decryptData["decryptKey"]}, resultText:{decryptData["resultText"]}");
            decryptTargetText = decryptData["targetText"];
            decryptResultText = decryptData["resultText"];
        }
        else
        {
            Debug.Log("이번 날짜의 Decrypt Task는 없음!");
            GameManager.decryptClear = true;
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
            GameManager.encryptClear = true;
        }
            
    }

    public void SwitchSystemMode()
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
        }
    }

    public void CutAvailabilityInputForWhile(float wait, float duration)
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

    public void SetAvailable(bool value)
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
