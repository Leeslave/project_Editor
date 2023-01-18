using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;

public class Log : MonoBehaviour
{
    private TextMeshPro markText;

    private FileInfo fileTxt;
    private StreamReader loadTxt;
    private string[] logs;

    private void Start()
    {
        if (transform.Find("MarkText") != null)
            markText = transform.Find("MarkText").GetComponent<TextMeshPro>();
    }

    public void SetColorText(Color value)
    {
        markText.color = value;
    }

    public void FillLoadingLog(float time, string fileName)//지정한 파일을 로드해서 로그를 채움
    {
        GetLogTxtFile(fileName);
        markText.text = "";
        StartCoroutine(FillLoadingLogIEnumerator(time, 0, 0));
    }

    private IEnumerator FillLoadingLogIEnumerator(float time, float currentTime, int idx)//로그를 채움 재귀
    {
        if (Mathf.CeilToInt(currentTime / time * logs.Length) == idx + 1)
        {
            if (idx < 5)
            {
                markText.text = logs[idx] + '\n' + markText.text;
            }
            else
            {
                int IndexOfSecondFromBack;
                for (IndexOfSecondFromBack = markText.text.Length - 2; markText.text[IndexOfSecondFromBack] != '\n'; IndexOfSecondFromBack--)
                {
                }
                markText.text = markText.text.Substring(0, IndexOfSecondFromBack + 1);
                markText.text = logs[idx] + '\n' + markText.text;
            }
            idx += 1;
        }

        currentTime += time / 100;
        if (currentTime > time)
            yield break;
        yield return new WaitForSeconds(time / 100);
        StartCoroutine(FillLoadingLogIEnumerator(time, currentTime, idx));
    }

    private void GetLogTxtFile(string fileName)//이번 로딩 텍스트 파일을 불러온다
    {
        string filePath = "Assets/Workspace_LeeJungWoo/Prefab/ADFGVX/LogTxt/" + fileName + ".txt";
        fileTxt = new FileInfo(filePath);

        if (fileTxt.Exists)
        {
            loadTxt = new StreamReader(filePath, System.Text.Encoding.UTF8);
            int length;
            for (length = 0; ; length++)
            {
                string value = loadTxt.ReadLine();
                if (value == null)
                    break;
            }
            logs = new string[length];

            loadTxt = new StreamReader(filePath, System.Text.Encoding.UTF8);
            for (int i = 0; i < length; i++)
            {
                logs[i] = loadTxt.ReadLine();
            }
        }
        else
            Debug.Log("Unexist filepath!");
    }

    public void HideTextOnly(float time)
    {
        ConvertColorText(markText, time, Color.clear);
    }

    private void ConvertColorText(TextMeshPro target, float time, Color targetValue)//텍스트 색 변환
    {
        StartCoroutine(ConvertColorTextIEnumerator(target, time, 0, targetValue));
    }

    private IEnumerator ConvertColorTextIEnumerator(TextMeshPro target, float time, float currentTime, Color targetValue)//텍스트 색 변환 재귀
    {
        currentTime += time / 100;
        if (currentTime > time)
            yield break;

        float target_r = target.color.r + (targetValue.r - target.color.r) * (currentTime / time);
        float target_g = target.color.g + (targetValue.g - target.color.g) * (currentTime / time);
        float target_b = target.color.b + (targetValue.b - target.color.b) * (currentTime / time);
        float target_a = target.color.a + (targetValue.a - target.color.a) * (currentTime / time);
        target.color = new Color(target_r, target_g, target_b, target_a);

        yield return new WaitForSeconds(time / 100);
        StartCoroutine(ConvertColorTextIEnumerator(target, time, currentTime, targetValue));
    }
}
