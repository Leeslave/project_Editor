using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TranspositionPart : MonoBehaviour
{
    private ADFGVXscript ADFGVX;
    private bool selected;
    private bool isover;

    //Keyword와 Chiper의 Transpose에 사용하는 처리용 배열
    private char[] array0;
    private char[] array1;

    //전치 암호와 그 순위
    private string KeyWord;
    private int[] place;

    //입력창 깜박임
    private bool Flash;

    //전치 작업
    private bool onWork;
    private string[] interline;
    private int SubstringNum;
    private int x;
    private TextMeshPro[] lines;            //전치된 암호문이 화면에 표시되는 텍스트메쉬프로 배열
    private string transposedChiper;        //intermediateChiper에 보낼 최종 결과물을 저장해둔다

    private void Awake()
    {
        ADFGVX = GameObject.Find("ADFGVX_Script").GetComponent<ADFGVXscript>();
        KeyWord = "";
        GetComponentsInChildren<TextMeshPro>()[0].text = KeyWord;
        selected = false;
        Flash = false;
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

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            if (isover && !selected)
                selected = true;
            else if (isover && selected)
                selected = true;
            else
                selected = false;

        }
    }

    private void OnMouseEnter()
    {
        isover = true;
    }

    private void OnMouseExit()
    {
        isover = false;
    }

    IEnumerator FlashText()
    {
        if (KeyWord.Length <= 16 && selected && !onWork)
        {
            if (Flash)
            {
                GetComponentsInChildren<TextMeshPro>()[0].text = KeyWord;
                Flash = false;
            }
            else
            {
                GetComponentsInChildren<TextMeshPro>()[0].text = KeyWord + " _";
                Flash = true;
            }
        }
        else if (KeyWord == "")
            GetComponentsInChildren<TextMeshPro>()[0].text = "클릭하여 입력...";
        else if(!selected)
            GetComponentsInChildren<TextMeshPro>()[0].text = KeyWord;

 
        yield return new WaitForSeconds(0.5f);
        StartCoroutine("FlashText", 0.5f);
    }

    private void ClearText()
    {
        GetComponentsInChildren<TextMeshPro>()[0].text = "";
    }

    public void AddText(string value)
    {
        if(!selected)
            return;

        if(KeyWord.Length>16)
        {
            ADFGVX.UpdateInfoBox("암호 키 최대 입력 재확인 요망");
            ADFGVX.InformCurrentMode();
        }
        else
        {
            KeyWord = KeyWord + " " + value;
            GetComponentsInChildren<TextMeshPro>()[0].text = KeyWord;
            UpdatePriority();
        }
        
    }

    public void DeleteText()
    {
        if (!selected)
            return;

        if(KeyWord.Length<2)
        {
            ADFGVX.UpdateInfoBox("암호문 삭제 불가 재확인 요망");
            ADFGVX.InformCurrentMode();
        }
        else
        {
            KeyWord = KeyWord.Substring(0, KeyWord.Length - 2);
            GetComponentsInChildren<TextMeshPro>()[0].text = KeyWord;
        }
        
        UpdatePriority();
    }

    private void UpdatePriority()
    {
        string Priority = "";
        
        array0 = new char[KeyWord.Length];
        array1 = new char[KeyWord.Length / 2];
        place = new int[KeyWord.Length / 2];

        array0 = KeyWord.ToCharArray();

        for (int i = 0; i < KeyWord.Length; i++)
        {
            if (array0[i] != ' ')
                array1[i/2] = array0[i];
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
                    if (array1[i] > array1[j])
                        place[i]++;
                    if (array1[i] == array1[j] && i < j)
                        place[j]++;
                }
            }
        }

        for (int i = 0; i < KeyWord.Length / 2; i++)
        {
            Priority += " ";
            Priority += place[i].ToString();
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
        if(onWork==true)
        {
            ADFGVX.InfoBox.UpdateText("전치 불가: 작업 진행 중");
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
        SubstringNum = Chiper.Length / place.Length;

        for (int i = 0; i < place.Length; i++)
        {
            for (int j = 0; j < place.Length; j++)
            {
                if (place[j] == InputPriority)
                {
                    InputPriority++;
                    interline[j] = Chiper.Substring(0, SubstringNum);
                    Chiper = Chiper.Substring(SubstringNum);
                }
            }
        }

        printFlow();

        //전치 최종 결과를 문자열로 저장
        Chiper = array1.ArrayToString();
        transposedChiper = "";
        for(int i=0;i<SubstringNum;i++)
        {
            for(int j=0;j<place.Length;j++)
            {
                transposedChiper += interline[j][i].ToString();
            }
        }
    }

    public void OnTransposeReverseDown()
    {
        //에러 발생
        if (onWork == true)
        {
            ADFGVX.InfoBox.UpdateText("역전치 불가: 작업 진행 중");
            return;
        }
        else if (KeyWord.Length == 0)
        {
            ADFGVX.InfoBox.UpdateText("역전치 불가: 암호 키 공백");
            return;
        }

        string Chiper = ADFGVX.InterChiperBox.GetComponentInChildren<TextMeshPro>().text;
        int newlenght = 0;

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

        Chiper = array1.ArrayToString();
        SubstringNum = Chiper.Length / place.Length;
        Debug.Log(Chiper);

        for(int i=0;i<9;i++)
        {
            interline[i] = "";
            lines[i].text = "";
        }

        int length = Chiper.Length;
        for (int i=0;i<length;i++)
        {
            interline[i%place.Length] += Chiper.Substring(0, 1);
            Chiper = Chiper.Substring(1);
        }

        printFlow();
    }

    private void printFlow()//흐름 출력
    {
        //흐름 출력 개시
        x = 0;
        InvokeRepeating("printxf", 0.0f, 0.2f);
        //깜박임 차단, 적절한 시간 후에 깜박임 회복
        GetComponentsInChildren<TextMeshPro>()[0].text = KeyWord;
        onWork = true;
        Invoke("SetonWorkFalse", 0.2f * (SubstringNum + place.Length));
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
        if (y >= SubstringNum)
            yield break;
        lines[x].text += interline[x][y].ToString();
        yield return new WaitForSeconds(0.2f);
        StartCoroutine(printyf(x, y+1));
    }

    private void SetonWorkFalse()
    {
        onWork = false;
    }

}