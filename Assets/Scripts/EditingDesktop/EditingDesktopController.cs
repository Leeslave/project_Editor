using System;
using System.Collections.Generic;
using System.Linq;
using GameService;
using UnityEngine;
using Utility;

// 뉴스와 인물 편집 프리팹의 콘텐츠를 관리한다.
public class EditingDesktopController : MonoBehaviour
{
    public static EditingDesktopController Instance { get; private set; }
    public DesktopDragState DragState;
    public DesktopTaskList TaskList;
    public InfChange PersonDataManager;
    public TextMannager_N NewsManager;
    public DesktopFileViewer FileViewer;
    [SerializeField] private DesktopFolder _profileFolder;
    [SerializeField] private DesktopFolder _newsFolder;
    [SerializeField] private Sprite _iconSprite;
    [SerializeField] private int _stage;
    public int Month;
    public int Day;
    [HideInInspector] public List<string[]> ProfileOptions = new List<string[]>(4);
    public List<PeopleIndex> PeopleList;
    public News[] NewsList;
    public List<Instruction> InstructionList;
    [HideInInspector] public Instruction Instructions;
    private IWorkService WorkService => GameSystem.GetService<IWorkService>();
    public string WorkCode => WorkService?.CurrentWorkCode;

    // 편집 화면의 공유 참조를 초기화한다.
    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    // 현재 업무의 콘텐츠와 정답 데이터를 준비한다.
    private void Start()
    {
        if (Instance != this || string.IsNullOrWhiteSpace(WorkCode)) return;
        if (PeopleList == null || NewsList == null || InstructionList == null)
        {
            EditorLogger.LogError("Editing desktop content is missing.");
            return;
        }
        _stage = WorkService.GetStage(WorkCode);
        InitializeInfoCategories();
        CreateFolderIcons();
        PrepareInstructions();
    }

    // 씬이 종료되면 공유 참조를 해제한다.
    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    // 프로필 선택지의 순서를 기존 인물 편집 데이터와 맞춘다.
    private void InitializeInfoCategories()
    {
        ProfileOptions.Clear();
        ProfileOptions.Add(Enum.GetNames(typeof(Country)));
        ProfileOptions.Add(Enum.GetNames(typeof(Job)));
        ProfileOptions.Add(Enum.GetNames(typeof(Belonging)));
        ProfileOptions.Add(Enum.GetNames(typeof(Part)));
    }

    // 기존 폴더 목록의 인물과 뉴스 아이콘을 생성한다.
    private void CreateFolderIcons()
    {
        if (_profileFolder != null)
        {
            for (int i = 0; i < PeopleList.Count - 1; i++)
                _profileFolder.CreateIcon(PeopleList[i].EnglishName, _iconSprite, 1);
            _profileFolder.gameObject.SetActive(false);
        }
        if (_newsFolder == null) return;
        for (int i = 0; i < NewsList.Length - 1; i++)
        {
            News news = NewsList[i];
            _newsFolder.CreateIcon(news.PublicationDate, _iconSprite, 2);
            string[] lines = news.Main[0].Split('\n');
            if (lines.Length > 1) news.Main = lines.ToList();
        }
        _newsFolder.gameObject.SetActive(false);
    }

    // 현재 스테이지의 지시를 선택해 비교용 정답을 만든다.
    private void PrepareInstructions()
    {
        Instructions = InstructionList.FirstOrDefault(instruction => instruction.Stage == _stage);
        if (Instructions == null) return;
        PreparePeopleAnswers(Instructions);
        PrepareNewsAnswers(Instructions);
    }

    // 같은 인물에 대한 여러 지시를 하나의 정답 프로필에 반영한다.
    private void PreparePeopleAnswers(Instruction instruction)
    {
        var expectedPeople = new Dictionary<string, PeopleIndex>(StringComparer.Ordinal);
        instruction.Peoples = new List<PeopleIndex>();
        if (instruction.InfoInst == null) return;
        foreach (ProfileEditDirective edit in instruction.InfoInst)
        {
            if (!expectedPeople.TryGetValue(edit.Target, out PeopleIndex person))
            {
                PeopleIndex original = FindPeople(edit.Target);
                if (original == null) continue;
                person = new PeopleIndex(original);
                expectedPeople.Add(edit.Target, person);
                instruction.Peoples.Add(person);
            }
            switch (edit.ToDo)
            {
                case 0: person.Country = (Country)edit.After; break;
                case 1: person.Job = (Job)edit.After; break;
                case 2: person.Affiliation = (Belonging)edit.After; break;
                case 3: person.Division = (Part)edit.After; break;
                case 4: person.CurrentFaceIndex = edit.After; break;
            }
        }
    }

    // 뉴스의 원본을 복사한 뒤 지시된 줄의 최종 문장을 반영한다.
    private void PrepareNewsAnswers(Instruction instruction)
    {
        News source = FindNews($"{Month}/{Day:D2}");
        instruction.NewsMain = source == null ? new List<string>() : new List<string>(source.Main);
        if (source == null || instruction.NewsInst == null) return;
        foreach (NewsEditDirective edit in instruction.NewsInst)
        {
            while (instruction.NewsMain.Count <= edit.Line) instruction.NewsMain.Add("");
            instruction.NewsMain[edit.Line] = edit.Goal;
        }
    }

    public PeopleIndex FindPeople(string name) => PeopleList?.FirstOrDefault(person =>
        string.Equals(person.EnglishName, name, StringComparison.OrdinalIgnoreCase) || person.KoreanName == name);

    public News FindNews(string date) => NewsList?.FirstOrDefault(news => news.PublicationDate == date);

    public int[] EvaluateWork() => new[] { EvaluatePeople(), EvaluateNews() };

    // 각 정답 프로필을 해당 인물의 현재 프로필과 비교한다.
    private int EvaluatePeople()
    {
        int score = 0;
        foreach (PeopleIndex expected in Instructions.Peoples)
        {
            PeopleIndex actual = FindPeople(expected.EnglishName);
            score += actual == null ? -1 : expected.Evaluate(actual);
        }
        return score;
    }

    // 뉴스의 줄 수와 각 문장의 변경 결과를 비교한다.
    private int EvaluateNews()
    {
        News news = FindNews($"{Month}/{Day:D2}");
        if (news == null) return 0;
        int score = -Mathf.Abs(news.Main.Count - Instructions.NewsMain.Count);
        int count = Mathf.Min(news.Main.Count, Instructions.NewsMain.Count);
        for (int i = 0; i < count; i++)
            if (news.Main[i].TrimEnd('\n', '\r') != Instructions.NewsMain[i].TrimEnd('\n', '\r')) score--;
        return score;
    }
}
