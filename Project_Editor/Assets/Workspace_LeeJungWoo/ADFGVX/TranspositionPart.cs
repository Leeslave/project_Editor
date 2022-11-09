using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TranspositionPart : MonoBehaviour
{
    private ADFGVXscript ADFGVX;
    private bool selected;
    private bool isover;

    //Keyword�� Chiper�� Transpose�� ����ϴ� ó���� �迭
    private char[] array0;
    private char[] array1;

    //��ġ ��ȣ�� �� ����
    private string KeyWord;
    private int[] place;

    //�Է�â ������
    private bool Flash;

    //��ġ �۾�
    private bool onWork;
    private string[] interline;
    private int SubstringNum;
    private int x;
    private TextMeshPro[] lines;            //��ġ�� ��ȣ���� ȭ�鿡 ǥ�õǴ� �ؽ�Ʈ�޽����� �迭
    private string transposedChiper;        //intermediateChiper�� ���� ���� ������� �����صд�

    private void Awake()
    {
        ADFGVX = GameObject.Find("ADFGVX_Script").GetComponent<ADFGVXscript>();
        KeyWord = "";
        GetComponentsInChildren<TextMeshPro>()[0].text = KeyWord;
        selected = false;
        Flash = false;
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

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            if (isover && !selected)
                selected = true;
            else if (isover && selected)
                selected = true;
            else
                selected = false;

        }
    }

    private void OnMouseEnter()
    {
        isover = true;
    }

    private void OnMouseExit()
    {
        isover = false;
    }

    IEnumerator FlashText()
    {
        if (KeyWord.Length <= 16 && selected && !onWork)
        {
            if (Flash)
            {
                GetComponentsInChildren<TextMeshPro>()[0].text = KeyWord;
                Flash = false;
            }
            else
            {
                GetComponentsInChildren<TextMeshPro>()[0].text = KeyWord + " _";
                Flash = true;
            }
        }
        else if (KeyWord == "")
            GetComponentsInChildren<TextMeshPro>()[0].text = "Ŭ���Ͽ� �Է�...";
        else if(!selected)
            GetComponentsInChildren<TextMeshPro>()[0].text = KeyWord;

 
        yield return new WaitForSeconds(0.5f);
        StartCoroutine("FlashText", 0.5f);
    }

    private void ClearText()
    {
        GetComponentsInChildren<TextMeshPro>()[0].text = "";
    }

    public void AddText(string value)
    {
        if(!selected)
            return;

        if(KeyWord.Length>16)
        {
            ADFGVX.UpdateInfoBox("��ȣ Ű �ִ� �Է� ��Ȯ�� ���");
            ADFGVX.InformCurrentMode();
        }
        else
        {
            KeyWord = KeyWord + " " + value;
            GetComponentsInChildren<TextMeshPro>()[0].text = KeyWord;
            UpdatePriority();
        }
        
    }

    public void DeleteText()
    {
        if (!selected)
            return;

        if(KeyWord.Length<2)
        {
            ADFGVX.UpdateInfoBox("��ȣ�� ���� �Ұ� ��Ȯ�� ���");
            ADFGVX.InformCurrentMode();
        }
        else
        {
            KeyWord = KeyWord.Substring(0, KeyWord.Length - 2);
            GetComponentsInChildren<TextMeshPro>()[0].text = KeyWord;
        }
        
        UpdatePriority();
    }

    private void UpdatePriority()
    {
        string Priority = "";
        
        array0 = new char[KeyWord.Length];
        array1 = new char[KeyWord.Length / 2];
        place = new int[KeyWord.Length / 2];

        array0 = KeyWord.ToCharArray();

        for (int i = 0; i < KeyWord.Length; i++)
        {
            if (array0[i] != ' ')
                array1[i/2] = array0[i];
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
                    if (array1[i] > array1[j])
                        place[i]++;
                    if (array1[i] == array1[j] && i < j)
                        place[j]++;
                }
            }
        }

        for (int i = 0; i < KeyWord.Length / 2; i++)
        {
            Priority += " ";
            Priority += place[i].ToString();
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
        if(onWork==true)
        {
            ADFGVX.InfoBox.UpdateText("��ġ �Ұ�: �۾� ���� ��");
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
        SubstringNum = Chiper.Length / place.Length;

        for (int i = 0; i < place.Length; i++)
        {
            for (int j = 0; j < place.Length; j++)
            {
                if (place[j] == InputPriority)
                {
                    InputPriority++;
                    interline[j] = Chiper.Substring(0, SubstringNum);
                    Chiper = Chiper.Substring(SubstringNum);
                }
            }
        }

        printFlow();

        //��ġ ���� ����� ���ڿ��� ����
        Chiper = array1.ArrayToString();
        transposedChiper = "";
        for(int i=0;i<SubstringNum;i++)
        {
            for(int j=0;j<place.Length;j++)
            {
                transposedChiper += interline[j][i].ToString();
            }
        }
    }

    public void OnTransposeReverseDown()
    {
        //���� �߻�
        if (onWork == true)
        {
            ADFGVX.InfoBox.UpdateText("����ġ �Ұ�: �۾� ���� ��");
            return;
        }
        else if (KeyWord.Length == 0)
        {
            ADFGVX.InfoBox.UpdateText("����ġ �Ұ�: ��ȣ Ű ����");
            return;
        }

        string Chiper = ADFGVX.InterChiperBox.GetComponentInChildren<TextMeshPro>().text;
        int newlenght = 0;

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

        Chiper = array1.ArrayToString();
        SubstringNum = Chiper.Length / place.Length;
        Debug.Log(Chiper);

        for(int i=0;i<9;i++)
        {
            interline[i] = "";
            lines[i].text = "";
        }

        int length = Chiper.Length;
        for (int i=0;i<length;i++)
        {
            interline[i%place.Length] += Chiper.Substring(0, 1);
            Chiper = Chiper.Substring(1);
        }

        printFlow();
    }

    private void printFlow()//�帧 ���
    {
        //�帧 ��� ����
        x = 0;
        InvokeRepeating("printxf", 0.0f, 0.2f);
        //������ ����, ������ �ð� �Ŀ� ������ ȸ��
        GetComponentsInChildren<TextMeshPro>()[0].text = KeyWord;
        onWork = true;
        Invoke("SetonWorkFalse", 0.2f * (SubstringNum + place.Length));
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
        if (y >= SubstringNum)
            yield break;
        lines[x].text += interline[x][y].ToString();
        yield return new WaitForSeconds(0.2f);
        StartCoroutine(printyf(x, y+1));
    }

    private void SetonWorkFalse()
    {
        onWork = false;
    }

}