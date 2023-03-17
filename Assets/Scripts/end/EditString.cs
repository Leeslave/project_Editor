using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EditStirng : MonoBehaviour
{
    public static string CollectEnglishUpperAlphabet(string value)//영어의 대문자 알파벳만 모아준다
    {
        //array 길이 확인
        int newArrayLenght = 0;
        for (int i = 0; i < value.Length; i++)
        {
            if (value[i] >= 'A' && value[i] <= 'Z' || value[i] == '_')
                newArrayLenght++;
        }

        //array 할당
        char[] array = new char[newArrayLenght];
        int idx = 0;

        //array에 영어 알파벳만 채워넣음
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