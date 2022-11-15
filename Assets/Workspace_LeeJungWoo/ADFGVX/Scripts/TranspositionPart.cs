using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TranspositionPart : MonoBehaviour
{
    private ADFGVX adfgvx;

    private TextMeshPro[] lines;
    private TextMeshPro keywordfieldtext;
    private TextMeshPro priority;
    private SpriteRenderer transposedlineguidebox;
    private SpriteRenderer keywordfieldcolor;

    private bool isflash;                               //������
    private bool skiponeflash;                          //true�� ������ �ѹ� �ǳʶڴ�

    private bool isonprintflow;                         //�帧 ��� �۾� ���ΰ�? �۾� ���̸� ��ġ ��ư�� ����ġ ��ư�� ���� �� ����
    public bool isreadyforinput;                        //Ű���� �Է� ������ �����ΰ�?
    private bool iscursoroverinputfield;                //�Է� �ʵ忡 Ŀ���� �ö� �ִ°�?

    //��ġ, ����ġ �� ����ϴ� ������
    private string keyword;                             //�Է� ���� Ű����
    private int[] place;                                //Ű���� ������ ����
    int rowlength;                                      //��ġ ����� �� ����
    int linelength;                                     //��ġ ����� �� ����
    private string[] templine;                          //�帧 ��� ��, ��� �����س��´�
    private int FlowLine;                               //�帧 ��� �ε���

    private void Awake()
    {
        adfgvx = GameObject.Find("ADFGVX").GetComponent<ADFGVX>();

        lines = new TextMeshPro[9];
        for (int i=0;i<9;i++)
        {
            lines[i] = GetComponentsInChildren<TextMeshPro>()[i];
            lines[i].text = "";
        }
        keywordfieldtext = GetComponentsInChildren<TextMeshPro>()[9];
        priority = GetComponentsInChildren<TextMeshPro>()[10];
        transposedlineguidebox = GetComponentsInChildren<SpriteRenderer>()[0];
        keywordfieldcolor = GetComponentsInChildren<SpriteRenderer>()[4];

        transposedlineguidebox.size = new Vector2(0, 0);
        keywordfieldcolor.color = new Color(0, 1, 0, 0);
        templine = new string[9];
        isreadyforinput = false;
        isflash = false;

        ClearKeyWord();
        ClearPriority();
        StartCoroutine("FlashText");
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            if (iscursoroverinputfield && !isreadyforinput)
            {
                isreadyforinput = true;
                keywordfieldcolor.color = new Color(0, 1, 0, 0);
                keywordfieldtext.text = keyword + " ��";
                isflash = true;
            }
            else if (iscursoroverinputfield && isreadyforinput)
                isreadyforinput = true;
            else
            {
                if (keyword == "")
                    keywordfieldtext.text = " Ŭ���Ͽ� �Է¡�";
                isreadyforinput = false;
                isflash = false;
            }
        }
    }

    private void OnMouseEnter()
    {
        if (!isreadyforinput)
            keywordfieldcolor.color = new Color(0, 1, 0, 0.15f);
        iscursoroverinputfield = true;
    }

    private void OnMouseExit()
    {
        keywordfieldcolor.color = new Color(0, 1, 0, 0);
        iscursoroverinputfield = false;
    }

    IEnumerator FlashText()//Ű���� â�� �����̰� �����
    {
        if (keyword.Length <= 16 && isreadyforinput && !isonprintflow && !skiponeflash)
        {
            if (isflash)
            {
                keywordfieldtext.text = keyword;
                isflash = false;
            }
            else
            {
                keywordfieldtext.text = keyword + " ��";
                isflash = true;
            }
        }
        else if (!isreadyforinput && keyword == "")
            keywordfieldtext.text = " Ŭ���Ͽ� �Է¡�";
        else if(!isreadyforinput && keyword != "")
            keywordfieldtext.text = keyword;

        skiponeflash = !skiponeflash ? false : false;                               //�̹� �Ͽ� ��ŵ������ ���� ������ ���ڿ��� �Ѵ�
        yield return new WaitForSeconds(0.5f);
        StartCoroutine("FlashText", 0.5f);
    }

    private void DelayFlashInputField()//�������� 0.5�� ���´�
    {
        skiponeflash = true;
    }

    public void AddKeyWord(string value)//Ű���忡 �� �ܾ� �߰��Ѵ�
    {
        if(!isreadyforinput)
            return;
        else if(keyword.Length>16)
        {
            adfgvx.UpdateInfoBox("��ȣ Ű �ִ� �Է� ��Ȯ�� ���");
            adfgvx.InformCurrentMode();
            return;
        }

        DelayFlashInputField();
        keyword = keyword + " " + value;
        keywordfieldtext.text = keyword;
        UpdatePriority();
    }

    public void DeleteKeyWord()//Ű���忡�� �� �ܾ� �����
    {
        if (!isreadyforinput)
            return;
        else if(keyword.Length<2)
        {
            adfgvx.UpdateInfoBox("��ȣ�� ���� �Ұ� ��Ȯ�� ���");
            adfgvx.InformCurrentMode();
            return;
        }

        DelayFlashInputField();
        keyword = keyword.Substring(0, keyword.Length - 2);
        keywordfieldtext.text = keyword;
        UpdatePriority();
    }

    public void ClearKeyWord()//Ű���带 ����
    {
        keyword = "";
        keywordfieldtext.text = keyword;
    }

    private void UpdatePriority()//Priority�� ���� ���Ѵ�
    {
        string Priority = "";
        string value = CollectEnglishAlphabet(keyword);
        place = new int[value.Length];

        for(int i = 0; i < value.Length; i++)
        {
            place[i] = 1;
        }

        for (int i = 0; i < value.Length; i++)
        {
            for (int j = 0; j < value.Length; j++)
            {
                if(i!=j)
                {
                    if (value[i] > value[j])
                        place[i]++;
                    if (value[i] == value[j] && i < j)
                        place[j]++;
                }
            }
        }

        for (int i = 0; i < keyword.Length / 2; i++)
        {
            Priority += " ";
            Priority += place[i].ToString();
        }

        ClearTransposition();

        float pos_x = transform.GetChild(0).localPosition.x + (2.5f * (place.Length - 1));
        float size_x = 2.5f * place.Length;
        float size_y = 2.5f;
        GetComponentsInChildren<SpriteRenderer>()[0].transform.localPosition = new Vector3(pos_x, -4, 0);
        GetComponentsInChildren<SpriteRenderer>()[0].size = new Vector2(size_x, size_y);
        
        priority.text = Priority;
    }

    public void ClearPriority()//Priority�� ����
    {
        priority.text = "";
    }

    private string CollectEnglishAlphabet(string value)//��ĭ, ���� ���� �����ϰ� ���� ���ĺ��� ��Ƽ� ��ȯ�Ѵ�
    {
        //array0�� ����ִ� ���ĺ� ���� Ȯ��, ���Ӱ� ������� array1�� ���� Ȯ��
        int newarraylenght = 0;
        for (int i = 0; i < value.Length; i++)
        {
            if (value[i] >= 'A' && value[i] <= 'Z' || value[i] == '_')
                newarraylenght++;
        }

        //array0�� ���ĺ� ���� ��ŭ array �Ҵ�
        char[] array = new char[newarraylenght];
        int idx = 0;

        //array01�� idx�� �÷����鼭 ���ĺ� ���� ����
        for (int i = 0; i < value.Length; i++)
        {
            if (value[i] >= 'A' && value[i] <= 'Z' || value[i] == '_')
            {
                array[idx] = value[i];
                idx++;
            }
        }
        return array.ArrayToString();
    }

    public void OnTransposeDown()//EncodedChiper�� ���ͼ� Transposition�� ä�� �ִ´�
    {
        if (!isreadyforinput)
            return;
        //���� �߻�
        if (isonprintflow == true)
        {
            adfgvx.InfoBox.UpdateText("��ġ �Ұ�: �۾� ���� ��");
            return;
        }
        else if (keyword.Length == 0)
        {
            adfgvx.InfoBox.UpdateText("��ġ �Ұ�: ��ȣ Ű ����");
            return;
        }
        else if(adfgvx.chiperpart.GetChiperText()=="")
        {
            adfgvx.InfoBox.UpdateText("��ġ �Ұ�: ��ġ ��� ����");
            return;
        }

        ClearTransposition();

        string Chiper = CollectEnglishAlphabet(adfgvx.chiperpart.GetChiperText());
        int InputPriority = 1;
        linelength = place.Length;
        rowlength = Chiper.Length / place.Length;
        for (int i = 0; i < place.Length; i++)
        {
            for (int j = 0; j < place.Length; j++)
            {
                if (place[j] == InputPriority)
                {
                    InputPriority++;
                    templine[j] = Chiper.Substring(0, rowlength);
                    Chiper = Chiper.Substring(rowlength);
                }
            }
        }

        ResizeAndRePositionEdge();
        printFlow();

        isreadyforinput = false;
        isflash = false;
    }

    public void OnTransposeReverseDown()//IntermediateChiper�� ���ͼ� Transposition�� ä�� �ִ´�
    {
        if (!isreadyforinput)
            return;

        //���� �߻�
        if (isonprintflow == true)
        {
            adfgvx.InfoBox.UpdateText("����ġ �Ұ�: �۾� ���� ��");
            return;
        }
        else if (keyword.Length == 0)
        {
            adfgvx.InfoBox.UpdateText("����ġ �Ұ�: ��ȣ Ű ����");
            return;
        }
        else if(adfgvx.intermediatepart.GetIntermediateChiper() == "")
        {
            adfgvx.InfoBox.UpdateText("����ġ �Ұ�: ����ġ ��� ����");
            return;
        }

        ClearTransposition();

        string Chiper = CollectEnglishAlphabet(adfgvx.intermediatepart.GetIntermediateChiper());
        linelength = place.Length;
        rowlength = Chiper.Length / place.Length;

        //�� ���� �� ���ư��鼭 ä���
        int length = Chiper.Length;
        for (int i = 0; i < length; i++)
        {
            templine[i % place.Length] += Chiper.Substring(0, 1);
            Chiper = Chiper.Substring(1);
        }

        ResizeAndRePositionEdge();
        printFlow();

        isreadyforinput = false;
        isflash = false;
    }

    public void ClearTransposition()//Transposition�� ����
    {
        for (int i = 0; i < 9; i++)
        {
            templine[i] = "";
            lines[i].text = "";
        }
    }

    private void ResizeAndRePositionEdge()//Transposition�� ���̵� ���� ����� ��� �� ũ�⿡ ���缭 ����� �����ϰ� ���ġ�Ѵ�
    {
        float pos_x = transform.GetChild(0).localPosition.x + (2.5f * (linelength - 1));
        float size_x = 2.5f * linelength;
        float size_y = 2.5f * rowlength;
        GetComponentsInChildren<SpriteRenderer>()[0].transform.localPosition = new Vector3(pos_x, -4, 0);
        GetComponentsInChildren<SpriteRenderer>()[0].size = new Vector2(size_x, size_y);
    }

    private void printFlow()//2���� �帧 ���
    {
        //�帧 ��� ����
        FlowLine = 0;
        InvokeRepeating("printFlowLine", 0.0f, 0.1f);
        //������ ����, ������ �ð� �Ŀ� ������ ȸ��
        keywordfieldtext.text = keyword;
        isonprintflow = true;
        Invoke("SetisonprintflowFalse", 0.1f * (rowlength + linelength));
    }

    private void printFlowLine()//�帧 ��� ������
    {
        if (FlowLine >= linelength)
        {
            CancelInvoke("printFlowLine");
            return;
        }
        StartCoroutine(printFlowRow(FlowLine++, 0));
    }

    private IEnumerator printFlowRow(int FlowLine, int FlowRow)//�帧 ��� �Ʒ���
    {
        if (FlowRow >= rowlength)
            yield break;
        lines[FlowLine].text += templine[FlowLine][FlowRow].ToString();
        yield return new WaitForSeconds(0.1f);
        StartCoroutine(printFlowRow(FlowLine, FlowRow + 1));
    }

    private void SetisonprintflowFalse()//isonprintflow������ �������� �Ѵ�_Invoke��
    {
        isonprintflow = false;
    }
}