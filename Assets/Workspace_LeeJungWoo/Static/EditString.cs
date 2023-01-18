using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EditStirng : MonoBehaviour
{
    public static string CollectEnglishUpperAlphabet(string value)//빈칸, 숫자 등을 제외하고 영어 알파벳만 모아서 반환한다
    {
        //value에 들어있는 알파벳 개수 확인, 새롭게 만들어질 array의 길이를 구한다
        int newArrayLenght = 0;
        for (int i = 0; i < value.Length; i++)
        {
            if (value[i] >= 'A' && value[i] <= 'Z' || value[i] == '_')
                newArrayLenght++;
        }

        //value의 알파벳 개수 만큼 array 할당
        char[] array = new char[newArrayLenght];
        int idx = 0;

        //array에 idx를 늘려가면서 알파벳 전부 저장
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
