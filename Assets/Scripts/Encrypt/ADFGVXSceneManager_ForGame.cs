using UnityEngine;

public class ADFGVXSceneManager_ForGame : MonoBehaviour
{
    public LoadEncrypted_ForGame LoadEncrypted { get; set; }
    public Transpose_ForGame Transpose { get; set; }
    public BilateralSubstitute_ForGame BilateralSubstitute { get; set; }
    public DisplayDecrypted_ForGame DisplayDecrypted { get; set; }
    public WritePlain_ForGame WritePlain { get; set; }
    public DisplayEncrypted_ForGame DisplayEncrypted { get; set; }
    public ResultPanel_ForGame ResultPanel { get; set; }
    
    public enum SystemMode { Encryption, Decryption }
    public SystemMode CurrentSystemMode { get; set; } = SystemMode.Decryption;

    private void Awake()
    {
        LoadEncrypted = this.transform.GetChild(0).GetComponent<LoadEncrypted_ForGame>();
        Transpose = this.transform.GetChild(1).GetComponent<Transpose_ForGame>();
        DisplayDecrypted = this.transform.GetChild(2).GetComponent<DisplayDecrypted_ForGame>();
        BilateralSubstitute = this.transform.GetChild(3).GetComponent<BilateralSubstitute_ForGame>();
        WritePlain = this.transform.GetChild(4).GetComponent<WritePlain_ForGame>();
        DisplayEncrypted = this.transform.GetChild(5).GetComponent<DisplayEncrypted_ForGame>();
        ResultPanel = this.transform.GetChild(6).GetComponent<ResultPanel_ForGame>();
    }

    public void SwitchSystemMode()
    {
        switch(CurrentSystemMode)
        {
            case SystemMode.Decryption:
                LJWConverter.Instance.ConvertTransformPos(false, 0.0f, 0.5f, new Vector3(120f, 50f, 5f), DisplayDecrypted.transform);
                LJWConverter.Instance.ConvertTransformPos(false, 0.5f, 0.5f, new Vector3(-15f, 50f, 5f), WritePlain.transform);
                LJWConverter.Instance.ConvertTransformPos(false, 0.5f, 0.5f, new Vector3(120f, 0f, 5f), LoadEncrypted.transform);
                LJWConverter.Instance.ConvertTransformPos(false, 1.0f, 0.5f, new Vector3(-15f, 0f, 5f), DisplayEncrypted.transform);
                CutOffInputForWhile(0f, 1.5f);
                CurrentSystemMode = SystemMode.Encryption;
                break;
            case SystemMode.Encryption:
                LJWConverter.Instance.ConvertTransformPos(false, 0.5f, 0.5f, new Vector3(-15f, 50f, 5f), DisplayDecrypted.transform);
                LJWConverter.Instance.ConvertTransformPos(false, 0.0f, 0.5f, new Vector3(-15f, 100f, 5f), WritePlain.transform);
                LJWConverter.Instance.ConvertTransformPos(false, 1.0f, 0.5f, new Vector3(-15f, 0f, 5f), LoadEncrypted.transform);
                LJWConverter.Instance.ConvertTransformPos(false, 0.5f, 0.5f, new Vector3(-15f, -62f, 5f),DisplayEncrypted.transform);
                CutOffInputForWhile(0f, 1.5f);
                CurrentSystemMode = SystemMode.Decryption;
                break;
        }
    }

    public void CutOffInputForWhile(float wait, float duration)
    {
        StartCoroutine(LoadEncrypted.CutOffInputForWhile(wait, duration));
        StartCoroutine(Transpose.CutOffInputForWhile(wait, duration));
        StartCoroutine(DisplayDecrypted.CutOffInputForWhile(wait, duration));
        StartCoroutine(BilateralSubstitute.CutOffInputForWhile(wait, duration));
        StartCoroutine(WritePlain.CutOffInputForWhile(wait, duration));
        StartCoroutine(DisplayEncrypted.CutOffInputForWhile(wait, duration));
    }

    public void SetAvailable(bool value)
    {
        LoadEncrypted.SetAvailable(value);
        Transpose.SetAvailable(value);
        DisplayDecrypted.SetAvailable(value);
        BilateralSubstitute.SetAvailable(value);
        WritePlain.SetAvailable(value);
        DisplayEncrypted.SetAvailable(value);
    }
}
