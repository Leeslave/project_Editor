using UnityEngine;
using UnityEngine.Serialization;

public class ADFGVXGameManager : MonoBehaviour
{
    public LoadEncrypted LoadEncrypted { get; private set; }
    public KeyPriorityTranspose KeyPriorityTranspose { get; private set; }
    public BilateralSubstitute BilateralSubstitute { get; private set; }
    public DisplayDecrypted DisplayDecrypted { get; private set; }
    public WritePlain WritePlain { get; private set; }
    public DisplayEncrypted DisplayEncrypted { get; private set; }
    public ResultPanel ResultPanel { get; private set; }
    public CurrentMode CurrentMode { get; set; }
    
    public enum SystemMode { Encryption, Decryption }
    public SystemMode CurrentSystemMode { get; set; } = SystemMode.Decryption;

    [SerializeField] public string encryptTargetData;

    private void Awake()
    {
        LoadEncrypted = transform.GetChild(0).GetComponent<LoadEncrypted>();
        KeyPriorityTranspose = transform.GetChild(1).GetComponent<KeyPriorityTranspose>();
        DisplayDecrypted = transform.GetChild(2).GetComponent<DisplayDecrypted>();
        BilateralSubstitute = transform.GetChild(3).GetComponent<BilateralSubstitute>();
        WritePlain = transform.GetChild(4).GetComponent<WritePlain>();
        DisplayEncrypted = transform.GetChild(5).GetComponent<DisplayEncrypted>();
        ResultPanel = transform.GetChild(6).GetComponent<ResultPanel>();
        CurrentMode = transform.GetChild(7).GetComponent<CurrentMode>();
    }

    public void SwitchSystemMode()
    {
        switch(CurrentSystemMode)
        {
            case SystemMode.Decryption:
                LJWConverter.Instance.PositionTransform(false, 0.0f, 0.5f, new Vector3(120f, 50f, 5f), DisplayDecrypted.transform);
                LJWConverter.Instance.PositionTransform(false, 0.5f, 0.5f, new Vector3(-15f, 50f, 5f), WritePlain.transform);
                LJWConverter.Instance.PositionTransform(false, 0.5f, 0.5f, new Vector3(120f, 0f, 5f), LoadEncrypted.transform);
                LJWConverter.Instance.PositionTransform(false, 1.0f, 0.5f, new Vector3(-15f, 0f, 5f), DisplayEncrypted.transform);
                CutAvailabilityInputForWhile(0f, 1.5f);
                CurrentSystemMode = SystemMode.Encryption;
                break;
            case SystemMode.Encryption:
                LJWConverter.Instance.PositionTransform(false, 0.5f, 0.5f, new Vector3(-15f, 50f, 5f), DisplayDecrypted.transform);
                LJWConverter.Instance.PositionTransform(false, 0.0f, 0.5f, new Vector3(-15f, 100f, 5f), WritePlain.transform);
                LJWConverter.Instance.PositionTransform(false, 1.0f, 0.5f, new Vector3(-15f, 0f, 5f), LoadEncrypted.transform);
                LJWConverter.Instance.PositionTransform(false, 0.5f, 0.5f, new Vector3(-15f, -62f, 5f),DisplayEncrypted.transform);
                CutAvailabilityInputForWhile(0f, 1.5f);
                CurrentSystemMode = SystemMode.Decryption;
                break;
        }
    }

    public void CutAvailabilityInputForWhile(float wait, float duration)
    {
        LoadEncrypted.CutAvailabilityInputForWhile(wait, duration);
        KeyPriorityTranspose.CutAvailabilityInputForWhile(wait, duration);
        DisplayDecrypted.CutAvailabilityInputForWhile(wait, duration);
        BilateralSubstitute.CutAvailabilityInputForWhile(wait, duration);
        WritePlain.CutAvailabilityInputForWhile(wait, duration);
        DisplayEncrypted.CutAvailabilityInputForWhile(wait, duration);
        CurrentMode.CutAvailabilityInputForWhile(wait, duration);
    }

    public void SetAvailable(bool value)
    {
        LoadEncrypted.SetAvailable(value);
        KeyPriorityTranspose.SetAvailable(value);
        DisplayDecrypted.SetAvailable(value);
        BilateralSubstitute.SetAvailable(value);
        WritePlain.SetAvailable(value);
        DisplayEncrypted.SetAvailable(value);
        CurrentMode.SetAvailable(value);
    }
}