using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace GameEditor
{
    public enum DayEditorIssueSeverity { Warning, Error }

    [Serializable]
    public sealed class DayEditorIssue
    {
        public DayEditorIssueSeverity severity;
        public string path;
        public string message;

        public DayEditorIssue(DayEditorIssueSeverity severity, string path, string message)
        {
            this.severity = severity;
            this.path = path;
            this.message = message;
        }

        public override string ToString() =>
            $"[{(severity == DayEditorIssueSeverity.Error ? "오류" : "경고")}] {path}: {message}";
    }

    public sealed class DayEditorLoadResult
    {
        public DailyData data;
        public readonly List<DayEditorIssue> issues = new();
        public bool CanEdit => data != null;
    }

    public static class DayEditorDataService
    {
        private static readonly JsonSerializerSettings JsonSettings = new()
        {
            TypeNameHandling = TypeNameHandling.All
        };

        public static string DataDirectory => Path.Combine(Application.streamingAssetsPath, "DayData");
        public static string ChatDirectory => Path.Combine(Application.streamingAssetsPath, "ChatData");

        public static IReadOnlyList<string> ListFiles()
        {
            if (!Directory.Exists(DataDirectory)) return Array.Empty<string>();
            return Directory.GetFiles(DataDirectory, "*.json", SearchOption.AllDirectories)
                .Select(x => Path.GetRelativePath(DataDirectory, x))
                .OrderBy(x => x, NaturalStringComparer.Instance)
                .ToArray();
        }

        public static bool TryNormalizeNewFileName(string input, out string fileName, out string error)
        {
            fileName = (input ?? string.Empty).Trim();
            error = null;
            if (fileName.Length == 0)
            {
                error = "파일명을 입력하세요.";
                return false;
            }
            if (fileName == "." || fileName == ".." ||
                fileName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0 ||
                fileName.Contains("/") || fileName.Contains("\\"))
            {
                error = "폴더 경로나 사용할 수 없는 문자가 포함되어 있습니다.";
                return false;
            }
            if (!fileName.EndsWith(".json", StringComparison.OrdinalIgnoreCase)) fileName += ".json";
            string normalizedName = fileName;
            if (ListFiles().Any(x => string.Equals(x, normalizedName, StringComparison.OrdinalIgnoreCase)))
            {
                error = "같은 이름의 파일이 이미 있습니다.";
                return false;
            }
            return true;
        }

        public static DayEditorLoadResult Load(string relativeName)
        {
            var result = new DayEditorLoadResult();
            string fullPath = SafePath(relativeName);
            if (fullPath == null || !File.Exists(fullPath))
            {
                result.issues.Add(new DayEditorIssue(DayEditorIssueSeverity.Error, "$", "파일을 찾을 수 없습니다."));
                return result;
            }

            try
            {
                string json = File.ReadAllText(fullPath);
                JToken root = JToken.Parse(json);
                if (root.Type != JTokenType.Object || root["days"] != null)
                {
                    result.issues.Add(new DayEditorIssue(DayEditorIssueSeverity.Error, "$",
                        "현재 단일 DailyData 형식이 아닙니다. 레거시 파일은 자동 변환하지 않습니다."));
                    return result;
                }
                result.data = JsonConvert.DeserializeObject<DailyData>(json, JsonSettings);
                if (result.data == null)
                    result.issues.Add(new DayEditorIssue(DayEditorIssueSeverity.Error, "$", "DailyData가 비어 있습니다."));
                else
                    result.issues.AddRange(DayEditorValidator.Validate(result.data, null, null, false));
            }
            catch (Exception e)
            {
                result.issues.Add(new DayEditorIssue(DayEditorIssueSeverity.Error, "$", $"JSON을 읽을 수 없습니다: {e.Message}"));
            }
            return result;
        }

        public static bool Save(string relativeName, DailyData data, out string error)
        {
            error = null;
            string fullPath = SafePath(relativeName);
            if (fullPath == null)
            {
                error = "안전하지 않은 파일 경로입니다.";
                return false;
            }
            try
            {
                Directory.CreateDirectory(DataDirectory);
                File.WriteAllText(fullPath, JsonConvert.SerializeObject(data, Formatting.Indented, JsonSettings));
                return true;
            }
            catch (Exception e)
            {
                error = e.Message;
                return false;
            }
        }

        public static DailyData CreateDefault()
        {
            var data = new DailyData
            {
                startLocation = new WorldVector(World.Street, 0) { name = "도심 시가지" },
                workList = new List<Work>(),
                dayTimes = new TimeData[4]
            };
            string[] hours = { "09", "10", "17", "20" };
            for (int i = 0; i < 4; i++)
            {
                data.dayTimes[i] = new TimeData { daytime = new DayTime(hours[i], "00") };
            }
            return data;
        }

        public static DailyData CloneAndNormalize(DailyData source)
        {
            DailyData copy = JsonConvert.DeserializeObject<DailyData>(
                JsonConvert.SerializeObject(source, JsonSettings), JsonSettings) ?? CreateDefault();
            copy.startLocation ??= new WorldVector(World.Street, 0);
            copy.workList ??= new List<Work>();
            var times = new TimeData[4];
            if (copy.dayTimes != null)
                Array.Copy(copy.dayTimes, times, Math.Min(4, copy.dayTimes.Length));
            for (int i = 0; i < times.Length; i++)
            {
                times[i] ??= new TimeData();
                times[i].npc ??= new List<ChatObjectData>();
                times[i].action ??= new List<ActionObjectData>();
                times[i].block ??= new List<WorldVector>();
                times[i].bgm ??= new List<BGMData>();
                for (int n = 0; n < times[i].npc.Count; n++)
                {
                    times[i].npc[n] ??= new ChatObjectData();
                    times[i].npc[n].positions ??= new List<WorldVector>();
                    times[i].npc[n].anchor ??= new List<Anchor>();
                    times[i].npc[n].chat ??= new List<string>();
                    times[i].npc[n].onAwake ??= new List<bool>();
                }
                for (int a = 0; a < times[i].action.Count; a++)
                {
                    times[i].action[a] ??= new ActionObjectData();
                    times[i].action[a].positions ??= new List<WorldVector>();
                    times[i].action[a].anchor ??= new List<Anchor>();
                }
            }
            copy.dayTimes = times;
            return copy;
        }

        private static string SafePath(string relativeName)
        {
            if (string.IsNullOrWhiteSpace(relativeName)) return null;
            string root = Path.GetFullPath(DataDirectory) + Path.DirectorySeparatorChar;
            string candidate = Path.GetFullPath(Path.Combine(DataDirectory, relativeName));
            return candidate.StartsWith(root, StringComparison.Ordinal) ? candidate : null;
        }
    }

    public static class DayEditorValidator
    {
        public static List<DayEditorIssue> Validate(
            DailyData data, IReadOnlyCollection<string> miniGameScenes,
            IReadOnlyList<AudioClip> bgmClips, bool includeReferences = true)
        {
            var issues = new List<DayEditorIssue>();
            void Error(string path, string message) =>
                issues.Add(new DayEditorIssue(DayEditorIssueSeverity.Error, path, message));
            void Warning(string path, string message) =>
                issues.Add(new DayEditorIssue(DayEditorIssueSeverity.Warning, path, message));

            if (data == null) { Error("$", "DailyData가 null입니다."); return issues; }
            ValidateWorld(data.startLocation, "startLocation", Error);
            if (data.workList == null) Error("workList", "목록이 null입니다.");
            else if (includeReferences && miniGameScenes != null)
                for (int i = 0; i < data.workList.Count; i++)
                    if (data.workList[i] == null) Error($"workList[{i}]", "업무가 null입니다.");
                    else if (!miniGameScenes.Contains(data.workList[i].code))
                        Warning($"workList[{i}].code", $"'{data.workList[i].code}' 씬을 찾을 수 없습니다.");

            if (data.dayTimes == null) { Error("dayTimes", "배열이 null입니다."); return issues; }
            if (data.dayTimes.Length != 4) Error("dayTimes", $"항상 4개여야 합니다(현재 {data.dayTimes.Length}개).");
            for (int i = 0; i < data.dayTimes.Length; i++)
            {
                TimeData time = data.dayTimes[i];
                string path = $"dayTimes[{i}]";
                if (time == null) { Error(path, "TimeData가 null입니다."); continue; }
                if (time.npc == null) Error(path + ".npc", "목록이 null입니다.");
                else for (int n = 0; n < time.npc.Count; n++)
                {
                    ChatObjectData npc = time.npc[n];
                    string np = $"{path}.npc[{n}]";
                    if (npc == null) { Error(np, "NPC가 null입니다."); continue; }
                    ValidatePair(npc.positions, npc.anchor, np + ".positions/anchor", Error);
                    ValidatePair(npc.chat, npc.onAwake, np + ".chat/onAwake", Error);
                    ValidatePositions(npc.positions, np + ".positions", Error);
                    if (npc.anchor != null)
                        for (int a = 0; a < npc.anchor.Count; a++)
                            if (npc.anchor[a] == null) Error($"{np}.anchor[{a}]", "Anchor가 null입니다.");
                            else if (npc.anchor[a].size < 0)
                                Error($"{np}.anchor[{a}].size", "크기는 음수일 수 없습니다.");
                            else if (Math.Abs(npc.anchor[a].size) < .0001f &&
                                     (npc.onAwake == null || npc.onAwake.Count == 0 || !npc.onAwake[0]))
                                Error($"{np}.anchor[{a}].size",
                                    "크기가 0인 숨김 NPC의 첫 Chat은 onAwake=true여야 합니다.");
                    if (includeReferences && npc.chat != null)
                        for (int c = 0; c < npc.chat.Count; c++)
                        {
                            string relative = (npc.chat[c] ?? "").TrimStart('/', '\\');
                            if (!File.Exists(Path.Combine(DayEditorDataService.ChatDirectory, relative)))
                                Warning($"{np}.chat[{c}]", "ChatData 파일을 찾을 수 없습니다.");
                        }
                }
                if (time.action == null) Error(path + ".action", "목록이 null입니다.");
                else for (int a = 0; a < time.action.Count; a++)
                {
                    ActionObjectData action = time.action[a];
                    string ap = $"{path}.action[{a}]";
                    if (action == null) { Error(ap, "Action이 null입니다."); continue; }
                    ValidatePair(action.positions, action.anchor, ap + ".positions/anchor", Error);
                    ValidatePositions(action.positions, ap + ".positions", Error);
                }
                if (time.block == null) Error(path + ".block", "목록이 null입니다.");
                else ValidatePositions(time.block, path + ".block", Error);
                if (time.bgm == null) Error(path + ".bgm", "목록이 null입니다.");
                else for (int b = 0; b < time.bgm.Count; b++)
                {
                    if ((int)time.bgm[b].location < 0 || time.bgm[b].location >= World.Max)
                        Error($"{path}.bgm[{b}].location", "유효하지 않은 World입니다.");
                    if (includeReferences && bgmClips != null &&
                        (time.bgm[b].code < 0 || time.bgm[b].code >= bgmClips.Count))
                        Warning($"{path}.bgm[{b}].code", "BGM 클립 범위를 벗어났습니다.");
                }
            }
            return issues;
        }

        private static void ValidatePair<TA, TB>(ICollection<TA> a, ICollection<TB> b, string path,
            Action<string, string> error)
        {
            if (a == null) error(path, "첫 번째 목록이 null입니다.");
            if (b == null) error(path, "두 번째 목록이 null입니다.");
            if (a != null && b != null && a.Count != b.Count)
                error(path, $"대응 개수가 다릅니다({a.Count}/{b.Count}).");
        }

        private static void ValidatePositions(IEnumerable<WorldVector> positions, string path,
            Action<string, string> error)
        {
            if (positions == null) return;
            int i = 0;
            foreach (WorldVector vector in positions) ValidateWorld(vector, $"{path}[{i++}]", error);
        }

        private static void ValidateWorld(WorldVector vector, string path, Action<string, string> error)
        {
            if (vector == null) { error(path, "WorldVector가 null입니다."); return; }
            if ((int)vector.location < 0 || vector.location >= World.Max) error(path + ".location", "유효하지 않은 World입니다.");
            if (vector.position < 0) error(path + ".position", "position은 음수일 수 없습니다.");
        }
    }

    internal sealed class NaturalStringComparer : IComparer<string>
    {
        public static readonly NaturalStringComparer Instance = new();
        private static readonly Regex Parts = new(@"\d+|\D+", RegexOptions.Compiled);
        public int Compare(string x, string y)
        {
            MatchCollection a = Parts.Matches(x ?? ""), b = Parts.Matches(y ?? "");
            for (int i = 0; i < Math.Min(a.Count, b.Count); i++)
            {
                string av = a[i].Value, bv = b[i].Value;
                int result;
                if (long.TryParse(av, out long an) && long.TryParse(bv, out long bn)) result = an.CompareTo(bn);
                else result = string.Compare(av, bv, StringComparison.OrdinalIgnoreCase);
                if (result != 0) return result;
            }
            return a.Count.CompareTo(b.Count);
        }
    }
}
