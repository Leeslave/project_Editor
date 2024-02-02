using UnityEngine;

public class ADFGVXGameManager : MonoBehaviour
{
    public LoadEncrypted LoadEncrypted { get; set; }
    public KeyPriorityTranspose KeyPriorityTranspose { get; set; }
    public BilateralSubstitute BilateralSubstitute { get; set; }
    public DisplayDecrypted DisplayDecrypted { get; set; }
    public WritePlain WritePlain { get; set; }
    public DisplayEncrypted DisplayEncrypted { get; set; }
    public ResultPanel ResultPanel { get; set; }
    
    public enum SystemMode { Encryption, Decryption }
    public SystemMode CurrentSystemMode { get; set; } = SystemMode.Decryption;

    private void Awake()
    {
        LoadEncrypted = this.transform.GetChild(0).GetComponent<LoadEncrypted>();
        KeyPriorityTranspose = this.transform.GetChild(1).GetComponent<KeyPriorityTranspose>();
        DisplayDecrypted = this.transform.GetChild(2).GetComponent<DisplayDecrypted>();
        BilateralSubstitute = this.transform.GetChild(3).GetComponent<BilateralSubstitute>();
        WritePlain = this.transform.GetChild(4).GetComponent<WritePlain>();
        DisplayEncrypted = this.transform.GetChild(5).GetComponent<DisplayEncrypted>();
        ResultPanel = this.transform.GetChild(6).GetComponent<ResultPanel>();
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
    }

    public void SetAvailable(bool value)
    {
        LoadEncrypted.SetAvailable(value);
        KeyPriorityTranspose.SetAvailable(value);
        DisplayDecrypted.SetAvailable(value);
        BilateralSubstitute.SetAvailable(value);
        WritePlain.SetAvailable(value);
        DisplayEncrypted.SetAvailable(value);
    }
}
