using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class AnimText : AnimBase
{
    public float DefaultStringTime = 0.05f;
    public float DefaultTextTime = 0.2f;
    public TextAsset ta;
    public TextInfo ti;
    // Use this for initialization
    void Start()
    {
        ti = JsonUtility.FromJson<TextInfo>(ta.text);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override IEnumerator PlayAnim()
    {
        yield return StartCoroutine(AnimTexts());
    }

    IEnumerator AnimTexts()
    {
        foreach (AnimString a in ti.ass)
        {
            switch (a.infoType)
            {
                case 0:
                    yield return StartCoroutine(AnimString(a.info, a.stringTime < 0 ? DefaultStringTime : a.stringTime));
                    yield return new WaitForSeconds(a.textTime < 0 ? DefaultTextTime : a.textTime);
                    break;
                case 1:
                    yield return StartCoroutine(AnimInt(a.info, a.stringTime < 0 ? DefaultStringTime : a.stringTime));
                    yield return new WaitForSeconds(a.textTime < 0 ? DefaultTextTime : a.textTime);
                    break;
            }
        }
    }

    IEnumerator AnimInt(string text, float time)
    {
        if (time != 0)
        {
            long tmp = Convert.ToInt64(text);
            int lens = text.Length + 2;
            string fm = "{0," + lens.ToString() + ":d}";
            int count = (int)(time / 0.01);
            GUITextCtrl.text += string.Format(fm, tmp * 0 / count);
            for (int i = 0; i < count; i++)
            {
                GUITextCtrl.text = GUITextCtrl.text.Substring(0, GUITextCtrl.text.Length - lens);
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

    IEnumerator AnimString(string text, float time)
    {
        if (time != 0)
            foreach (char c in text.ToCharArray())
            {
                GUITextCtrl.text += c;
                yield return new WaitForSeconds(time);
            }
        else
        {
            GUITextCtrl.text += text;
            yield return 0;
        }
    }
}

[Serializable]
public class TextInfo
{
    public List<AnimString> ass;
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
