using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EditStirng : MonoBehaviour
{
    public static string CollectEnglishUpperAlphabet(string value)//��ĭ, ���� ���� �����ϰ� ���� ���ĺ��� ��Ƽ� ��ȯ�Ѵ�
    {
        //value�� ����ִ� ���ĺ� ���� Ȯ��, ���Ӱ� ������� array�� ���̸� ���Ѵ�
        int newArrayLenght = 0;
        for (int i = 0; i < value.Length; i++)
        {
            if (value[i] >= 'A' && value[i] <= 'Z' || value[i] == '_')
                newArrayLenght++;
        }

        //value�� ���ĺ� ���� ��ŭ array �Ҵ�
        char[] array = new char[newArrayLenght];
        int idx = 0;

        //array�� idx�� �÷����鼭 ���ĺ� ���� ����
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


}
