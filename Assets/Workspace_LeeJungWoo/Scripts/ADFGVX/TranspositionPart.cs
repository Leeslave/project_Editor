using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TranspositionPart : MonoBehaviour
{
    private ADFGVX adfgvx;

    private InputField_ADFGVX keyword;
    private TextField priority;

    private TextMeshPro[] lines;

    private SpriteRenderer transposedLineGuideBox;

    //��ġ, ����ġ �� ����ϴ� ������
    private int[] place;                                //Ű���� ������ ����
    int rowLength;                                      //��ġ ����� �� ����
    int lineLength;                                     //��ġ ����� �� ����
    private string[] tempLine;                          //�帧 ��� ��, ��� �����س��´�
    private int flowLine;                               //�帧 ��� �ε���

    private void Start()
    {
        adfgvx = GameObject.Find("GameManager").GetComponent<ADFGVX>();

        lines = new TextMeshPro[9];
        for (int i=0;i<9;i++)
        {
            lines[i] = GetComponentsInChildren<TextMeshPro>()[i];
            lines[i].text = "";
        }

        keyword = transform.Find("Keyword").GetComponent<InputField_ADFGVX>();
        priority = transform.Find("Priority").GetComponent<TextField>();
        priority.SetText("");
        
        transposedLineGuideBox = GetComponentsInChildren<SpriteRenderer>()[0];
        transposedLineGuideBox.size = new Vector2(0, 0);
        tempLine = new string[9];
    }

    public void SetLayer(int layer)//�� ���ӿ�����Ʈ ���� ����� ���̾� ����
    {
        transform.Find("Keyword").gameObject.layer = layer;
    }

    public InputField_ADFGVX GetInputField_keyword()
    {
        return keyword;
    }

    public int[] GetPriority()
    {
        return place;
    }

    public void AddKeyword(string value)//Keyword�� �� ���� �Է�
    {
        if (value.ToCharArray()[0] < 'A' || value.ToCharArray()[0] > 'Z')
        {
            adfgvx.InformError("��ȿ���� ���� ��ġ Ű �Է� : �Է� �Ұ�");
            return;
        }

        keyword.AddInputField(value + " ");
        UpdatePriority();
    }

    public void DeleteKeyword()//Keyword���� �� ���� ����
    {
        keyword.DeleteInputField(2);
        UpdatePriority();
    }
  
    private void UpdatePriority()//Priority�� ���� ���Ѵ�
    {
        string result = "";
        string value = CollectEnglishAlphabet(keyword.GetInputString());
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

        for (int i = 0; i < keyword.GetInputString().Length / 2; i++)
        {
            result += place[i].ToString();
            result += " ";
        }

        ClearTransposition();

        float pos_x = transform.GetChild(0).localPosition.x + (2.5f * (place.Length - 1));
        float size_x = 2.5f * place.Length;
        float size_y = 2.5f;
        GetComponentsInChildren<SpriteRenderer>()[0].transform.localPosition = new Vector3(pos_x, -3.5f, 0);
        GetComponentsInChildren<SpriteRenderer>()[0].size = new Vector2(size_x, size_y);

        priority.SetText(result);
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

    public void OnTransposeDown()//EncodeData�� Ű ������ ���� Transposition�� ��ġ
    {
        //���� �߻�
        if (keyword.GetInputString().Length == 0)
        {
            adfgvx.InformError("��ġ ���� : ��ġ Ű ����");
            keyword.SetIsReadyForInput(false);
            keyword.SetIsFlash(false);
            return;
        }
        else if (adfgvx.encodeDataLoadPart.GetData() == "��ȣȭ �����͸� �ε��Ͽ� ���ۡ�")
        {
            adfgvx.InformError("��ġ ���� : ��ġ ��� ����");
            keyword.SetIsReadyForInput(false);
            keyword.SetIsFlash(false);
            return;
        }
        else if (CollectEnglishAlphabet(adfgvx.encodeDataLoadPart.GetData()).Length / place.Length > 12)
        {            
            //Ʃ�丮�� ���� �ڵ�
            if (adfgvx.GetCurrentTutorialPhase() == 1 && adfgvx.CurrentMode == ADFGVX.mode.Decoding)
            {
                if (CollectEnglishAlphabet(keyword.GetInputString()) != "HELLO")
                    adfgvx.DisplayTutorialDialog(41, 0f);
                else
                    adfgvx.MoveToNextTutorialPhase(2.0f);
            }

            adfgvx.InformError("��ġ ���� : �޸� �뷮 �ʰ�");
            keyword.SetIsReadyForInput(false);
            keyword.SetIsFlash(false);
            return;
        }
        else if (CollectEnglishAlphabet(adfgvx.encodeDataLoadPart.GetData()).Length % place.Length != 0)
        {
            //Ʃ�丮�� ���� �ڵ�
            if (adfgvx.GetCurrentTutorialPhase() == 2 && adfgvx.CurrentMode == ADFGVX.mode.Decoding)
            {
                if (CollectEnglishAlphabet(keyword.GetInputString()).Length != 7)
                    adfgvx.DisplayTutorialDialog(73, 0f);
                else
                    adfgvx.MoveToNextTutorialPhase(2.0f);
            }

            adfgvx.InformError("��ġ ���� : �޸� ���� �߻�");
            keyword.SetIsReadyForInput(false);
            keyword.SetIsFlash(false);
            return;
        }

        //Ű ���� �ʱ�ȭ
        ClearTransposition();

        string Chiper = CollectEnglishAlphabet(adfgvx.encodeDataLoadPart.GetData());
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

        adfgvx.InformUpdate("��ġ �۾� �ߡ� ���α׷��� ���� �������� ���ʽÿ�");
        ResizeAndRePositionEdge();
        printFlow();

        keyword.SetIsReadyForInput(false);
        keyword.SetIsFlash(false);

        //���� ���
        adfgvx.soundFlow(rowLength + lineLength, 0.1f * (rowLength + lineLength));

        //Ʃ�丮�� ���� �ڵ�
        if (adfgvx.GetCurrentTutorialPhase() == 3 && adfgvx.CurrentMode == ADFGVX.mode.Decoding)
        {
            if (CollectEnglishAlphabet(keyword.GetInputString()) != "SUKHOI")
                adfgvx.DisplayTutorialDialog(85, 0f);
            else
                adfgvx.MoveToNextTutorialPhase(0.1f * (rowLength + lineLength));
        }
    }

    public void OnTransposeReverseDown()//OriginalData�� Ű ������ ���� Transposition�� ����ġ
    {
        //���� �߻�
        if (keyword.GetInputString().Length == 0)
        {
            adfgvx.InformError("����ġ ���� : ��ġ Ű ����");
            keyword.SetIsReadyForInput(false);
            keyword.SetIsFlash(false);
            return;
        }
        else if (adfgvx.beforeEncodingPart.GetInputField_Data().GetInputString() == "")
        {
            adfgvx.InformError("����ġ ���� : ����ġ ��� ����");
            keyword.SetIsReadyForInput(false);
            keyword.SetIsFlash(false);
            return;
        }
        else if (CollectEnglishAlphabet(adfgvx.beforeEncodingPart.GetInputField_Data().GetInputString()).Length / place.Length > 12)
        {
            adfgvx.InformError("����ġ ���� : �޸� �뷮 �ʰ�");
            keyword.SetIsReadyForInput(false);
            keyword.SetIsFlash(false);
            return;
        }
        else if (CollectEnglishAlphabet(adfgvx.beforeEncodingPart.GetInputField_Data().GetInputString()).Length % place.Length != 0)
        {
            adfgvx.InformError("����ġ ���� : �޸� ���� �߻�");
            keyword.SetIsReadyForInput(false);
            keyword.SetIsFlash(false);
            return;
        }

        ClearTransposition();

        string Chiper = CollectEnglishAlphabet(adfgvx.beforeEncodingPart.GetInputField_Data().GetInputString());
        lineLength = place.Length;
        rowLength = Chiper.Length / place.Length;

        //�� ���� �� ���ư��鼭 ä���
        int length = Chiper.Length;
        for (int i = 0; i < length; i++)
        {
            tempLine[i % place.Length] += Chiper.Substring(0, 1);
            Chiper = Chiper.Substring(1);
        }

        adfgvx.InformUpdate("����ġ �۾� �ߡ� ���α׷��� ���� �������� ���ʽÿ�");
        ResizeAndRePositionEdge();
        printFlow();

        keyword.SetIsReadyForInput(false);
        keyword.SetIsFlash(false);

        //���� ���
        adfgvx.soundFlow(rowLength + lineLength, 0.1f * (rowLength + lineLength));

        //Ʃ�丮�� ���� �ڵ�
        if(adfgvx.GetCurrentTutorialPhase() == 2 && adfgvx.CurrentMode == ADFGVX.mode.Encoding)
        {
            adfgvx.MoveToNextTutorialPhase(0.1f * (rowLength + lineLength));
        }
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
        adfgvx.SetPartLayerWaitForSec(0f, 2, 2, 2, 2, 2, 2, 2, 2, 2);

        //�帧 ��� ����
        flowLine = 0;
        InvokeRepeating("printFlowLine", 0.0f, 0.1f);
        keyword.SetMarkText(keyword.GetInputString());
    }

    private void printFlowLine()//�帧 ��� ������
    {
        if (flowLine == lineLength)
        {
            CancelInvoke("printFlowLine");
            return;
        }
        StartCoroutine(printFlowRow(flowLine++, 0));
    }

    private IEnumerator printFlowRow(int flowLine, int FlowRow)//�帧 ��� �Ʒ���
    {
        if(flowLine == lineLength - 1 && FlowRow == rowLength)//�帧 ��� ���� ���� ��
        {
            adfgvx.SetPartLayerWaitForSec(0f, 0, 0, 0, 0, 0, 0, 0, 0, 0);
            adfgvx.InformUpdate("��ġ �۾� ���� : �� �۾� �ð� " + (0.1f * (rowLength + lineLength)).ToString() + "s");
            yield break;
        }
        else if (FlowRow == rowLength)
            yield break;
        lines[flowLine].text += tempLine[flowLine][FlowRow].ToString();
        yield return new WaitForSeconds(0.1f);
        StartCoroutine(printFlowRow(flowLine, FlowRow + 1));
    }
}