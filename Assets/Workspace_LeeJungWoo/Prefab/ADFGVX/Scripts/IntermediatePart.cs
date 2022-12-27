using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class IntermediatePart : MonoBehaviour
{
    private ADFGVX adfgvx;

    private TextMeshPro partTitle;
    private TextMeshPro fileRoute;
    public TextMeshPro chiperUI;
    public TextMeshPro chiperTitle;
    private TextMeshPro chiper;
    public TextMeshPro dateUI;
    public TextMeshPro date;
    public TextMeshPro senderUI;
    public TextMeshPro sender;

    private SpriteRenderer inputFieldColor;
    private string inputString;
    private bool isCursorOverInputField;
    public bool isReadyForInput;
    private bool isFlash;
    private bool skipOneFlash;

    private void Awake()
    {
        adfgvx = GameObject.Find("ADFGVX").GetComponent<ADFGVX>();

        partTitle = GetComponentsInChildren<TextMeshPro>()[0];
        fileRoute = GetComponentsInChildren<TextMeshPro>()[1];
        chiperUI = GetComponentsInChildren<TextMeshPro>()[2];
        chiperTitle = GetComponentsInChildren<TextMeshPro>()[3];
        chiper = GetComponentsInChildren<TextMeshPro>()[4];
        dateUI = GetComponentsInChildren<TextMeshPro>()[5];
        date = GetComponentsInChildren<TextMeshPro>()[6];
        senderUI = GetComponentsInChildren<TextMeshPro>()[7];
        sender = GetComponentsInChildren<TextMeshPro>()[8];

        inputFieldColor = GetComponentsInChildren<SpriteRenderer>()[0];
        inputFieldColor.color = new Color(0, 1, 0, 0);
        inputString = "";
        isFlash = false;

        ClearIntermediateChiper();
        ClearIntermediateChiperAll();
        InitializeIntermediateChiperAll();
        StartCoroutine(FlashInputField());
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (isCursorOverInputField && !isReadyForInput)
            {
                inputFieldColor.color = new Color(0, 1, 0, 0);
                chiper.text = inputString + "…";
                isReadyForInput = true;
                isFlash = true;
            }
            else if (isCursorOverInputField && isReadyForInput)
            {

            }
            else
            {
                InitializeIntermediateChiperAll();
                isReadyForInput = false;
                isFlash = false;
            }
        }

        if (adfgvx.currentmode == ADFGVX.mode.Encoding)
            fileRoute.text = "경로: 암호화 데이터";
        else
            fileRoute.text = "경로: 복호화 데이터";
    }

    private void OnMouseEnter()
    {
        if (!isReadyForInput)
            inputFieldColor.color = new Color(0, 1, 0, 0.15f);
        isCursorOverInputField = true;
    }

    private void OnMouseExit()
    {
        inputFieldColor.color = new Color(0, 1, 0, 0);
        isCursorOverInputField = false;
    }


    public void SetLayer(int layer)//모든 입력 차단
    {
        this.gameObject.layer = layer;
        GameObject.Find("Save").layer = layer;
    }

    IEnumerator FlashInputField()//검색창을 깜박이게 만든다
    {
        if (inputString.Length <= 192 && isReadyForInput && !skipOneFlash)//입력창 길이를 넘기거나, 입력 중이 아니거나, 스킵 명령이 있다면 건너뛴다
        {
            if (isFlash)
            {
                chiper.text = inputString;
                isFlash = false;
            }
            else
            {
                chiper.text = inputString + "…";
                isFlash = true;
            }
        }
        else if (!isReadyForInput && inputString != "")//입력 중이 아니나 빈칸이 아니라면, 입력 상태를 유지한다
            chiper.text = inputString;

        //이번 턴에 스킵했으니 다음 번에는 깜박여야 한다
        skipOneFlash = !skipOneFlash ? false : false;                               
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(FlashInputField());
    }

    private void DelayFlashInputField()//깜박임을 0.5초 막는다
    {
        skipOneFlash = true;
        return;
    }

    public void AddIntermediateChiper(string value)
    {
        if (!isReadyForInput)
            return;

        if(inputString.Length > 192)
        {
            adfgvx.InformError("암호문 최대 입력 : 입력 불가");
            return;
        }

        inputString += value;
        chiper.text = inputString;
        skipOneFlash = true;

        //입력음 재생
        adfgvx.PlayAudioSource(1);
        
        return;
    }

    public void DeleteIntermediateChiper()
    {
        if (!isReadyForInput)
            return;

        string text = inputString;
        int DeleteLength = adfgvx.currentmode == ADFGVX.mode.Decoding ? 2 : 3;
        
        if (text.Length < DeleteLength)
        {
            adfgvx.InformError("암호문 최소 입력 : 삭제 불가");
            return;
        }
        
        inputString = text.Substring(0, text.Length - DeleteLength);
        chiper.text = inputString;
        skipOneFlash = true;

        //삭제음 재생
        adfgvx.PlayAudioSource(2);

        return;
    }

    public void ClearIntermediateChiper()
    {
        chiper.text = "";
        return;
    }

    public string GetIntermediateChiper()
    {
        return inputString;
    }

    public void ClearIntermediateChiperAll()
    {
        inputString = "";
        chiperUI.text = "";
        chiperTitle.text = "";
        dateUI.text = "";
        date.text = "";
        senderUI.text = "";
        sender.text = "";
    }

    public void InitializeIntermediateChiperAll()
    {
        if (adfgvx.currentmode == ADFGVX.mode.Encoding)
        {
            partTitle.text = "암호화 디스플레이";
            if (chiperUI.text == "")
                chiperUI.text = "[보안 등급]";
            if (chiperTitle.text == "")
                chiperTitle.text = "[파일의 제목]";
            if (inputString == "")
                chiper.text = "[암호화 내용]";
            if (dateUI.text == "")
                dateUI.text = "[작성일]";
            if (date.text == "")
                date.text = "…";
            if (senderUI.text == "")
                senderUI.text = "[작성자]";
            if (sender.text == "")
                sender.text = "…";
        }
        else
        {
            partTitle.text = "복호화 디스플레이";
            if (chiperUI.text == "")
                chiperUI.text = "[보안 등급]";
            if (chiperTitle.text == "")
                chiperTitle.text = "[파일의 제목]";
            if (inputString == "")
                chiper.text = "클릭하여 입력…";
            if (dateUI.text == "")
                dateUI.text = "[작성일]";
            if (date.text == "")
                date.text = "…";
            if (senderUI.text == "")
                senderUI.text = "[작성자]";
            if (sender.text == "")
                sender.text = "…";
        }
    }
}
