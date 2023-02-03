using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;

public class VerificationPanelPart : MonoBehaviour
{
    private ADFGVX adfgvx;

    private SpriteRenderer screenBlurSprite;
    private SpriteRenderer panelBackgroundSprite_U;
    private SpriteRenderer panelBackgroundSprite_D;
    private SpriteRenderer panelGuideSprite;

    private TextField title;
    private TextField result;
    private TextField percentageInfo;
    private TextField percentage;
    private Gauge loadingGauge;
    private Log consoleLog;

    private void Start()
    {
        adfgvx = GameObject.Find("GameManager").GetComponent<ADFGVX>();

        title = transform.Find("Title").GetComponent<TextField>();
        result = transform.Find("Result").GetComponent<TextField>();
        percentageInfo = transform.Find("PercentageInfo").GetComponent<TextField>();
        percentage = transform.Find("Percentage").GetComponent<TextField>();
        loadingGauge = transform.Find("LoadingGauge").GetComponent<Gauge>();
        consoleLog = transform.Find("ConsoleLog").GetComponent<Log>();

        screenBlurSprite = GetComponentsInChildren<SpriteRenderer>()[0];
        panelBackgroundSprite_U = GetComponentsInChildren<SpriteRenderer>()[1];
        panelBackgroundSprite_D = GetComponentsInChildren<SpriteRenderer>()[2];
        panelGuideSprite = GetComponentsInChildren<SpriteRenderer>()[3];

        UnvisiblePart();
    }

    public void SetLayer(int layer)//��� �Է� ����
    {
        this.gameObject.layer = layer;
        GameObject.Find("XButton").layer = layer;
    }

    public void VisiblePart()//Ȯ�� â ����
    {
        this.transform.localPosition = new Vector3(-63.6f, -45.7f, 0);
    }

    public void UnvisiblePart()//Ȯ�� â �񰡽�
    {
        this.transform.localPosition = new Vector3(330, -45.7f, 0);
    }

    public void StartDecodeVerification()//��ȣȭ Ȯ�� �����ϱ�
    {
        StartCoroutine(StartDecodeVerificationIEnumerator());
    }

    private IEnumerator StartDecodeVerificationIEnumerator()//��ȣȭ Ȯ�� �����ϱ�
    {
        if (adfgvx.encodeDataLoadPart.GetData() == "��ȣȭ �����͸� �ε��Ͽ� ���ۡ�")
        {
            adfgvx.InformError("��ȣȭ ������ �� ĭ : ��ȣȭ ������ ���� �Ұ�");
            yield break;
        }
        else if(adfgvx.afterDecodingPart.GetInputField_Data().GetInputString() == "")
        {
            adfgvx.InformError("��ȣȭ ������ �� ĭ : ��ȣȭ ������ ���� �Ұ�");
            yield break;
        }
        else if(adfgvx.transpositionpart.GetInputField_keyword().GetInputString() == "")
        {
            adfgvx.InformError("��ȣȭ Ű �� ĭ : ��ȣȭ ������ ���� �Ұ�");
            yield break;
        }

        //��� â ���� �ʱ�ȭ
        title.SetSizeTextOnly(new Vector2(1,1));
        title.SetColorText(Color.white);
        title.SetText("���� ��ȣȭ ������ ���� ��");

        //��� â ũ�� ����
        panelBackgroundSprite_D.size = new Vector2(231.8f, 46.3f);
        panelGuideSprite.size = new Vector2(57.9f, 11.7f);

        loadingGauge.VisibleGaugeImediately();
        
        percentage.SetColorText(Color.white);
        percentageInfo.SetColorText(Color.white);
        result.SetColorText(Color.clear);
        consoleLog.SetColorText(Color.white);

        //�÷��� �ð� ����
        float totalElaspedTime = adfgvx.GetTotalElapsedTime();

        //��� �Է� ����
        adfgvx.SetPartLayer(2, 2, 2, 2, 2, 2, 2, 2);

        //Ȯ�� �г� ���� ���
        VisiblePart();

        //������ ��
        loadingGauge.FillGaugeBar(3.0f, new Color(0.13f, 0.67f, 0.28f, 1));
        percentage.FillPercentage(3.0f);

        //�ε� �α�
        string filePath = adfgvx.ReturnDecodeScore() ? "Decode_Success" : "Decode_Fail";
        consoleLog.FillLoadingLog(3.0f, filePath);

        //�۾� ���� �� ���� ���� ���
        adfgvx.PlayAudioSource(ADFGVX.Audio.DataProcessing);

        //Ȯ�� â �۾� ���� ����
        yield return new WaitForSeconds(4f);
        title.ConvertSizeTextOnly(new Vector2(1.66f,1.66f), 1);
        percentage.ConvertColorTextOnly(1, Color.clear);
        percentageInfo.ConvertColorTextOnly(1, Color.clear);
        loadingGauge.UnvisibleGauge(1);
        consoleLog.HideTextOnly(1);
        
        //��� ���λ��� �帧 ��� �غ�
        string info;
        string keword;
        string time;
        if (adfgvx.ReturnDecodeScore())
        {
            title.SetText("���� ��ȣȭ ������ �Ϸ�");
            title.ConvertColorTextOnly(3f, new Color(0.1f, 0.35f, 0.85f, 1f));

            info = "�˸� : " + adfgvx.encodeDataLoadPart.GetTextField_SecurityLevel().GetText() + " '" + adfgvx.encodeDataLoadPart.GetInputField_filePath().GetInputString() + "'��\n";
            keword = "��ȣ Ű : " + EditStirng.CollectEnglishUpperAlphabet(adfgvx.transpositionpart.GetInputField_keyword().GetInputString()) + "�� ��ȣȭ ����\n";
        }
        else
        {
            title.SetText("���� ��ȣȭ ������ ����");
            title.ConvertColorTextOnly(3f, new Color(0.76f, 0.28f, 0.28f, 1f));

            info = "��� : " + adfgvx.encodeDataLoadPart.GetTextField_SecurityLevel().GetText() + " '" + adfgvx.encodeDataLoadPart.GetInputField_filePath().GetInputString() + "'��\n";
            keword = "��ȣ Ű : " + EditStirng.CollectEnglishUpperAlphabet(adfgvx.transpositionpart.GetInputField_keyword().GetInputString()) + "�� ��ȣȭ ����\n";
        }
        time = "�� �۾� �ð�: " + Mathf.FloorToInt(totalElaspedTime / 60).ToString("D2") + ":" + Mathf.FloorToInt(totalElaspedTime % 60).ToString("D2");

        ConvertSpriteSize(new Vector2(231.8f, 57.6f), panelBackgroundSprite_D, 1f);
        ConvertSpriteSize(new Vector2(57.9f, 14.6f), panelGuideSprite, 1f);

        //��� ���λ��� ���
        yield return new WaitForSeconds(1.5f);
        result.SetColorText(Color.white);
        result.SetText("");
        result.FlowText(info + keword + time, 3f);

        //�Ҹ� ���
        adfgvx.soundFlow(30, 3f);

        //Ȯ�� â �Է� ȸ��
        yield return new WaitForSeconds(4f);
        adfgvx.SetPartLayer(2, 2, 2, 2, 2, 2, 2, 0);
    }

    public void StartEncodeVerifiaction()//��ȣȭ Ȯ�� �����ϱ�
    {
        StartCoroutine(StartEncodeVerificationIEnumerator());
    }

    private IEnumerator StartEncodeVerificationIEnumerator()//��ȣȭ Ȯ�� �����ϱ�
    {
        if(adfgvx.encodeDataSavePart.GetInputField_Data().GetInputString() == "")
        {
            adfgvx.InformError("��ȣȭ ���� �� ĭ : ��ȣȭ ������ ���� �Ұ�");
            yield break;
        }
        if(adfgvx.encodeDataSavePart.GetInputField_Title().GetInputString() == "")
        {
            adfgvx.InformError("��ȣȭ ���� �� ĭ : ��ȣȭ ������ ���� �Ұ�");
            yield break;
        }
        if(adfgvx.transpositionpart.GetInputField_keyword().GetInputString() == "")
        {
            adfgvx.InformError("��ȣȭ Ű �� ĭ : ��ȣȭ ������ ���� �Ұ�");
            yield break;
        }

        //��� â ���� �ʱ�ȭ
        title.SetSizeTextOnly(new Vector2(1, 1));
        title.SetColorText(Color.white);
        title.SetText("���� ��ȣȭ ������ ���� ��");

        //��� â ũ�� ����
        panelBackgroundSprite_D.size = new Vector2(231.8f, 46.3f);
        panelGuideSprite.size = new Vector2(57.9f, 11.7f);

        //��� â ������ �ʱ�ȭ
        loadingGauge.VisibleGaugeImediately();

        percentage.SetColorText(Color.white);
        percentageInfo.SetColorText(Color.white);
        result.SetColorText(Color.clear);
        consoleLog.SetColorText(Color.white);

        //�÷��� �ð� ����
        float totalElaspedTime = adfgvx.GetTotalElapsedTime();

        //��� �Է� ����
        adfgvx.SetPartLayer(2, 2, 2, 2, 2, 2, 2, 2);

        //Ȯ�� �г� ���� ���
        VisiblePart();

        //������ ��
        loadingGauge.FillGaugeBar(3f, new Color(0.13f, 0.67f, 0.28f, 1));
        percentage.FillPercentage(3f);

        //�ε� �α�
        string filePath = true ? "Encode_Success" : "Encode_Fail";
        consoleLog.FillLoadingLog(3f, filePath);

        //�۾� ���� �� ���� ���� ���
        adfgvx.PlayAudioSource(ADFGVX.Audio.DataProcessing);

        //Ȯ�� â �۾� ���� ����
        yield return new WaitForSeconds(4f);
        title.ConvertSizeTextOnly(new Vector2(1.66f,1.66f), 1);
        percentage.ConvertColorTextOnly(1, Color.clear);
        percentageInfo.ConvertColorTextOnly(1, Color.clear);
        loadingGauge.UnvisibleGauge(1);
        consoleLog.HideTextOnly(1);

        //��� �ȳ� �帧 ���
        string info;
        string keyword;
        string time;
        if (adfgvx.ReturnEncodeScore())
        {
            title.SetText("���� ��ȣȭ ������ ����");
            title.ConvertColorTextOnly(3f, new Color(0.1f, 0.35f, 0.85f, 1f));

            info = "�˸� : " + adfgvx.encodeDataSavePart.GetSecurityLevel() + " '" + adfgvx.encodeDataSavePart.GetInputField_Title().GetInputString() + "'��\n";
            keyword = "��ȣ Ű : " + EditStirng.CollectEnglishUpperAlphabet(adfgvx.transpositionpart.GetInputField_keyword().GetInputString()) + "�� ��ȣȭ ����\n";
        }
        else
        {
            title.SetText("���� ��ȣȭ ������ ����");
            title.ConvertColorTextOnly(3f, new Color(0.76f, 0.28f, 0.28f, 1f));

            info = "��� : " + adfgvx.encodeDataSavePart.GetSecurityLevel() + " '" + adfgvx.encodeDataSavePart.GetInputField_Title().GetInputString() + "'��\n";
            keyword = "��ȣ Ű : " + EditStirng.CollectEnglishUpperAlphabet(adfgvx.transpositionpart.GetInputField_keyword().GetInputString()) + "�� ��ȣȭ ����\n";
        }

        time = "�� �۾� �ð�: " + Mathf.FloorToInt(totalElaspedTime / 60).ToString("D2") + ":" + Mathf.FloorToInt(totalElaspedTime % 60).ToString("D2");
        
        ConvertSpriteSize(new Vector2(231.8f, 57.6f), panelBackgroundSprite_D, 1f);
        ConvertSpriteSize(new Vector2(57.9f, 14.6f), panelGuideSprite, 1f);

        //��� ���λ��� ���
        yield return new WaitForSeconds(1.5f);
        result.SetText("");
        result.SetColorText(Color.white);
        result.FlowText(info + keyword + time, 3f);

        //�Ҹ� ���
        adfgvx.soundFlow(30, 3f);

        //Ȯ�� â �Է� ȸ��
        yield return new WaitForSeconds(4f);
        adfgvx.SetPartLayer(2, 2, 2, 2, 2, 2, 2, 0);
    }

    private void ConvertSpriteSize(Vector2 targetSize, SpriteRenderer target, float endTime)
    {
        StartCoroutine(ConvertSpriteSizeIEnumerator(target.size, target, targetSize, endTime, 0));
    }

    private IEnumerator ConvertSpriteSizeIEnumerator(Vector2 currentSize, SpriteRenderer target, Vector2 targetSize, float endTime, float currentTime)
    {
        currentTime += endTime / 100;
        if (currentTime >= endTime)
            yield break;

        float targetSizeX = currentSize.x + (targetSize.x - currentSize.x) * (currentTime / endTime);
        float targetSizeY = currentSize.y + (targetSize.y - currentSize.y) * (currentTime / endTime);
        target.size = new Vector2(targetSizeX, targetSizeY);

        yield return new WaitForSeconds(endTime / 100);
        StartCoroutine(ConvertSpriteSizeIEnumerator(target.size, target, targetSize, endTime, currentTime));
    }
}
