using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TranspositionPart : MonoBehaviour
{
    private ADFGVXscript ADFGVX;

    private string KeyWord;
    private char[] vs;
    private char[] vs1;
    private int[] place;

    private bool Flash;
    private string FlashKeyWord;
    private bool FlashBlock;

    //OnTransposeDown
    private char[] array0;
    private char[] array1;
    string[] interline;
    int InputCharNum;
    int x;

    private TextMeshPro[] lines;

    private void Awake()
    {
        ADFGVX = GameObject.Find("ADFGVX_Script").GetComponent<ADFGVXscript>();
        KeyWord = "";
        Flash = false;
        FlashBlock = false;
        StartCoroutine("FlashText", 0.5f);
        lines = new TextMeshPro[9];
        interline = new string[9];
        for (int i=5;i<5+lines.Length;i++)
        {
            lines[i-5] = GetComponentsInChildren<TextMeshPro>()[i];
            lines[i-5].text = "";
        }
        ClearPriority();
    }

    IEnumerator FlashText()
    {
        FlashKeyWord = KeyWord + "_ ";
        if (KeyWord.Length <= 16 && !FlashBlock)
        {
            if (Flash)
            {
                GetComponentsInChildren<TextMeshPro>()[0].text = KeyWord;
                Flash = false;
            }
            else
            {
                GetComponentsInChildren<TextMeshPro>()[0].text = FlashKeyWord;
                Flash = true;
            }
        }
        yield return new WaitForSeconds(0.5f);
        StartCoroutine("FlashText", 0.5f);
    }

    private void ClearText()
    {
        GetComponentsInChildren<TextMeshPro>()[0].text = "";
    }

    public void AddText(string value)
    {
        if(KeyWord.Length <= 16)
        {
            KeyWord = KeyWord + value + " ";
            GetComponentsInChildren<TextMeshPro>()[0].text = KeyWord;
            UpdatePriority();
        }
        else
        {
            ADFGVX.UpdateInfoBox("��ȣ Ű �߰� �Է� �Ұ� ��Ȯ�� ���");
            ADFGVX.InformCurrentMode();
        }
    }

    public void DeleteText()
    {
        if(KeyWord.Length >= 2)
        {
            KeyWord = KeyWord.Substring(0, KeyWord.Length - 2);
            GetComponentsInChildren<TextMeshPro>()[0].text = KeyWord;
        }
        else
        {
            ADFGVX.UpdateInfoBox("��ȣ�� ���� �Ұ� ��Ȯ�� ���");
            ADFGVX.InformCurrentMode();
        }
        UpdatePriority();
    }

    private void UpdatePriority()
    {
        string Priority = "";
        
        vs = new char[KeyWord.Length];
        vs1 = new char[KeyWord.Length / 2];
        place = new int[KeyWord.Length / 2];

        vs = KeyWord.ToCharArray();

        for (int i = 0; i < KeyWord.Length; i++)
        {
            if (vs[i] != ' ')
                vs1[i/2] = vs[i];
        }

        for(int i = 0; i < KeyWord.Length / 2; i++)
        {
            place[i] = 1;
        }

        for (int i = 0; i < KeyWord.Length / 2; i++)
        {
            for (int j = 0; j < KeyWord.Length / 2; j++)
            {
                if(i!=j)
                {
                    if (vs1[i] > vs1[j])
                        place[i]++;
                    if (vs1[i] == vs1[j] && i < j)
                        place[j]++;
                }
            }
        }

        for (int i = 0; i < KeyWord.Length / 2; i++)
        {
            Priority += place[i].ToString();
            Priority += " ";
        }

        GetComponentsInChildren<TextMeshPro>()[1].text = Priority;
    }

    private void ClearPriority()
    {
        GetComponentsInChildren<TextMeshPro>()[1].text = "";
    }

    public void OnTransposeDown()//��ġ ����
    {
        //���� �߻�
        if(FlashBlock==true)
        {
            ADFGVX.InfoBox.UpdateText("��ġ �Ұ�: ��ġ �۾� ���� ��");
            return;
        }
        else if(KeyWord.Length==0)
        {
            ADFGVX.InfoBox.UpdateText("��ġ �Ұ�: ��ȣ Ű ����");
            return;
        }

        string Chiper = GameObject.Find("EncodedChipertext").GetComponentsInChildren<TextMeshPro>()[1].text;
        int newlenght = 0;

        for(int i=0;i<lines.Length;i++)
        {
            lines[i].text = "";
        }

        array0 = new char[Chiper.Length];        
        array0 = Chiper.ToCharArray();
        
        //array0�� ����ִ� ���ĺ� ���� Ȯ��
        for (int i = 0; i < Chiper.Length; i++)
        {
            if (array0[i] >= 'A' && array0[i] <= 'Z')
                newlenght++;
        }
        
        //���ĺ� ���� ��ŭ �Ҵ�
        array1 = new char[newlenght];
        int idx = 0;
        
        //array01�� ���ĺ� ���� ����
        for (int i = 0; i < Chiper.Length; i++)
        {
            if (array0[i] >= 'A' && array0[i] <= 'Z')
            {
                array1[idx] = array0[i];
                idx++;
            }
        }

        //array01�� Chiper�� ���ڿ��� ����
        Chiper = array1.ArrayToString();
        int InputPriority = 1;
        InputCharNum = Chiper.Length / place.Length;

        for (int i = 0; i < place.Length; i++)
        {
            for (int j = 0; j < place.Length; j++)
            {
                if (place[j] == InputPriority)
                {
                    InputPriority++;
                    interline[j] = Chiper.Substring(0, InputCharNum);
                    Chiper = Chiper.Substring(InputCharNum);
                }
            }
        }

        //�帧 ��� ����
        x = 0;
        InvokeRepeating("printxf", 0.0f, 0.2f);

        //������ ����, ������ �ð� �Ŀ� ������ ȸ��
        GetComponentsInChildren<TextMeshPro>()[0].text = KeyWord;
        FlashBlock = true;
        Invoke("SetFlashBlockFalse", 0.2f * (InputCharNum + place.Length));
    }

    private void printxf()//�帧 ��� ������
    {
        if (x >= place.Length)
        {
            CancelInvoke("printxf");
            return;
        }
        StartCoroutine(printyf(x, 0));
        x++;
    }

    private IEnumerator printyf(int x, int y)//�帧 ��� �Ʒ���
    {
        if (y >= InputCharNum)
            yield break;
        lines[x].text += interline[x][y].ToString();
        yield return new WaitForSeconds(0.2f);
        StartCoroutine(printyf(x, y+1));
    }

    private void SetFlashBlockFalse()//������ Ȱ��
    {
        FlashBlock = false;
    }
}