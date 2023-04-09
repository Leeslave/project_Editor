using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.UI;

public class TextAnimation : AnimBase
{
    /**
    *   json 텍스트코드를 불러와 딜레이 애니메이션 기능 구현
    *   - 글자/숫자 애니메이션 구현
    */
    public TextAsset textAsset;
    public TextInfo textInfo;
    public float delayEachChar = 0.05f;
    public float delayEachStringLine = 0.2f;

    void Awake()
    {
        if (textAsset != null)
            textInfo = JsonUtility.FromJson<TextInfo>(textAsset.text);  // json 텍스트 불러오기
    }

    // 텍스트 코루틴 실행
    public override IEnumerator Play()
    {   
        Debug.Log($"{gameObject.ToString()}Play Animation");
        yield return StartCoroutine(AnimTexts());
        
    }

    // TextInfo 내부의 텍스트들의 애니메이션 코루틴 실행
    IEnumerator AnimTexts()
    {
        foreach (AnimString iter in textInfo.textStrings)
        {
            // info 타입으로 애니메이션 타입 구분 (0 : string, 1 : int)
            switch (iter.infoType)
            {
                case 0:
                    yield return StartCoroutine(AnimString(iter.info, iter.stringTime < 0 ? delayEachChar : iter.stringTime));
                    yield return new WaitForSeconds(iter.textTime < 0 ? delayEachStringLine : iter.textTime);
                    break;
                case 1:
                    yield return StartCoroutine(AnimInt(iter.info, iter.stringTime < 0 ? delayEachChar : iter.stringTime));
                    yield return new WaitForSeconds(iter.textTime < 0 ? delayEachStringLine : iter.textTime);
                    break;
            }
        }
    }

    // int 타입 text 애니메이션
    IEnumerator AnimInt(string text, float time)
    {
        if (time != 0)  //딜레이가 0이 아닐때
        {
            long tmp = Convert.ToInt64(text);   //텍스트 전환
            int lens = text.Length + 2;         //길이 저장
            string fm = "{0," + lens.ToString() + ":d}";    // {0, ?:d}    
            int count = (int)(time / 0.01);                 // time (100배)
            GUITextCtrl.text += string.Format(fm, tmp * 0 / count); // GUIText(상속됨)에 형식 텍스트 추가    
            //.text + "{0, 길이+2:d}" <= tmp * 0 / count
            for (int i = 0; i < count; i++)
            {
                // idx = 0~길이 - lens 만큼 자르기 (뒤에서 lens만큼 잘라내기)
                GUITextCtrl.text = GUITextCtrl.text.Substring(0, GUITextCtrl.text.Length - lens);
                // 뒤에 {tmp * i /count} 글자 추가 글자 뒤에 공백도 추가 (fm 포맷)
                GUITextCtrl.text += string.Format(fm, tmp * i / count);
                yield return new WaitForSeconds(0.01f);
            }
        }
        else
        {
            GUITextCtrl.text += text;
            yield return 0;
        }
    }

    // string 타입 text 애니메이션
    IEnumerator AnimString(string text, float time)
    {
        if (time != 0)
            foreach (char c in text.ToCharArray())
            {
                GUITextCtrl.text += c;
                yield return new WaitForSeconds(time);  //1글자씩 time 간격으로 출력
            }
        else                                            //딜레이 없이 일괄 출력
        {
            GUITextCtrl.text += text;
            yield return 0;
        }
    }
}

[Serializable]
public class TextInfo
{
    public List<AnimString> textStrings;
}

[Serializable]
public class AnimString
{
    public string info;
    public int infoType;
    public float textTime;
    public float stringTime;

    public AnimString(string _i, int _it, float _t, float _s)
    {
        info = _i;
        infoType = _it;
        textTime = _t;
        stringTime = _s;
    }
}
