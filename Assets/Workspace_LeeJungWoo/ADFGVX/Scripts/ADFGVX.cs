using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ADFGVX : MonoBehaviour
{
    public enum mode//ADFGVX���
    { Encoding,Decoding};
    public mode currentmode;//���� ADFGVX���

    [Header("��� ��ư")]
    public Button_ShiftMode shiftmode;

    [Header("���� ���� ǥ��")]
    public Info InfoBox;

    [Header("���� ���� ġȯ ��Ʈ")]
    public BiliteralSubstitutionPart biliteralsubstitutionpart;
    [Header("��ġ ��Ʈ")]
    public TranspositionPart transpositionpart;
    [Header("�߰� ��Ʈ")]
    public IntermediatePart intermediatepart;
    [Header("��ȣ ��Ʈ")]
    public ChiperPart chiperpart;
   

    private void Start()
    {
        UpdateInfoBox("ADFGVX ���̺��� �����Ͻʽÿ�.");
    }


    public void OnClearDown()//Clear ��ư�� ������ ��
    {
        UpdateInfoBox("�߰� ��ȣ ���� ���");
        InformCurrentMode();
    }

    public void OnDeleteDown()//Delete ��ư�� ������ ��
    {
    }

    public void OnModeDown()//��� ��ȯ ��ư�� ������ ��
    {
        chiperpart.ClearChiperAll();
        chiperpart.InitializeChiperAll();

        transpositionpart.ClearKeyWord();
        transpositionpart.ClearPriority();
        transpositionpart.ClearTransposition();

        intermediatepart.ClearIntermediateChiperAll();
        intermediatepart.InitializeIntermediateChiperAll();

        biliteralsubstitutionpart.InitializeText();
        
        if (currentmode == ADFGVX.mode.Encoding)
        {
            currentmode = ADFGVX.mode.Decoding;
            UpdateInfoBox("��� ��ȯ : ��ȣȭ");
        }
        else if (currentmode == ADFGVX.mode.Decoding)
        {
            currentmode = ADFGVX.mode.Encoding;
            UpdateInfoBox("��� ��ȯ : ��ȣȭ");
        }
        InformCurrentMode();
    }

    public void UpdateInfoBox(string Value)//InfoBox�� �ؽ�Ʈ�� Value�� �ٲ۴�
    {
        InfoBox.UpdateText(Value);
    }
    
    public void InformCurrentMode()//1�� �Ŀ� ���� ��� ���
    {
        if (currentmode == mode.Encoding)
            UpdateInfoBoxDelay(1, "��ȣȭ ���� ���� ��...");
        else if (currentmode == mode.Decoding)
            UpdateInfoBoxDelay(1, "��ȣȭ ���� ���� ��...");
    }
    
    private void UpdateInfoBoxDelay(float Time, string Value)//InfoBox�� �ؽ�Ʈ�� Timer�� �Ŀ� Value�� �ٲ۴�
    {
        StartCoroutine(UpdateInfoBoxTimer(Time, Value));        
    }
    
    private IEnumerator UpdateInfoBoxTimer(float Time, string Value)//UpdateInfoBoxDelay �ڷ�ƾ
    {
        float currenttime = 0.0f;
        while (currenttime < Time)
        {
            yield return new WaitForSeconds(0.01f);
            currenttime += 0.01f;
        }
        UpdateInfoBox(Value);
        yield return null;
    }
}
