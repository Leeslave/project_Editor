using Newtonsoft.Json;
using System;
using System.Collections.Generic;


/*------------------ Info Data ---------------------*/

[Serializable]
public class PeopleIndex
{
    public string name_k;
    public string name_e;
    public string job;
    public string sex;
    public string country;
    public string key;
    public string age;
    public int face;
}

public class Peoples
{
    public PeopleIndex[] PL;
}

/*------------------ News Data ---------------------*/

[Serializable]
public class News
{
    public string publishDay;
    public string Title;
    public string Date;
    public string Reporter;
    public string[] Main; /*= new string[50];*/
    public int CountM; /*= 0;*/
    public string[] Revise; /*= new string[4];*/
    public int CountR; /*= 0;*/
    public List<int> Errors; /*= new List<int>(4);*/
    public News()
    {
        Main = new string[50];
        Revise = new string[4];
        Errors = new List<int>(4);
        CountM = 0;
        CountR = 0;
    }
}

/*----------------- Docs Data --------------------------*/
[Serializable]
public class Docs
{
    public string Subject;
    public int Month;
    public int Date;
    public string Recorder;
    public List<string> RecorderTexts;
    public List<int> RecorderTextInd;
    public List<string> SubjectTexts;
    public List<string> Time_Action;
    public List<int> SubjectAns;
    public List<int> ActionAns;
}


/*------------------ Instructions Data ---------------------*/

[Serializable]
public class Instruction
{
    [JsonProperty("Info")]
    public info[] InfoInst;
    [JsonProperty("News")]
    public news[] NewsInst;
    [JsonProperty("Docs")]
    public docs[] DocsInst;
}

[Serializable]
public class info
{
    /*
     * 0 : 국적
     * 1 : 나라
     * 2 : 얼굴
     */
    public int ToDo;
    public string Target;
    public int Before;
    public int After;
}

[Serializable]
public class news
{
    /*
     * 0 : 추가
     * 1 : 삭제
     * 2 : 변경
     */
    public int ToDo;
    public int Line;        // 줄
    public string Normal;   // 이전(변경일 경우만 적을 것)
    public string Revise;   // 이후(변경일 경우에만 적을 것)
    public string Goal;     // 최종적으로 적용 되어야 하는 문장(삭제의 경우는 제외)
}

[Serializable]
public class docs
{
    /*
     * 0 : 추가
     * 1 : 삭제
     * 2 : 변경
     */
    public int ToDo;
    public int Line;
    public string Normal;
    public string Revise;
    public string Goal;
}


