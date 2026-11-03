using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using EditorGame.WorkIntegration;
using EditorGame.Documents;
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
        foreach (WorkKind kind in Enum.GetValues(typeof(WorkKind)))
            foreach (WorkSecurity security in Enum.GetValues(typeof(WorkSecurity)))
                CheckWork(kind, security);
        CheckIsolation();
        for (int layout = 0; layout < 2; layout++) CheckMaze(layout);
        CheckScenes(args[0]);
        CheckDocumentTutorials(args[0]);
        CheckMazeTutorial();
        Console.WriteLine("PASS: " + checks + " checks; four work types, auth retry/cancel/isolation, maze routes, scene wiring, document/maze tutorial actions and non-mutation.");
    }

    private static void CheckDocumentTutorials(string root)
    {
        var assignments = JsonConvert.DeserializeObject<List<DocumentAssignment>>(
            File.ReadAllText(Path.Combine(root, "docs/DocumentContractExamples.json")));
        foreach (var assignment in assignments)
        {
            var state = DocumentContract.Begin(assignment, "tutorial-test-" + assignment.Id);
            var lesson = new DocumentWorkLesson();
            var open = new HashSet<string>();
            DocumentLessonStep Observe(string person = null, string statement = null, string action = null)
            {
                string before = JsonConvert.SerializeObject(state);
                var result = lesson.Current(assignment, state, open, person, statement, action);
                Check(before == JsonConvert.SerializeObject(state), "Tutorial observation must not solve or submit a document");
                return result;
            }
            Check(Observe().Id == "intro", "Document introduction appears");
            lesson.Continue("not-a-step");
            Check(Observe().Id == "intro", "Unknown continue cannot bypass introduction");
            lesson.Continue("intro");
            if (assignment.Mode == DocumentMode.StatementCheck)
            {
                string person = assignment.TargetPersonIds[0];
                var statement = assignment.Documents.First(d => d.PersonId == person && d.Kind == DocumentKind.Statement && d.DateId == assignment.TargetDateId);
                var action = assignment.Documents.First(d => d.PersonId == person && d.Kind == DocumentKind.Action && d.DateId == assignment.TargetDateId);
                Check(Observe().Id == "open:" + statement.Id, "Open actual statement first");
                Check(Observe().Id == "open:" + statement.Id, "No action means no tutorial advancement");
                open.Add(statement.Id);
                Check(Observe().Id == "open:" + action.Id, "Open matching action file");
                open.Add(action.Id);
                Check(Observe().Id == "statement", "Select a real statement");
                Check(Observe(person, statement.Lines[0].Id).Id == "action", "Wait for actual action selection");
                Check(Observe(person, statement.Lines[0].Id, action.Lines[0].Id).Id == "record", "Both real selections enable record guidance");
                open.Remove(statement.Id);
                Check(Observe(person, statement.Lines[0].Id, action.Lines[0].Id).Id == "open:" + statement.Id,
                    "Closed/minimized document gets a recovery instruction");
                open.Add(statement.Id);
                var investigation = state.Investigations.First(i => i.PersonId == person);
                investigation.Records.Add(new PairRecord { Correct = false });
                Check(Observe().Id == "complete:" + person, "Record guidance follows actual records without revealing correctness");
                investigation.Completed = true;
                if (state.Investigations.Count > 1)
                    Check(Observe().Id != "submit", "Other targets still require actual investigation");
                foreach (var item in state.Investigations) item.Completed = true;
                Check(Observe().Id == "submit", "Submit guidance after investigations");
            }
            else
            {
                var first = assignment.Edits.First();
                string key = DocumentContract.Key(first.PersonId, first.Field.ToString());
                Check(Observe().Id.StartsWith("open:"), "Forgery opens the requested profile");
                foreach (var document in assignment.Documents) open.Add(document.Id);
                Check(Observe().Id == "edit:" + key, "Guide actual instructed input");
                state.People.First(p => p.PersonId == first.PersonId).Values.Set(first.Field, "unfinished typing");
                Check(Observe().Id == "edit:" + key, "Typing does not advance before IME/end-edit commit");
                lesson.CommittedFields.Add(key);
                Check(Observe().Id != "edit:" + key, "Committed change advances without evaluating the answer");
                foreach (var edit in assignment.Edits)
                {
                    state.People.First(p => p.PersonId == edit.PersonId).Values.Set(edit.Field, edit.TargetValue);
                    lesson.CommittedFields.Add(DocumentContract.Key(edit.PersonId, edit.Field.ToString()));
                }
                Check(Observe().Id == "submit", "All committed fields lead to submission guidance");
                state.People.First(p => p.PersonId == first.PersonId).Values.Set(first.Field,
                    assignment.People.First(p => p.Id == first.PersonId).Profile.Get(first.Field));
                Check(Observe().Id == "edit:" + key, "Restoring original value recovers the pending instruction");
            }
            state.Phase = SubmissionPhase.Submitted;
            Check(Observe().Id == "result", "Submission leads to real result instructions");
            lesson.Continue("result");
            Check(Observe() == null && lesson.Finished, "Finish guide without changing work/save state");
        }
    }

    private static void CheckMazeTutorial()
    {
        var maze = new ShortMaze(0);
        var lesson = new MazeWorkLesson(maze);
        Check(lesson.Current(maze) == MazeLessonStage.Introduction, "Maze intro first");
        Check(maze.Remaining == 35 && !maze.HasToken, "Reading intro grants no time/token progress");
        lesson.IntroRead = true;
        Check(lesson.Current(maze) == MazeLessonStage.Move, "Movement requires real action");
        Check(!maze.Move(0, -1) && lesson.Current(maze) == MazeLessonStage.Move, "Wall bump does not count as movement");
        maze.Move(1, 0);
        Check(lesson.Current(maze) == MazeLessonStage.Token, "Real move advances to token");
        Walk(maze, Route(maze, maze.X, maze.Y, 'K'));
        Check(lesson.Current(maze) == MazeLessonStage.Exit && !maze.Succeeded, "Real token collection advances to exit");
        Walk(maze, Route(maze, maze.X, maze.Y, 'E'));
        Check(lesson.Current(maze) == MazeLessonStage.Approved && maze.Succeeded, "Approval only after real exit");
        Check(new MazeWorkLesson(maze) { IntroRead = true }.Current(maze) == MazeLessonStage.Approved,
            "Restarted guide respects existing progress");
    }

    private static void CheckWork(WorkKind kind, WorkSecurity security)
    {
        var task = new WorkAssignment { Id = kind + ":" + security, Kind = kind, Security = security };
        var flow = new WorkExecution();
        Check(flow.Begin(task), "Start " + task.Id);
        string token = flow.ExecutionId;
        Check(!flow.Begin(task), "Cannot replace an active work");
        if (security == WorkSecurity.Confidential)
        {
            Check(flow.Phase == ExecutionPhase.AuthenticationRequired, "Secret work requires auth");
            Check(!flow.HasAccess(task.WorkCode), "No work access before auth");
            Check(!flow.Complete(token, task.WorkCode), "Auth is not completion");
            Check(!flow.FinishAuthentication(token, true), "Cannot skip starting auth");
            Check(flow.StartAuthentication(), "Start auth");
            Check(!flow.FinishAuthentication("stale", true), "Reject stale auth callback");
            Check(flow.FinishAuthentication(token, false), "Fail auth");
            Check(!flow.IsCompleted(task.Id) && !flow.HasAccess(task.WorkCode), "Failure has no completion/access");
            Check(flow.StartAuthentication(), "Retry auth");
            Check(flow.FinishAuthentication(token, true), "Pass auth");
        }
        else
        {
            Check(flow.Phase == ExecutionPhase.Working, "Normal work bypasses auth");
            Check(!flow.StartAuthentication(), "Normal work cannot authenticate");
        }
        Check(flow.HasAccess(task.WorkCode), "Work accessible");
        Check(!flow.Complete(token, "Maze"), "Maze cannot finish a work");
        Check(!flow.Complete("stale", task.WorkCode), "Reject stale completion");
        Check(flow.Complete(token, task.WorkCode), "Submit selected work");
        Check(!flow.Complete(token, task.WorkCode), "Completion is idempotent");
        flow.Return();
        Check(!flow.Begin(task), "Completed work cannot restart");
    }

    private static void CheckIsolation()
    {
        var flow = new WorkExecution();
        var first = new WorkAssignment { Id = "A", Kind = WorkKind.Document, Security = WorkSecurity.Confidential };
        var second = new WorkAssignment { Id = "B", Kind = WorkKind.ADFGVX, Security = WorkSecurity.Confidential };
        flow.Begin(first); flow.StartAuthentication();
        string old = flow.ExecutionId;
        flow.Return();
        Check(!flow.IsCompleted(first.Id), "Cancelling auth does not complete");
        flow.Begin(second); flow.StartAuthentication();
        Check(!flow.FinishAuthentication(old, true), "Cancelled callback cannot unlock another task");
        flow.FinishAuthentication(flow.ExecutionId, true);
        flow.Return();
        Check(!flow.IsCompleted(second.Id), "Cancelling work does not complete");
        flow.Begin(first);
        Check(!flow.HasAccess(first.WorkCode), "Passed auth is not reused for another task");
        Check(flow.ExecutionId != old, "Each entry has an isolated execution ID");
    }

    private static List<(int x, int y)> Route(ShortMaze maze, int x, int y, char target, bool avoidToken = false)
    {
        var queue = new Queue<(int x, int y)>();
        var previous = new Dictionary<(int x, int y), (int x, int y)>();
        queue.Enqueue((x, y)); previous[(x, y)] = (x, y);
        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            if (maze.Cell(current.x, current.y) == target)
            {
                var route = new List<(int x, int y)>();
                for (var at = current; at != (x, y); at = previous[at]) route.Add(at);
                route.Reverse(); return route;
            }
            foreach (var delta in new[] { (1, 0), (-1, 0), (0, 1), (0, -1) })
            {
                var next = (x: current.x + delta.Item1, y: current.y + delta.Item2);
                if (next.x < 0 || next.y < 0 || next.x >= maze.Width || next.y >= maze.Height ||
                    previous.ContainsKey(next) || maze.Cell(next.x, next.y) == '#' ||
                    (avoidToken && maze.Cell(next.x, next.y) == 'K')) continue;
                previous[next] = current; queue.Enqueue(next);
            }
        }
        return null;
    }

    private static void Walk(ShortMaze maze, List<(int x, int y)> route)
    {
        Check(route != null && route.Count > 0, "Maze route must exist");
        foreach (var step in route) Check(maze.Move(step.x - maze.X, step.y - maze.Y), "Legal route step");
    }

    private static void CheckMaze(int layout)
    {
        var maze = new ShortMaze(layout);
        Check(!maze.Move(0, -1), "Walls block movement");
        Check(!maze.Move(1, 1), "No diagonal shortcuts");
        var bypass = Route(maze, maze.X, maze.Y, 'E', true);
        if (bypass != null)
        {
            Walk(maze, bypass);
            Check(!maze.Succeeded, "Exit alone does not authenticate");
        }
        Walk(maze, Route(maze, maze.X, maze.Y, 'K'));
        Check(maze.HasToken && !maze.Succeeded, "Collecting token is not success");
        Walk(maze, Route(maze, maze.X, maze.Y, 'E'));
        Check(maze.Succeeded && !maze.Failed, "Token and exit succeed");
        maze.Tick(100);
        Check(maze.Succeeded && !maze.Failed && !maze.Move(0, 1), "Success is final");
        var timed = new ShortMaze(layout);
        timed.Tick(35);
        Check(timed.Failed && !timed.Succeeded && !timed.Move(1, 0), "Timeout locks movement");
        Check(new ShortMaze(layout).Remaining == 35, "Retry gets fresh time/token");
    }

    private static void CheckScenes(string root)
    {
        string build = File.ReadAllText(Path.Combine(root, "ProjectSettings/EditorBuildSettings.asset"));
        foreach (string name in new[] { "WorkIntegration_TEST", "Document_TEST", "Maze_TEST", "ADFGVX_TEST" })
        {
            string path = Path.Combine(root, "Assets/Scenes/Minigame", name + ".unity");
            string guid = Regex.Match(File.ReadAllText(path + ".meta"), "guid: ([a-f0-9]+)").Groups[1].Value;
            Check(Regex.IsMatch(build, "/" + name + @"\.unity\s+guid: " + guid), "Build registration " + name);
            string scene = File.ReadAllText(path);
            foreach (Match match in Regex.Matches(scene, "m_Script:.*guid: ([a-f0-9]+)"))
            {
                string scriptGuid = match.Groups[1].Value;
                if (!scriptGuid.StartsWith("f1b62ba7")) continue;
                Check(Directory.EnumerateFiles(Path.Combine(root, "Assets/Scripts/WorkIntegration"), "*.meta")
                    .Any(meta => File.ReadAllText(meta).Contains("guid: " + scriptGuid)), "New scene script GUID resolves");
            }
        }
        string doc = File.ReadAllText(Path.Combine(root, "Assets/Scenes/Minigame/Document_TEST.unity"));
        Check(doc.Contains("guid: 7839a7bd27d34732b5e689802c765994") &&
            doc.Contains("guid: 991eedbbef2d486fa3cb62478aabed45"), "Document TEST uses both remake controllers");
    }
}
