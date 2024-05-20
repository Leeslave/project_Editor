using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

// ETC Data

public enum Country
{
    아스토츠카,
    아슬라니아,
    주렌,
    리퍼블리카
}

public enum Belonging
{
    언론부,
    사상경찰부,
    Unknown
}

public enum Part
{
    정보1반,
    정보2반,
    정보3반,
    None
}

public enum Job
{
    인턴,
    대리,
    과장,
    총책,
    사령관
}
/*----------------- Answer Data --------------------*/
[Serializable]
public class AnswerIndex
{
    
    
}

/*------------------ Info Data ---------------------*/

[Serializable]
public class PeopleIndex
{
    public string name_k;
    public string name_e;
    public string age;
    public Country country;
    public Belonging belong;
    public Part part;
    public Job job;
    public bool isMan;
    public List<Sprite> Faces;
    public int curFace;

    public PeopleIndex() { }

    public PeopleIndex(PeopleIndex Target)
    {
        name_e = Target.name_e;
        country = Target.country;
        belong = Target.belong;
        part = Target.part;
        job = Target.job;
        curFace = Target.curFace;
    }

    public int Evaluate(PeopleIndex Target)
    {
        int cnt = 0;
        if (country != Target.country) cnt--;
        if (belong != Target.belong) cnt--;
        if (part != Target.part) cnt--;
        if (job != Target.job) cnt--;
        if (curFace != Target.curFace) cnt--;
        return cnt;
    }
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
    [Multiline(10)]
    public List<string> Main; /*= new string[50];*/
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
    public string test;
    public int Month, date;
    [JsonProperty("Info")]
    public info[] InfoInst;
    [JsonProperty("News")]
    public news[] NewsInst;
    [JsonProperty("Docs")]
    public docs[] DocsInst;


    // 정답 저장
    public List<PeopleIndex> Peoples;
    public List<string> NewsMain;
}

[Serializable]
public class info
{
    /*
     * 0 : 국적
     * 1 : 직업
     * 2 : 얼굴
     * 3 : 부서
     * 4 : 소속
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
    public string Name;
    public int Count;
}


