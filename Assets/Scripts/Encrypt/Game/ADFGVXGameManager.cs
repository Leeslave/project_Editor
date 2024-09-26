using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.IO;
using UnityEngine.Serialization;

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
    public static ADFGVXTutorialManager ADFGVXTutorialManager;
    
    public enum SystemMode { Encryption, Decryption }
    public static SystemMode CurrentSystemMode { get; private set; } = SystemMode.Decryption;
    
    [SerializeField] public string encryptTargetText;
    [SerializeField] public string encryptTransposeKey;
    [SerializeField] public string encryptTransposeTable;
    [SerializeField] public string encryptTransposeText;
    [SerializeField] public string encryptResultText;
    [SerializeField] public string encryptSaveTitle;
    
    [SerializeField] public string decryptTargetTitle;
    [SerializeField] public string decryptTargetText;
    [SerializeField] public string decryptTransposeKey;
    [SerializeField] public string decryptTransposeTable;
    [SerializeField] public string decryptTransposeText;
    [SerializeField] public string decryptResultText;
    [SerializeField] public string decryptSaveTitle;
    
    [SerializeField] public bool encryptClear;
    [SerializeField] public bool decryptClear;
    
    [SerializeField] private bool startTutorial;
    [SerializeField] private SystemMode tutorialMode = SystemMode.Decryption;
    
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
        ADFGVXTutorialManager = FindObjectOfType<ADFGVXTutorialManager>();
    }
    private void Start()
    {
        if (startTutorial)
        {
            if (tutorialMode == SystemMode.Decryption)
            {
                SetSystemMode(SystemMode.Decryption);
                ADFGVXTutorialManager.StartDecryptTutorial();
            }
            else
            {
                SetSystemMode(SystemMode.Encryption);
                ADFGVXTutorialManager.StartEncryptTutorial();   
            }
        }
        else
        {
            ADFGVXTutorialManager.blocker.gameObject.SetActive(false);
            TryGetStageData();
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha7))
            EncryptCheck();
        if(Input.GetKeyDown(KeyCode.Alpha8))
            DecryptCheck();
    }
    private void TryGetStageData()
    {
        //스테이지 정보 로드
        TextAsset stageText = Resources.Load<TextAsset>("GameData/Encrypt/ADFGVXStageData"); 
        ADFGVXStageData stageData = JsonConvert.DeserializeObject<ADFGVXStageData>(stageText.text);
        int stageNum = GameSystem.Instance.GetTask("ADFGVX");
        if (stageNum == -1)
        {
            Debug.LogError("스테이지 데이터 로드 실패!");
            return;
        }
        
        if (stageData.Decrypt.TryGetValue(stageNum.ToString(), out var decryptData))
        {
            Debug.Log($"이번 날짜의 Decrypt Task[targetText:{decryptData["targetText"]}, decryptKey:{decryptData["decryptKey"]}, resultText:{decryptData["resultText"]}");
            decryptTargetTitle = decryptData["targetText"];
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

    public static void ToggleSystemMode()
    {
        switch(CurrentSystemMode)
        {
            case SystemMode.Decryption:
                CurrentSystemMode = SystemMode.Encryption;
                LJWConverter.Instance.PositionTransform(false, 0.0f, 0.5f, new Vector3(70f, 50f, 5f), DisplayDecrypted.transform);
                LJWConverter.Instance.PositionTransform(false, 0.5f, 0.5f, new Vector3(-15f, 50f, 5f), WritePlain.transform);
                LJWConverter.Instance.PositionTransform(false, 0.5f, 0.5f, new Vector3(70f, 0f, 5f), LoadEncrypted.transform);
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
    private static void SetSystemMode(SystemMode target)
    {
        switch (target)
        {
            case SystemMode.Decryption:
                DisplayDecrypted.transform.localPosition = new Vector3(-15f, 50f, 5f);
                WritePlain.transform.localPosition = new Vector3(-15f, 100f, 5f);
                LoadEncrypted.transform.localPosition = new Vector3(-15f, 0f, 5f);
                DisplayEncrypted.transform.localPosition = new Vector3(-15f, -62f, 5f);
                CurrentSystemMode = SystemMode.Decryption;
                break;
            case SystemMode.Encryption:
                DisplayDecrypted.transform.localPosition = new Vector3(70f, 50f, 5f);
                WritePlain.transform.localPosition = new Vector3(-15f, 50f, 5f);
                LoadEncrypted.transform.localPosition = new Vector3(70f, 0f, 5f);
                DisplayEncrypted.transform.localPosition = new Vector3(-15f, 0f, 5f);
                CurrentSystemMode = SystemMode.Encryption;
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

    private void EncryptCheck()
    {
        //기초 데이터
        string targetText = encryptTargetText;
        string transposeKey = encryptTransposeKey;
        
        //키 순위 결정
        int[] keyPriority = new int[transposeKey.Length]; 
        for(int i = 0; i < transposeKey.Length; i++)
            keyPriority[i] = 1;
        for (int i = 0; i < transposeKey.Length; i++)
            for (int j = 0; j < transposeKey.Length; j++)
            {
                if (i == j) 
                    continue;
                if (transposeKey[i] > transposeKey[j])
                    keyPriority[i]++;
                if (transposeKey[i] == transposeKey[j] && i < j)
                    keyPriority[j]++;
            }

        //치환 테이블 로드
        string filePath = Application.dataPath + "/Resources/GameData/Encrypt/Tables/Table_" + encryptTransposeTable + ".txt";
        FileInfo txtFile = new(filePath);
        if (!txtFile.Exists)
            Debug.Log("테이블 로드에 문제 발생!");
        StreamReader reader = new(filePath);
        string substitutionTable = reader.ReadToEnd();
        reader.Close();

        //이중 문자 치환
        string transposeText = "";
        string[] adfgvx = { "A", "D", "F", "G", "V", "X" };
        for (int i = 0; i < targetText.Length; i++)
            for (int j = 0; j < substitutionTable.Length; j++)
                if (targetText[i] == substitutionTable[j])
                    transposeText += adfgvx[j / 6] + adfgvx[j % 6];
 
        //키 순위 전치
        string[] orderedText = new string[keyPriority.Length];
        for (int i = 0; i < orderedText.Length; i++)
            orderedText[i] = "";
        for (int i = 0; i < orderedText.Length; i++)
            for (int j = 0; j < transposeText.Length / keyPriority.Length; j++)
                orderedText[keyPriority[i] - 1] += transposeText[i + keyPriority.Length * j].ToString();
        string resultText = "";
        for (int i = 0; i < orderedText.Length; i++)
            resultText += orderedText[i];
        
        
        Debug.Log($"전치 결과: {transposeText.Aggregate("", (current, i) => current + i + " ")}");
        Debug.Log($"암호화 결과: {resultText.Aggregate("", (current, i) => current + i + " ")}");
    }
    private void DecryptCheck()
    {
        //기초 데이터
        string targetText = decryptTargetText;
        string transposeKey = decryptTransposeKey; 
        
        //키 순위 결정
        int[] keyPriority = new int[transposeKey.Length]; 
        for(int i = 0; i < transposeKey.Length; i++)
            keyPriority[i] = 1;
        for (int i = 0; i < transposeKey.Length; i++)
        for (int j = 0; j < transposeKey.Length; j++)
        {
            if (i == j) 
                continue;
            if (transposeKey[i] > transposeKey[j])
                keyPriority[i]++;
            if (transposeKey[i] == transposeKey[j] && i < j)
                keyPriority[j]++;
        }

        //키 순위 전치
        string[] orderedText = new string[keyPriority.Length];
        for (int i = 0; i < keyPriority.Length; i++)
            for (int j = 0; j < targetText.Length / keyPriority.Length; j++)
                orderedText[i] += targetText[(keyPriority[i] - 1) * (targetText.Length / keyPriority.Length) + j];
        string transposedText = "";
        for (int i = 0; i < targetText.Length / keyPriority.Length; i++)
            for (int j = 0; j < keyPriority.Length; j++)
                transposedText += orderedText[j][i];
        
        //치환 테이블 로드
        string filePath = Application.dataPath + "/Resources/GameData/Encrypt/Tables/Table_" + decryptTransposeTable + ".txt";
        FileInfo txtFile = new(filePath);
        if (!txtFile.Exists)
            Debug.Log("테이블 로드에 문제 발생!");
        StreamReader reader = new(filePath);
        string substitutionTable = reader.ReadToEnd();
        reader.Close();
        
        //이중 문자 치환
        Dictionary<char, int> dic = BilateralSubstitute.LineRowDecode;
        string resultText = "";
        for (int i = 0; i < targetText.Length / 2; i++)
        {
            int line = dic[transposedText[0]];
            int row = dic[transposedText[1]];
            resultText += substitutionTable[row + line * 6];
            transposedText = transposedText[2..];
        }
        
        Debug.Log($"전치 결과: {transposedText}");
        Debug.Log($"복호화 결과: {resultText}");
    }
}
