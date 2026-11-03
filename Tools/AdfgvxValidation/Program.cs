using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using EditorGame.AdfgvxRemake;
using Newtonsoft.Json;

internal static class Program
{
    private static int checks;
    private static void Check(bool condition, string message)
    {
        checks++;
        if (!condition) throw new Exception(message);
    }

    private static void Main(string[] args)
    {
        string resources = Path.Combine(args[0], "Assets", "Resources", "GameData", "Encrypt");
        var data = JsonConvert.DeserializeObject<ADFGVXStageData>(File.ReadAllText(Path.Combine(resources, "ADFGVXStageData.json")));
        var tasks = new List<CipherTask>();
        foreach (CipherDirection direction in Enum.GetValues(typeof(CipherDirection)))
        {
            string prefix = direction == CipherDirection.Encrypt ? "encrypt" : "decrypt";
            var records = direction == CipherDirection.Encrypt ? data.Encrypt : data.Decrypt;
            foreach (var record in records)
            {
                var v = record.Value;
                var task = new CipherTask
                {
                    Stage = int.Parse(record.Key), Direction = direction,
                    Title = v.ContainsKey("title") ? v["title"] : record.Key,
                    Input = v["targetText"], Key = v[prefix + "Key"],
                    Intermediate = v[prefix + "TransposeText"], Result = v["resultText"],
                    TableNumber = int.Parse(v[prefix + "Table"]),
                    Table = File.ReadAllText(Path.Combine(resources, "Tables", "Table_" + v[prefix + "Table"] + ".txt")).Trim('\uFEFF', '\r', '\n')
                };
                task.Validate();
                tasks.Add(task);
                PlayTask(task);
                PlayLesson(task);
            }
        }
        Check(tasks.Count == 5, "Expected all five existing tasks.");
        CheckSpotlightLayout();
        Check(AdfgvxCipher.ColumnOrder("MUNIN").SequenceEqual(new[] { 3, 0, 2, 4, 1 }), "Duplicate-key stable order.");
        foreach (string key in new[] { "WAIT", "MUNIN", "AAAA", "FRAKTIYA" })
            for (int length = 1; length < 100; length++)
            {
                string text = string.Concat(Enumerable.Range(0, length).Select(i => "ADFGVX"[i % 6]));
                Check(AdfgvxCipher.Untranspose(AdfgvxCipher.Transpose(text, key), key) == text,
                    "Uneven-column round trip " + key + "/" + length);
            }
        var a = new CipherSession(tasks[0]);
        var b = new CipherSession(tasks[1]);
        a.Load(); a.ApplyKey(a.Task.Key, out _);
        Check(!b.Loaded && !b.KeyApplied, "Tasks must have isolated progress.");
        Console.WriteLine("PASS: " + checks + " checks; 5 existing tasks, click flows, errors, undo/redo, duplicate keys, uneven columns.");
    }

    private static void PlayLesson(CipherTask task)
    {
        var state = new CipherSession(task);
        var lesson = new AdfgvxLesson();
        int actions = 0;
        bool sawRow = false, sawColumn = false, sawResult = false;
        while (lesson.Active)
        {
            Check(++actions < 250, "Tutorial must reach completion without a loop: " + task.Id);
            string before = Fingerprint(state);
            LessonStep step = lesson.Current(state);
            Check(before == Fingerprint(state), "Reading a tutorial step must not solve the puzzle");
            Check(!string.IsNullOrEmpty(step.Title) && !string.IsNullOrEmpty(step.Text) &&
                !string.IsNullOrEmpty(step.Target), "Every tutorial action needs explanation and focus");
            if (step.ReadThenContinue)
            {
                if (step.Action == LessonAction.ExplainRow) sawRow = true;
                if (step.Action == LessonAction.ExplainColumn) sawColumn = true;
                if (step.Action == LessonAction.ExplainResult) sawResult = true;
                lesson.Continue(step.Action);
                Check(before == Fingerprint(state), "Explanation button must not enter answers");
                continue;
            }
            switch (step.Action)
            {
                case LessonAction.Load: state.Load(); break;
                case LessonAction.Key:
                    state.ApplyKey("WRONG", out _);
                    Check(lesson.Current(state).Action == LessonAction.Key, "Mistyped key keeps learner in the key step");
                    Check(state.ApplyKey(task.Key.ToLowerInvariant(), out _), "Follow key instruction");
                    break;
                case LessonAction.SelectChunk: state.SelectedChunk = step.Index; break;
                case LessonAction.PlaceColumn:
                    Check(step.Target == "column:" + step.Index && state.PlaceColumn(step.Index, out _), "Follow highlighted column");
                    break;
                case LessonAction.NextTable: state.TableNumber = (state.TableNumber + 1) % 7; break;
                case LessonAction.PreviousTable: state.TableNumber = (state.TableNumber + 6) % 7; break;
                case LessonAction.RecordCell:
                    state.SelectedUnit = AdfgvxLesson.FindPendingUnit(state);
                    Check(state.RecordCell(step.Index, task.Table, out _), "Following the highlighted cell must be correct");
                    lesson.FirstCharacterRecorded = true;
                    break;
                case LessonAction.Submit: Check(state.Submit(out _), "Following lesson must allow submission"); break;
                default: throw new Exception("Unsupported interactive lesson step: " + step.Action);
            }
        }
        Check(state.Submitted && sawResult, "Tutorial finishes only after real submission and result explanation");
        if (task.Direction == CipherDirection.Decrypt)
            Check(sawRow && sawColumn, "Decrypt teaches left-row and top-column separately");

        // A stopped lesson can observe a partially solved puzzle without deleting progress.
        var resumed = new CipherSession(task);
        resumed.Load(); resumed.ApplyKey(task.Key, out _);
        if (task.Direction == CipherDirection.Decrypt)
        {
            resumed.SelectedChunk = task.Key.Length - 1;
            var hint = new AdfgvxLesson { TransposeExplained = true }.Current(resumed);
            Check(hint.Action == LessonAction.SelectChunk && hint.Index == 0, "Resume repairs a wrong selected chunk without resetting columns");
        }
        else
        {
            resumed.TableNumber = task.TableNumber;
            resumed.SelectedUnit = 2;
            resumed.RecordCell(task.Table.IndexOf(task.Input[2]), task.Table, out _);
            string preserved = Fingerprint(resumed);
            var restarted = new AdfgvxLesson().Current(resumed);
            Check(preserved == Fingerprint(resumed), "Restarting guidance preserves the existing answer");
        }
        Console.WriteLine("PASS: Guided " + task.Id + " completed in " + actions + " learner actions");
    }

    private static string Fingerprint(CipherSession state) => state.Loaded + ":" + state.KeyApplied + ":" +
        state.Submitted + ":" + state.KeyDraft + ":" + state.TableNumber + ":" + state.SelectedUnit + ":" +
        state.SelectedChunk + ":" + string.Join("|", state.Entries) + ":" + string.Join(",", state.PlacedColumns);

    private static void CheckSpotlightLayout()
    {
        var holes = new List<UnityEngine.Rect>
        {
            new UnityEngine.Rect(8, 137, 338, 837), // whole transposition section
            new UnityEngine.Rect(346, 137, 476, 837), // whole substitution section
            new UnityEngine.Rect(822, 137, 610, 406), // result section
            new UnityEngine.Rect(1244, 602, 168, 56), // document button
            new UnityEngine.Rect(842, 695, 450, 54), // key entry and apply button
            new UnityEngine.Rect(1166, 471, 246, 56) // submit
        };
        for (int row = 0; row < 6; row++)
        {
            holes.Add(new UnityEngine.Rect(392, 327 + row * 60, 377, 72));
            holes.Add(new UnityEngine.Rect(444 + row * 52, 267, 65, 432));
            for (int column = 0; column < 6; column++)
                holes.Add(new UnityEngine.Rect(444 + column * 52, 327 + row * 60, 65, 72));
        }
        foreach (UnityEngine.Rect hole in holes)
        {
            UnityEngine.Vector2 point = AdfgvxTestController.LessonCardPosition(hole, 560, 360);
            var card = new UnityEngine.Rect(point.x, point.y, 560, 360);
            Check(card.xMin >= 0 && card.yMin >= 0 && card.xMax <= 1440 && card.yMax <= 1080,
                "Tutorial explanation must stay inside the screen");
            Check(!card.Overlaps(hole), "Tutorial explanation must not cover the highlighted target");
        }
        Console.WriteLine("PASS: " + holes.Count + " spotlight/card positions fit without overlapping the target");
    }

    private static void PlayTask(CipherTask task)
    {
        var state = new CipherSession(task);
        Check(!state.Submit(out _), task.Id + " premature submit");
        Check(!state.PlaceColumn(0, out _), "Unloaded transposition rejected");
        Check(!state.RecordCell(0, task.Table, out _), "Unloaded substitution rejected");
        Check(!state.ApplyKey(task.Key, out _), "Unloaded key rejected");
        state.Load();
        Check(!state.ApplyKey("WRONG", out _), "Wrong key rejected");
        Check(state.ApplyKey(" " + task.Key.ToLowerInvariant() + " ", out _), "Key normalization");
        if (task.Direction == CipherDirection.Decrypt)
        {
            Check(!state.CanSubstitute, "Decrypt must transpose before substitution");
            Transpose(state);
        }
        else Check(!state.CanTranspose, "Encrypt must substitute before transposition");

        state.TableNumber = (task.TableNumber + 1) % 7;
        Check(!state.RecordCell(0, task.Table, out _) && state.Entries[0] == null, "Wrong table must retain entries");
        state.TableNumber = task.TableNumber;
        for (int i = 0; i < state.Entries.Length; i++)
        {
            state.SelectedUnit = i;
            int cell = task.Direction == CipherDirection.Encrypt ? task.Table.IndexOf(task.Input[i])
                : task.Table.IndexOf(task.Result[i]);
            Check(state.RecordCell(cell, task.Table, out _), task.Id + " correct cell " + i);
        }
        Check(state.SubstitutionComplete, "Completed substitution");
        state.SelectedUnit = 0;
        string[] before = (string[])state.Entries.Clone();
        int goodCell = task.Table.IndexOf(task.Direction == CipherDirection.Encrypt ? task.Input[0] : task.Result[0]);
        Check(!state.RecordCell((goodCell + 1) % 36, task.Table, out _), "Wrong cell gets local error");
        Check(state.Entries.Skip(1).SequenceEqual(before.Skip(1)), "Editing first cell preserves later cells");
        Check(!state.SubstitutionComplete && !state.CanSubmit, "Error prevents submit");
        state.Undo();
        Check(state.Entries.SequenceEqual(before), "Undo restores one-cell edit");
        state.Redo();
        Check(!state.SubstitutionComplete, "Redo restores error");
        state.Undo();
        Check(state.SubstitutionComplete, "Correct state recovered");
        if (task.Direction == CipherDirection.Encrypt)
        {
            Transpose(state);
            int count = state.PlacedColumns.Count;
            state.SelectedUnit = 0;
            Check(state.RecordCell(goodCell, task.Table, out _), "Re-recording identical entry is safe");
            Check(state.PlacedColumns.Count == count, "No-op edit preserves transposition");
            state.RecordCell((goodCell + 1) % 36, task.Table, out _);
            Check(!state.TranspositionComplete, "Changed substitution invalidates derived encryption output");
            state.Undo();
            Check(state.TranspositionComplete && state.SubstitutionComplete, "Undo restores dependent state");
        }
        Check(state.CanSubmit, "Ready to submit " + task.Id);
        Check(state.Submit(out _), "Existing answer matches " + task.Id);
        Check(!state.CanUndo && !state.Submit(out _), "Submitted task is immutable/idempotent");
        Check(!state.RecordCell(goodCell, task.Table, out _), "Submitted entries locked");
        string draft = state.KeyDraft;
        Check(!state.ApplyKey("WRONG", out _) && state.KeyDraft == draft, "Submitted key locked");
        Console.WriteLine("PASS: " + task.Id + " / " + task.Input.Length + " source characters");
    }

    private static void Transpose(CipherSession state)
    {
        int[] order = state.Order;
        Check(!state.PlaceColumn(order[1], out _), "Out-of-order column rejected without reset");
        Check(state.PlacedColumns.Count == 0, "Wrong column leaves state intact");
        for (int i = 0; i < order.Length; i++)
            Check(state.PlaceColumn(order[i], out _), "Column placed " + i);
        Check(state.TranspositionComplete, "Transposition complete");
        state.Undo();
        Check(!state.TranspositionComplete, "Undo column");
        state.Redo();
        Check(state.TranspositionComplete, "Redo column");
        Check(state.TranspositionOutput == (state.Task.Direction == CipherDirection.Encrypt
            ? state.Task.Result : state.Task.Intermediate), "Transposition matches existing task data");
    }
}
