using System;
using System.IO;
using System.Linq;
using GameService;
using Newtonsoft.Json;

internal static class Program
{
    private static readonly JsonSerializerSettings Settings = new() { TypeNameHandling = TypeNameHandling.All };
    private static void Check(bool result, string description)
    {
        if (!result) throw new Exception(description);
        Console.WriteLine("PASS: " + description);
    }

    private static void Main(string[] args)
    {
        string root = args.Length > 0 ? Path.GetFullPath(args[0]) : Directory.GetCurrentDirectory();
        string streaming = Path.Combine(root, "Assets", "StreamingAssets");
        DailyData ReadDay(int index) => JsonConvert.DeserializeObject<DailyData>(
            File.ReadAllText(Path.Combine(streaming, "DayData", $"dailyData{index}.json")), Settings);

        int paragraphCount = 0, files = 0;
        foreach (string file in Directory.GetFiles(Path.Combine(streaming, "ChatData"), "*.json", SearchOption.AllDirectories))
        {
            var dialogue = JsonConvert.DeserializeObject<Dialogue>(File.ReadAllText(file), Settings);
            if (dialogue.chatList != null) paragraphCount += dialogue.chatList.Count;
            files++;
        }
        for (int index = 0; index <= 13; index++)
            if (ReadDay(index).dayTimes.Length == 0) throw new Exception("Empty day: " + index);
        Check(true, $"Newtonsoft TypeNameHandling.All: {files} dialogues, {paragraphCount} paragraphs, 14 days");

        var day = new DayController();
        GameSystem.RegisterService<IDayService>(day);
        var work = new WorkManager();
        var data = ReadDay(12);
        day.Init(data);
        work.Init(data);
        Check(!work.IsWorkClear(), "Day 12 is not complete before any work");
        Check(!work.GetList().Contains("VoightKampff"), "Exam cannot be launched through morning task console");
        Check(work.GetStage("VoightKampff") == 0, "Exam stage 0 and tutorial remain configured");
        Check(!work.ClearWork("VoightKampff"), "Exam cannot be cleared before the interrogation");
        day.Time = 1;
        Check(!work.ClearWork("ADFGVX") && day.Time == 1, "First normal task does not skip the remaining Maze task");
        Check(!work.ClearWork("unknown"), "Unknown work does not advance the story");
        Check(work.ClearWork("Maze") && day.Time == 2, "Normal work completion advances to afternoon without requiring the evening exam");
        day.Time = 3;
        Check(work.GetList().Contains("VoightKampff") && !work.IsWorkClear(), "Evening exam becomes active and remains incomplete");
        Check(work.ClearWork("VoightKampff") && day.Time == 3, "Exam completion does not rewind to the afternoon");

        var location = new LocationController();
        location.OnAwake();
        location.Init(data);
        foreach (int phase in new[] { 4, 5 })
        {
            day.Time = phase;
            location.MoveLocation(new WorldVector(World.Interrogate, 0));
            Check(location.CurrentPosition.location == World.Interrogate, "Exam result phase " + phase + " supports location lookup");
        }
        var shortDay = ReadDay(2);
        day.Init(shortDay);
        location.Init(shortDay);
        Check(shortDay.dayTimes.Length == 3, "Three-phase original days still initialize correctly");
        foreach (int index in new[] { 4, 5, 6 })
        {
            day.Init(ReadDay(index));
            Check(day.Time == 1, "Day " + index + " starts at its populated morning phase");
        }

        var epilogue = ReadDay(13);
        day.Init(data);
        day.Time = 4;
        location.Init(epilogue);
        day.Init(epilogue);
        Check(location.CurrentPosition.location == World.Office3,
            "Day 12 phase 4 can transition to the shorter Day 13 schedule regardless of service init order");
        Check(epilogue.workList.Count == 0 && epilogue.startLocation.location == World.Office3 &&
            epilogue.dayTimes[0].npc.Single().chat.Single().Contains("[13-H]"),
            "Passed exam leads to the chapter-2 confrontation rather than the old QA placeholder");
    }
}
