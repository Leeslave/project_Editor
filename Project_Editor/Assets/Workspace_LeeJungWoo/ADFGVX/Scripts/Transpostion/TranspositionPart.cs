using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TranspositionPart : MonoBehaviour
{
    private ADFGVXscript ADFGVX;

    //Ű���� �Է� â ����
    private bool selected;
    private bool isover;

    //��ġ ��ȣ Ű����� �� ���� ����
    private string keyword;
    private int[] place;

    //�Է�â ������
    private bool isflash;
    private bool onWork;

    //��ġ �۾�
    int rowlength;
    int linelength;
    private string[] interline;
    private int FlowLine;

    private TextMeshPro[] lines;
    private TextMeshPro keywords;
    private TextMeshPro prioritys;

    private void Awake()
    {
        ADFGVX = GameObject.Find("ADFGVX_Script").GetComponent<ADFGVXscript>();

        lines = new TextMeshPro[9];
        interline = new string[9];
        for (int i=0;i<9;i++)
        {
            lines[i] = GetComponentsInChildren<TextMeshPro>()[i];
            lines[i].text = "";
        }
        keywords = GetComponentsInChildren<TextMeshPro>()[9];
        prioritys = GetComponentsInChildren<TextMeshPro>()[10];

        GetComponentsInChildren<SpriteRenderer>()[0].size = new Vector2(0, 0);

        ClearKeyWord();
        ClearPriority();

        selected = false;
        isflash = false;
        StartCoroutine("FlashText", 0.5f);
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

    IEnumerator FlashText()//Ű���� â�� �����̰� �����
    {
        if (keyword.Length <= 16 && selected && !onWork)
        {
            if (isflash)
            {
                keywords.text = keyword;
                isflash = false;
            }
            else
            {
                keywords.text = keyword + " _";
                isflash = true;
            }
        }
        else if (keyword == "")
            keywords.text = " Ŭ���Ͽ� �Է�...";
        else if(!selected)
            keywords.text = keyword;

        yield return new WaitForSeconds(0.5f);
        StartCoroutine("FlashText", 0.5f);
    }

    public void AddKeyWord(string value)//Ű���忡 �� �ܾ� �߰��Ѵ�
    {
        if(!selected)
            return;
        else if(keyword.Length>16)
        {
            ADFGVX.UpdateInfoBox("��ȣ Ű �ִ� �Է� ��Ȯ�� ���");
            ADFGVX.InformCurrentMode();
            return;
        }

        keyword = keyword + " " + value;
        keywords.text = keyword;
        UpdatePriority();
    }

    public void DeleteKeyWord()//Ű���忡�� �� �ܾ� �����
    {
        if (!selected)
            return;
        else if(keyword.Length<2)
        {
            ADFGVX.UpdateInfoBox("��ȣ�� ���� �Ұ� ��Ȯ�� ���");
            ADFGVX.InformCurrentMode();
            return;
        }

        keyword = keyword.Substring(0, keyword.Length - 2);
        keywords.text = keyword;
        UpdatePriority();
    }

    private void ClearKeyWord()//Ű���带 ����
    {
        keyword = "";
        keywords.text = keyword;
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
        
        prioritys.text = Priority;
    }

    private void ClearPriority()//Priority�� ����
    {
        prioritys.text = "";
    }

    private string CollectEnglishAlphabet(string value)//��ĭ, ���� ���� �����ϰ� ���� ���ĺ��� ��Ƽ� ��ȯ�Ѵ�
    {
        //array0�� ����ִ� ���ĺ� ���� Ȯ��, ���Ӱ� ������� array1�� ���� Ȯ��
        int newarraylenght = 0;
        for (int i = 0; i < value.Length; i++)
        {
            if (value[i] >= 'A' && value[i] <= 'Z')
                newarraylenght++;
        }

        //array0�� ���ĺ� ���� ��ŭ array �Ҵ�
        char[] array = new char[newarraylenght];
        int idx = 0;

        //array01�� idx�� �÷����鼭 ���ĺ� ���� ����
        for (int i = 0; i < value.Length; i++)
        {
            if (value[i] >= 'A' && value[i] <= 'Z')
            {
                array[idx] = value[i];
                idx++;
            }
        }
        return array.ArrayToString();
    }

    public void OnTransposeDown()//EncodedChiper�� ���ͼ� Transposition�� ä�� �ִ´�
    {
        //���� �߻�
        if (onWork == true)
        {
            ADFGVX.InfoBox.UpdateText("��ġ �Ұ�: �۾� ���� ��");
            return;
        }
        else if (keyword.Length == 0)
        {
            ADFGVX.InfoBox.UpdateText("��ġ �Ұ�: ��ȣ Ű ����");
            return;
        }
        else if(GameObject.Find("EncodedChipertext").GetComponentsInChildren<TextMeshPro>()[1].text == "")
        {
            ADFGVX.InfoBox.UpdateText("��ġ �Ұ�: ��ġ ��� ����");
            return;
        }

        ClearTransposition();

        string Chiper = CollectEnglishAlphabet(GameObject.Find("EncodedChipertext").GetComponentsInChildren<TextMeshPro>()[1].text);
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
                    interline[j] = Chiper.Substring(0, rowlength);
                    Chiper = Chiper.Substring(rowlength);
                }
            }
        }

        ResizeAndRePositionEdge();
        printFlow();
    }

    public void OnTransposeReverseDown()//intermediateChiper�� ���ͼ� Transposition�� ä�� �ִ´�
    {
        //���� �߻�
        if (onWork == true)
        {
            ADFGVX.InfoBox.UpdateText("����ġ �Ұ�: �۾� ���� ��");
            return;
        }
        else if (keyword.Length == 0)
        {
            ADFGVX.InfoBox.UpdateText("����ġ �Ұ�: ��ȣ Ű ����");
            return;
        }
        else if(ADFGVX.InterChiperBox.GetComponentInChildren<TextMeshPro>().text == "")
        {
            ADFGVX.InfoBox.UpdateText("����ġ �Ұ�: ����ġ ��� ����");
            return;
        }

        ClearTransposition();

        string Chiper = CollectEnglishAlphabet(ADFGVX.InterChiperBox.GetComponentInChildren<TextMeshPro>().text);
        linelength = place.Length;
        rowlength = Chiper.Length / place.Length;

        //�� ���� �� ���ư��鼭 ä���
        int length = Chiper.Length;
        for (int i = 0; i < length; i++)
        {
            interline[i % place.Length] += Chiper.Substring(0, 1);
            Chiper = Chiper.Substring(1);
        }

        ResizeAndRePositionEdge();
        printFlow();
    }

    private void ClearTransposition()//Transposition�� ����
    {
        for (int i = 0; i < 9; i++)
        {
            interline[i] = "";
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

    private void printFlow()//�帧 ���
    {
        //�帧 ��� ����
        FlowLine = 0;
        InvokeRepeating("printFlowLine", 0.0f, 0.2f);
        //������ ����, ������ �ð� �Ŀ� ������ ȸ��
        keywords.text = keyword;
        onWork = true;
        Invoke("SetonWorkFalse", 0.2f * (rowlength + linelength));
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
        lines[FlowLine].text += interline[FlowLine][FlowRow].ToString();
        yield return new WaitForSeconds(0.2f);
        StartCoroutine(printFlowRow(FlowLine, FlowRow + 1));
    }

    private void SetonWorkFalse()//onWork������ �������� �Ѵ�_Invoke��
    {
        onWork = false;
    }
}