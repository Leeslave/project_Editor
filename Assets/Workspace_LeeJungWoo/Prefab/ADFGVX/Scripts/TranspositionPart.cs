using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TranspositionPart : MonoBehaviour
{
    private ADFGVX adfgvx;

    private TextMeshPro[] lines;
    private TextMeshPro keyWordFieldText;
    private TextMeshPro priority;
    private SpriteRenderer transposedLineGuideBox;
    private SpriteRenderer keyWordFieldColor;

    private bool isFlash;                               //������
    private bool skipOneFlash;                          //true�� ������ �ѹ� �ǳʶڴ�

    private bool isOnPrintFlow;                         //�帧 ��� �۾� ���ΰ�? �۾� ���̸� ��ġ ��ư�� ����ġ ��ư�� ���� �� ����
    public bool isReadyForInput;                        //Ű���� �Է� ������ �����ΰ�?
    private bool isCursorOverInputField;                //�Է� �ʵ忡 Ŀ���� �ö� �ִ°�?

    //��ġ, ����ġ �� ����ϴ� ������
    private string keyword;                             //�Է� ���� Ű����
    private int[] place;                                //Ű���� ������ ����
    int rowLength;                                      //��ġ ����� �� ����
    int lineLength;                                     //��ġ ����� �� ����
    private string[] tempLine;                          //�帧 ��� ��, ��� �����س��´�
    private int flowLine;                               //�帧 ��� �ε���

    private void Awake()
    {
        adfgvx = GameObject.Find("ADFGVX").GetComponent<ADFGVX>();

        lines = new TextMeshPro[9];
        for (int i=0;i<9;i++)
        {
            lines[i] = GetComponentsInChildren<TextMeshPro>()[i];
            lines[i].text = "";
        }
        keyWordFieldText = GetComponentsInChildren<TextMeshPro>()[9];
        priority = GetComponentsInChildren<TextMeshPro>()[10];
        transposedLineGuideBox = GetComponentsInChildren<SpriteRenderer>()[0];
        keyWordFieldColor = GetComponentsInChildren<SpriteRenderer>()[4];

        transposedLineGuideBox.size = new Vector2(0, 0);
        keyWordFieldColor.color = new Color(0, 1, 0, 0);
        tempLine = new string[9];
        isReadyForInput = false;
        isFlash = false;

        ClearKeyWord();
        ClearPriority();
        FlashText();
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            if (isCursorOverInputField && !isReadyForInput)
            {
                isReadyForInput = true;
                keyWordFieldColor.color = new Color(0, 1, 0, 0);
                keyWordFieldText.text = keyword;
                isFlash = true;
            }
            else if (isCursorOverInputField && isReadyForInput)
            {

            }
            else
            {
                if (keyword == "")
                    keyWordFieldText.text = " Ŭ���Ͽ� �Է¡�";
                isReadyForInput = false;
                isFlash = false;
            }
        }
    }

    private void OnMouseEnter()
    {
        if (!isReadyForInput)
            keyWordFieldColor.color = new Color(0, 1, 0, 0.15f);
        isCursorOverInputField = true;
    }

    private void OnMouseExit()
    {
        keyWordFieldColor.color = new Color(0, 1, 0, 0);
        isCursorOverInputField = false;
    }

    public void SetLayer(int layer)//��� �Է� ����
    {
        this.gameObject.layer = layer;
    }

    private void FlashText()//Ű���� â�� �����̰� �����
    {
        StartCoroutine(FlashTextIEnumerator());
    }

    private IEnumerator FlashTextIEnumerator()//FlashText ���
    {
        if (keyword.Length <= 16 && isReadyForInput && !isOnPrintFlow && !skipOneFlash)
        {
            if (isFlash)
            {
                keyWordFieldText.text = keyword;
                isFlash = false;
            }
            else
            {
                keyWordFieldText.text = keyword + " ��";
                isFlash = true;
            }
        }
        else if (!isReadyForInput && keyword == "")
            keyWordFieldText.text = " Ŭ���Ͽ� �Է¡�";
        else if(!isReadyForInput && keyword != "")
            keyWordFieldText.text = keyword;

        //�̹� �Ͽ� ��ŵ������ ���� ������ ���ڿ��� �Ѵ�
        skipOneFlash = !skipOneFlash ? false : false;
        
        //0.5�� ���
        yield return new WaitForSeconds(0.5f);
        StartCoroutine("FlashText", 0.5f);
    }

    private void DelayFlashInputField()//�������� 0.5�� ���´�
    {
        skipOneFlash = true;
    }

    public void AddKeyWord(string value)//Ű���忡 �� �ܾ� �߰��Ѵ�
    {
        if(!isReadyForInput)
            return;
        else if(keyword.Length>16)
        {
            adfgvx.InformError("��ȣ Ű �ִ� �Է� : �Է� �Ұ�");
            return;
        }
        DelayFlashInputField();
        keyword = keyword + " " + value;
        keyWordFieldText.text = keyword;
        UpdatePriority();
    }

    public void DeleteKeyWord()//Ű���忡�� �� �ܾ� �����
    {
        if (!isReadyForInput)
            return;
        else if(keyword.Length<2)
        {
            adfgvx.InformError("��ȣ Ű �ּ� �Է� : ���� �Ұ�");
            return;
        }

        DelayFlashInputField();
        keyword = keyword.Substring(0, keyword.Length - 2);
        keyWordFieldText.text = keyword;
        UpdatePriority();
    }

    public void ClearKeyWord()//Ű���带 ����
    {
        keyword = "";
        keyWordFieldText.text = keyword;
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
        GetComponentsInChildren<SpriteRenderer>()[0].transform.localPosition = new Vector3(pos_x, -3.5f, 0);
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
        if (!isReadyForInput)
            return;

        //���� �߻�
        if (keyword.Length == 0)
        {
            adfgvx.InformError("��ġ ���� : ��ȣ Ű ����");
            return;
        }
        else if(adfgvx.chiperpart.GetChiperText()=="[������ ����]")
        {
            adfgvx.InformError("��ġ ���� : ��ġ ��� ����");
            return;
        }

        ClearTransposition();

        string Chiper = CollectEnglishAlphabet(adfgvx.chiperpart.GetChiperText());
        int InputPriority = 1;
        lineLength = place.Length;
        rowLength = Chiper.Length / place.Length;
        for (int i = 0; i < place.Length; i++)
        {
            for (int j = 0; j < place.Length; j++)
            {
                if (place[j] == InputPriority)
                {
                    InputPriority++;
                    tempLine[j] = Chiper.Substring(0, rowLength);
                    Chiper = Chiper.Substring(rowLength);
                }
            }
        }

        adfgvx.InformUpdate("�۾� �� : ���α׷��� ���� �������� ���ʽÿ�");
        ResizeAndRePositionEdge();
        printFlow();
        isFlash = false;
        isReadyForInput = false;

        adfgvx.soundFlow(rowLength + lineLength, 0, 0.1f * (rowLength + lineLength));
    }

    public void OnTransposeReverseDown()//IntermediateChiper�� ���ͼ� Transposition�� ä�� �ִ´�
    {
        if (!isReadyForInput)
            return;

        //���� �߻�
        if (keyword.Length == 0)
        {
            adfgvx.InformError("����ġ ���� : ��ȣ Ű ����");
            return;
        }
        else if(adfgvx.intermediatepart.GetIntermediateChiper() == "")
        {
            adfgvx.InformError("����ġ ���� : ����ġ ��� ����");
            return;
        }

        ClearTransposition();

        string Chiper = CollectEnglishAlphabet(adfgvx.intermediatepart.GetIntermediateChiper());
        lineLength = place.Length;
        rowLength = Chiper.Length / place.Length;

        //�� ���� �� ���ư��鼭 ä���
        int length = Chiper.Length;
        for (int i = 0; i < length; i++)
        {
            tempLine[i % place.Length] += Chiper.Substring(0, 1);
            Chiper = Chiper.Substring(1);
        }

        ResizeAndRePositionEdge();
        printFlow();
        isReadyForInput = false;
        isFlash = false;

        //���� ���
        adfgvx.soundFlow(rowLength + lineLength, 0, 0.1f * (rowLength + lineLength));
    }

    public void ClearTransposition()//Transposition�� ����
    {
        for (int i = 0; i < 9; i++)
        {
            tempLine[i] = "";
            lines[i].text = "";
        }
    }

    private void ResizeAndRePositionEdge()//Transposition�� ���̵� ���� ����� ��� �� ũ�⿡ ���缭 ����� �����ϰ� ���ġ�Ѵ�
    {
        float pos_x = transform.GetChild(0).localPosition.x + (2.5f * (lineLength - 1));
        float size_x = 2.5f * lineLength;
        float size_y = 2.5f * rowLength;
        GetComponentsInChildren<SpriteRenderer>()[0].transform.localPosition = new Vector3(pos_x, -3.5f, 0);
        GetComponentsInChildren<SpriteRenderer>()[0].size = new Vector2(size_x, size_y);
    }

    private void printFlow()//2���� ��� �帧 ���
    {
        adfgvx.SetPartLayer(2, 2, 2, 2);

        //�帧 ��� ����
        flowLine = 0;
        InvokeRepeating("printflowLine", 0.0f, 0.1f);

        //������ ����, ������ �ð� �Ŀ� ������ ȸ��
        keyWordFieldText.text = keyword;
        isOnPrintFlow = true;
        Invoke("SetisOnPrintFlowFalse", 0.1f * (rowLength + lineLength));
    }

    private void printflowLine()//�帧 ��� ������
    {
        if (flowLine == lineLength)
        {
            CancelInvoke("printflowLine");
            return;
        }
        StartCoroutine(printFlowRow(flowLine++, 0));
    }

    private IEnumerator printFlowRow(int flowLine, int FlowRow)//�帧 ��� �Ʒ���
    {
        if(flowLine == lineLength - 1 && FlowRow == rowLength)//�帧 ��� ���� ���� ��
        {
            adfgvx.SetPartLayer(0, 0, 0, 0);
            adfgvx.InformUpdate("��ġ �۾� ���� : �� �۾� �ð� " + (0.1f * (rowLength + lineLength)).ToString() + "s");
            yield break;
        }
        else if (FlowRow == rowLength)
            yield break;
        lines[flowLine].text += tempLine[flowLine][FlowRow].ToString();
        yield return new WaitForSeconds(0.1f);
        StartCoroutine(printFlowRow(flowLine, FlowRow + 1));
    }

    private void SetisOnPrintFlowFalse()//isOnPrintFlow������ �������� �Ѵ�_Invoke��
    {
        isOnPrintFlow = false;
    }
}