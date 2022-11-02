using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TranspositionPart : MonoBehaviour
{
    private ADFGVXscript ADFGVX;

    private string KeyWord;
    private char[] vs;
    private char[] vs1;
    private int[] place;

    private bool Flash;
    private string FlashKeyWord;
    private bool FlashBlock;

    //OnTransposeDown
    private char[] array0;
    private char[] array1;
    string[] interline;
    int InputCharNum;
    int x;

    private TextMeshPro[] lines;

    private void Awake()
    {
        ADFGVX = GameObject.Find("ADFGVX_Script").GetComponent<ADFGVXscript>();
        KeyWord = "";
        Flash = false;
        FlashBlock = false;
        StartCoroutine("FlashText", 0.5f);
        lines = new TextMeshPro[9];
        interline = new string[9];
        for (int i=5;i<5+lines.Length;i++)
        {
            lines[i-5] = GetComponentsInChildren<TextMeshPro>()[i];
            lines[i-5].text = "";
        }
        ClearPriority();
    }

    IEnumerator FlashText()
    {
        FlashKeyWord = KeyWord + "_ ";
        if (KeyWord.Length <= 16 && !FlashBlock)
        {
            if (Flash)
            {
                GetComponentsInChildren<TextMeshPro>()[0].text = KeyWord;
                Flash = false;
            }
            else
            {
                GetComponentsInChildren<TextMeshPro>()[0].text = FlashKeyWord;
                Flash = true;
            }
        }
        yield return new WaitForSeconds(0.5f);
        StartCoroutine("FlashText", 0.5f);
    }

    private void ClearText()
    {
        GetComponentsInChildren<TextMeshPro>()[0].text = "";
    }

    public void AddText(string value)
    {
        if(KeyWord.Length <= 16)
        {
            KeyWord = KeyWord + value + " ";
            GetComponentsInChildren<TextMeshPro>()[0].text = KeyWord;
            UpdatePriority();
        }
        else
        {
            ADFGVX.UpdateInfoBox("암호 키 추가 입력 불가 재확인 요망");
            ADFGVX.InformCurrentMode();
        }
    }

    public void DeleteText()
    {
        if(KeyWord.Length >= 2)
        {
            KeyWord = KeyWord.Substring(0, KeyWord.Length - 2);
            GetComponentsInChildren<TextMeshPro>()[0].text = KeyWord;
        }
        else
        {
            ADFGVX.UpdateInfoBox("암호문 삭제 불가 재확인 요망");
            ADFGVX.InformCurrentMode();
        }
        UpdatePriority();
    }

    private void UpdatePriority()
    {
        string Priority = "";
        
        vs = new char[KeyWord.Length];
        vs1 = new char[KeyWord.Length / 2];
        place = new int[KeyWord.Length / 2];

        vs = KeyWord.ToCharArray();

        for (int i = 0; i < KeyWord.Length; i++)
        {
            if (vs[i] != ' ')
                vs1[i/2] = vs[i];
        }

        for(int i = 0; i < KeyWord.Length / 2; i++)
        {
            place[i] = 1;
        }

        for (int i = 0; i < KeyWord.Length / 2; i++)
        {
            for (int j = 0; j < KeyWord.Length / 2; j++)
            {
                if(i!=j)
                {
                    if (vs1[i] > vs1[j])
                        place[i]++;
                    if (vs1[i] == vs1[j] && i < j)
                        place[j]++;
                }
            }
        }

        for (int i = 0; i < KeyWord.Length / 2; i++)
        {
            Priority += place[i].ToString();
            Priority += " ";
        }

        GetComponentsInChildren<TextMeshPro>()[1].text = Priority;
    }

    private void ClearPriority()
    {
        GetComponentsInChildren<TextMeshPro>()[1].text = "";
    }

    public void OnTransposeDown()//전치 실행
    {
        //에러 발생
        if(FlashBlock==true)
        {
            ADFGVX.InfoBox.UpdateText("전치 불가: 전치 작업 진행 중");
            return;
        }
        else if(KeyWord.Length==0)
        {
            ADFGVX.InfoBox.UpdateText("전치 불가: 암호 키 공백");
            return;
        }

        string Chiper = GameObject.Find("EncodedChipertext").GetComponentsInChildren<TextMeshPro>()[1].text;
        int newlenght = 0;

        for(int i=0;i<lines.Length;i++)
        {
            lines[i].text = "";
        }

        array0 = new char[Chiper.Length];        
        array0 = Chiper.ToCharArray();
        
        //array0에 들어있는 알파벳 개수 확인
        for (int i = 0; i < Chiper.Length; i++)
        {
            if (array0[i] >= 'A' && array0[i] <= 'Z')
                newlenght++;
        }
        
        //알파벳 개수 만큼 할당
        array1 = new char[newlenght];
        int idx = 0;
        
        //array01에 알파벳 전부 저장
        for (int i = 0; i < Chiper.Length; i++)
        {
            if (array0[i] >= 'A' && array0[i] <= 'Z')
            {
                array1[idx] = array0[i];
                idx++;
            }
        }

        //array01을 Chiper에 문자열로 저장
        Chiper = array1.ArrayToString();
        int InputPriority = 1;
        InputCharNum = Chiper.Length / place.Length;

        for (int i = 0; i < place.Length; i++)
        {
            for (int j = 0; j < place.Length; j++)
            {
                if (place[j] == InputPriority)
                {
                    InputPriority++;
                    interline[j] = Chiper.Substring(0, InputCharNum);
                    Chiper = Chiper.Substring(InputCharNum);
                }
            }
        }

        //흐름 출력 개시
        x = 0;
        InvokeRepeating("printxf", 0.0f, 0.2f);

        //깜박임 차단, 적절한 시간 후에 깜박임 회복
        GetComponentsInChildren<TextMeshPro>()[0].text = KeyWord;
        FlashBlock = true;
        Invoke("SetFlashBlockFalse", 0.2f * (InputCharNum + place.Length));
    }

    private void printxf()//흐름 출력 오른쪽
    {
        if (x >= place.Length)
        {
            CancelInvoke("printxf");
            return;
        }
        StartCoroutine(printyf(x, 0));
        x++;
    }

    private IEnumerator printyf(int x, int y)//흐름 출력 아래쪽
    {
        if (y >= InputCharNum)
            yield break;
        lines[x].text += interline[x][y].ToString();
        yield return new WaitForSeconds(0.2f);
        StartCoroutine(printyf(x, y+1));
    }

    private void SetFlashBlockFalse()//깜박임 활성
    {
        FlashBlock = false;
    }
}