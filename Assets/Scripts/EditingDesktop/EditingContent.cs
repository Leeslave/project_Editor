using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

// 인물 편집과 뉴스 업무에서 사용하는 콘텐츠 모델이다.

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

[Serializable]
public class PeopleIndex
{
    [JsonProperty("name_k")]
    public string KoreanName;
    [JsonProperty("name_e")]
    public string EnglishName;
    [JsonProperty("age")]
    public string Age;
    [JsonProperty("country")]
    public Country Country;
    [JsonProperty("belong")]
    public Belonging Affiliation;
    [JsonProperty("part")]
    public Part Division;
    [JsonProperty("job")]
    public Job Job;
    [JsonProperty("isMan")]
    public bool IsMale;
    public List<Sprite> Faces;
    [JsonProperty("curFace")]
    public int CurrentFaceIndex;

    public PeopleIndex() { }

    public PeopleIndex(PeopleIndex target)
    {
        KoreanName = target.KoreanName;
        EnglishName = target.EnglishName;
        Age = target.Age;
        IsMale = target.IsMale;
        Faces = target.Faces == null ? null : new List<Sprite>(target.Faces);
        Country = target.Country;
        Affiliation = target.Affiliation;
        Division = target.Division;
        Job = target.Job;
        CurrentFaceIndex = target.CurrentFaceIndex;
    }

    public int Evaluate(PeopleIndex target)
    {
        int score = 0;
        if (Country != target.Country) score--;
        if (Affiliation != target.Affiliation) score--;
        if (Division != target.Division) score--;
        if (Job != target.Job) score--;
        if (CurrentFaceIndex != target.CurrentFaceIndex) score--;
        return score;
    }
}

[Serializable]
public class News
{
    [JsonProperty("publishDay")]
    public string PublicationDate;
    public string Title;
    public string Date;
    public string Reporter;
    [Multiline(10)]
    public List<string> Main;
}

[Serializable]
public class Instruction
{

    [JsonProperty("stageInt")]
    public int Stage;
    public int Month;
    [JsonProperty("date")]
    public int Day;
    [JsonProperty("Info")]
    public ProfileEditDirective[] InfoInst;
    [JsonProperty("News")]
    public NewsEditDirective[] NewsInst;

    // 정답 저장
    public List<PeopleIndex> Peoples;
    public List<string> NewsMain;
}

[Serializable]
public class ProfileEditDirective
{
    /*
     * 0 : 국적
     * 1 : 직업
     * 2 : 부서
     * 3 : 소속
     * 4 : 얼굴
     */
    public int ToDo;
    public string Target;
    public int Before;
    public int After;
}

[Serializable]
public class NewsEditDirective
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

