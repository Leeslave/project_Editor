using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;

public class VerificationPanelPart : MonoBehaviour
{
    private ADFGVX adfgvx;

    private SpriteRenderer screenBlur;
    private SpriteRenderer panelBackground;
    private SpriteRenderer panelGuide;
    private SpriteRenderer loadingGageGuide;
    private SpriteRenderer loadingGageBar;
    
    private TextMeshPro loadingPercent;
    private TextMeshPro loadingInform;
    private TextMeshPro panelTitle;
    private TextMeshPro loadingLog;
    private TextMeshPro verificationInfo;

    private float percent;
    private float size_x;

    private FileInfo fileTxt;
    private StreamReader loadTxt;
    private string[] loadingLogs;

    private void Start()
    {
        adfgvx = GameObject.Find("ADFGVX").GetComponent<ADFGVX>();

        screenBlur = GetComponentsInChildren<SpriteRenderer>()[0];
        panelBackground = GetComponentsInChildren<SpriteRenderer>()[1];
        panelGuide = GetComponentsInChildren<SpriteRenderer>()[2];
        loadingGageGuide = GetComponentsInChildren<SpriteRenderer>()[3];
        loadingGageBar = GetComponentsInChildren<SpriteRenderer>()[4];

        loadingPercent = GetComponentsInChildren<TextMeshPro>()[0];
        loadingInform = GetComponentsInChildren<TextMeshPro>()[1];
        panelTitle = GetComponentsInChildren<TextMeshPro>()[2];
        loadingLog = GetComponentsInChildren<TextMeshPro>()[3];


        UnvisibleVerificationPanel();
    }

    public void StartVerificationSequence()
    {
        //모든 입력 차단
        adfgvx.SetPartLayer(2, 2, 2, 2);

        //확인 패널 가시 모드
        VisibleVerificationPanel();

        //게이지 바 2초
        FillGageBar(3f);

        //로딩 로그 2.8초
        GetLoadingTxtFile(0);
        FillLoadingLog(loadingLogs.Length, 3.8f);

        //사운드 재생
        adfgvx.PlayAudioSource(5);

        EndVerificationSequence();
    }

    private void VisibleVerificationPanel()
    {
        screenBlur.color = new Color(1, 1, 1, 1);
        panelBackground.color = new Color(1, 1, 1, 1);
        panelGuide.color = new Color(1, 1, 1, 1);
        loadingGageGuide.color = new Color(1, 1, 1, 1);
        loadingGageBar.color = new Color(0.149f, 1, 0, 0.5f);

        loadingPercent.color = new Color(1, 1, 1, 1);
        loadingInform.color = new Color(1, 1, 1, 1);
        panelTitle.color = new Color(1, 1, 1, 1);
        loadingLog.color = new Color(1, 1, 1, 1);

        this.transform.localPosition = new Vector3(-62.2f, -45.7f, 0);
    }

    private void UnvisibleVerificationPanel()
    {
        screenBlur.color = new Color(1, 1, 1, 0);
        panelBackground.color = new Color(1, 1, 1, 0);
        panelGuide.color = new Color(1, 1, 1, 0);
        loadingGageGuide.color = new Color(1, 1, 1, 0);
        loadingGageBar.color = new Color(1, 1, 1, 0);

        loadingPercent.color = new Color(1, 1, 1, 0);
        loadingInform.color = new Color(1, 1, 1, 0);
        panelTitle.color = new Color(1, 1, 1, 0);
        loadingLog.color = new Color(1, 1, 1, 0);

        this.transform.localPosition = new Vector3(-261, -45.7f, 0);
    }

    private void FillGageBar(float time)
    {
        percent = 0;
        loadingGageBar.size = new Vector2(0.01f, 3.8f);
        StartCoroutine(FillGageBarIEnumerator(0, time));
    }

    private IEnumerator FillGageBarIEnumerator(int idx, float time)
    {
        percent+=0.1f;
        size_x = 47.6f / 100 * percent;
        loadingGageBar.size = new Vector2(size_x, 3.8f);
        if (idx>1000)
            yield break;
        loadingPercent.text = Mathf.FloorToInt(percent).ToString() + "%";
        yield return new WaitForSeconds(time/1000);
        StartCoroutine(FillGageBarIEnumerator(idx + 1, time));
    }

    private void FillLoadingLog(int length, float time)
    {
        loadingLog.text = "";
        StartCoroutine(FillLoadingLogIEnumerator(0, length, time));
    }

    private IEnumerator FillLoadingLogIEnumerator(int idx, int length, float time)
    {
        if (idx >= length)
            yield break;
        if(idx < 5)
        {
            loadingLog.text = loadingLogs[idx] + '\n' + loadingLog.text;
        }
        else
        {
            int IndexOfSecondFromBack;
            for(IndexOfSecondFromBack = loadingLog.text.Length - 2;loadingLog.text[IndexOfSecondFromBack] != '\n' ;IndexOfSecondFromBack--)
            {

            }
            loadingLog.text = loadingLog.text.Substring(0, IndexOfSecondFromBack + 1);
            loadingLog.text = loadingLogs[idx] + '\n' + loadingLog.text;
        }
        yield return new WaitForSeconds(time / length);
        StartCoroutine(FillLoadingLogIEnumerator(idx + 1, length, time));
    }

    private void GetLoadingTxtFile(int fileNum)//이번 로딩 텍스트 파일을 불러온다
    {
        string filePath = "Assets/Workspace_LeeJungWoo/Prefab/ADFGVX/LoadingTxt/LoadingTxt_" + fileNum.ToString() + ".txt";
        fileTxt = new FileInfo(filePath);

        if (fileTxt.Exists)
        {
            loadTxt = new StreamReader(filePath, System.Text.Encoding.UTF8);
            int length;
            for(length = 0; ;length++)
            {
                string value = loadTxt.ReadLine();
                if (value == null)
                    break;
            }
            loadingLogs = new string[length];

            loadTxt = new StreamReader(filePath, System.Text.Encoding.UTF8);
            for (int i=0;i<length;i++)
            {
                loadingLogs[i] = loadTxt.ReadLine();
            }
        }
        else
            Debug.Log("Unexist filepath!");
    }

    private void EndVerificationSequence()
    {
        StartCoroutine(EndVerificationSequenceIEnumerator());
    }

    private IEnumerator EndVerificationSequenceIEnumerator()
    {
        if(adfgvx.ReturnDecodeScore())
        {
            yield return new WaitForSeconds(4.5f);

            panelTitle.text = "최종 복호화 시퀀스 완료";
            SizeUpText(panelTitle, 1.66f, 1.66f, 1);
            ConvertColorText(panelTitle, 1, new Color(0.1f, 0.35f, 0.85f, 1));
            
            TransparentZeroText(loadingPercent, 1);
            TransparentZeroText(loadingInform, 1);
            TransparentZeroText(loadingLog, 1);

            TransparentZeroSprite(loadingGageGuide, 1);
            TransparentZeroSprite(loadingGageBar, 1);

        }
        else
        {
            yield return new WaitForSeconds(4.5f);
           
            panelTitle.text = "최종 복호화 시퀀스 실패";
            SizeUpText(panelTitle, 1.66f, 1.66f, 1);
            ConvertColorText(panelTitle, 1, new Color(0.76f,0.28f,0.28f,1));

            TransparentZeroText(loadingPercent, 1);
            TransparentZeroText(loadingInform, 1);
            TransparentZeroText(loadingLog, 1);
            
            TransparentZeroSprite(loadingGageGuide, 1);
            TransparentZeroSprite(loadingGageBar, 1);

        }
    }

    private void SizeUpText(TextMeshPro target, float x, float y, float time)
    {
        int timer = new int();
        timer = 0;
        StartCoroutine(SizeUpTextIEnumerator(target,x,y,time,timer));
    }

    private IEnumerator SizeUpTextIEnumerator(TextMeshPro target, float x, float y, float time, float currentTime)
    {
        currentTime += time / 1000;
        if (currentTime > time)
            yield break;

        target.transform.localScale = new Vector3(1 + (x - 1) * (currentTime / time), 1 + (y - 1) * (currentTime / time), 1);

        yield return new WaitForSeconds(time / 1000);
        StartCoroutine(SizeUpTextIEnumerator(target, x, y, time, currentTime));
    }

    private void TransparentZeroText(TextMeshPro target, float time)
    {
        int timer = new int();
        timer = 0;
        StartCoroutine(TransparentZeroTextIEnumerator(target, time, timer));
    }

    private IEnumerator TransparentZeroTextIEnumerator(TextMeshPro target, float time, float currentTime)
    {
        currentTime += time / 1000;
        if (currentTime > time)
            yield break;

        target.color = new Color(1, 1, 1, Mathf.Clamp(1 - (currentTime / time), 0, 1));

        yield return new WaitForSeconds(time / 1000);
        StartCoroutine(TransparentZeroTextIEnumerator(target, time, currentTime));
    }

    private void TransparentZeroSprite(SpriteRenderer target, float time)
    {
        int timer = new int();
        timer = 0;
        StartCoroutine(TransparentZeroSpriteIEnumerator(target, time, timer));
    }

    private IEnumerator TransparentZeroSpriteIEnumerator(SpriteRenderer target, float time, float currentTime)
    {
        currentTime += time / 1000;
        if (currentTime > time)
            yield break;

        target.color = new Color(1, 1, 1, 1 - Mathf.Clamp(currentTime / time, 0, 1));

        yield return new WaitForSeconds(time / 1000);
        StartCoroutine(TransparentZeroSpriteIEnumerator(target, time, currentTime));
    }

    private void ConvertColorText(TextMeshPro target, float time, Color value)
    {
        int timer = new int();
        timer = 0;
        StartCoroutine(ConvertColorTextIEnumerator(target, time, timer, value));
    }

    private IEnumerator ConvertColorTextIEnumerator(TextMeshPro target, float time, float currentTime, Color value)
    {
        currentTime += time / 1000;
        if (currentTime > time)
            yield break;

        target.color = new Color(value.r * (currentTime / time), value.g * (currentTime / time), value.b*(currentTime / time));

        yield return new WaitForSeconds(time / 1000);
        StartCoroutine(ConvertColorTextIEnumerator(target, time, currentTime, value));
    }

    private void FlowTextWithDelayTime(TextMeshPro target, string value, float delayTime)//딜레이 흐름 출력
    {
        StartCoroutine(FlowtextWithDelayTimeIEnumerator(target, value, 0, delayTime));
    }

    private IEnumerator FlowtextWithDelayTimeIEnumerator(TextMeshPro target, string value, int idx, float delayTime)//딜레이 흐름 출력 재귀
    {
        if (idx >= value.Length)//끝까지 다 출력했음
            yield break;
        target.text += value.Substring(idx, 1);
        yield return new WaitForSeconds(delayTime);
        StartCoroutine(FlowtextWithDelayTimeIEnumerator(target, value, idx + 1, delayTime));
    }
}
