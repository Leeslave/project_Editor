using System;
using System.Linq;

namespace EditorGame.AdfgvxRemake
{
    public enum LessonAction { Load, Key, ExplainTranspose, SelectChunk, PlaceColumn, PreviousTable,
        NextTable, ExplainSubstitution, ExplainRow, ExplainColumn, RecordCell, ExplainResult, Submit, Finish }

    public sealed class LessonStep
    {
        public LessonAction Action;
        public string Target, Title, Text, Progress;
        public int Index = -1;
        public bool ReadThenContinue;
    }

    // Lessons observe real puzzle state. Only actual successful actions advance the puzzle.
    // This keeps hints correct after undo, task switches, mistakes and resuming a partial task.
    public sealed class AdfgvxLesson
    {
        public bool Active = true;
        public bool TransposeExplained, SubstitutionExplained, RowExplained, ColumnExplained, ResultExplained;
        public bool FirstCharacterRecorded;
        public int FirstRecordedUnit = -1;

        public LessonStep Next(CipherSession state)
        {
            CipherTask task = state.Task;
            bool encrypt = task.Direction == CipherDirection.Encrypt;
            if (state.Submitted)
                return Step(LessonAction.Finish, "result", "해내셨어요! 암호 작업 완료!",
                    "직접 글자의 자리를 맞추고, 표에서 문자를 찾으셨어요.\n다른 과제도 같은 방법으로 풀 수 있어요!",
                    "완료", true);
            if (!state.Loaded)
                return Step(LessonAction.Load, "load", "함께 암호를 풀어 볼까요?",
                    "암호는 글자를 바꾸거나 자리를 섞은 비밀 편지예요.\n밝게 보이는 [문서 열기]를 눌러 편지를 꺼내 주세요.\n하나씩 직접 해 보면서 배워 볼게요.", "01  문서 열기");
            if (!state.KeyApplied)
                return Step(LessonAction.Key, "key", "열쇠 글자를 입력해 주세요",
                    "키는 글자의 자리를 맞추는 데 쓰는 열쇠예요.\n입력칸에  " + task.Key + "  를 영어로 적어 주세요.\n그다음 옆의 [키 적용]을 눌러 주세요. 소문자도 괜찮아요!",
                    "01  키 입력");

            bool transposeNow = !state.TranspositionComplete && (!encrypt || state.SubstitutionComplete);
            if (transposeNow)
            {
                if (!TransposeExplained)
                    return Step(LessonAction.ExplainTranspose, "transpose", "전치 = 글자의 자리 바꾸기",
                        "열은 위에서 아래로 이어진 세로줄이에요.\n키를 알파벳 순서로 세면, 각 열에 번호가 붙어요.\n1번부터 차례로 눌러 주세요. 같은 글자는 왼쪽이 먼저예요.",
                        encrypt ? "03  자리 바꾸기" : "02  자리 되찾기", true);
                int rank = state.PlacedColumns.Count;
                if (!encrypt && state.SelectedChunk != rank)
                    return Step(LessonAction.SelectChunk, "chunk:" + rank, "이번에는 " + (rank + 1) + "번 묶음이에요",
                        "밝은 암호 묶음을 눌러 주세요.\n이 묶음을 같은 번호가 붙은 열에 넣을 거예요.", "암호 묶음 선택", false, rank);
                int column = state.Order[rank];
                string verb = encrypt ? "그 열을 위에서 아래로 읽어 암호문 뒤에 붙여 드릴게요."
                    : "암호 묶음을 이 세로줄에 넣어 드릴게요.";
                return Step(LessonAction.PlaceColumn, "column:" + column,
                    (rank + 1) + "번 열을 눌러 주세요",
                    "밝은 버튼의 위쪽은 " + task.Key[column] + ", 아래쪽 번호는 " + (rank + 1) + "이에요.\n이 버튼을 눌러 주세요. " + verb +
                    "\n" + (rank == 0 ? "첫 번째 열부터 함께 해 볼까요?" : "좋아요! 앞에서 넣은 글자는 그대로 남아 있어요."),
                    "열 맞추기  " + (rank + 1) + " / " + task.Key.Length, false, column);
            }

            if (!state.SubstitutionComplete)
            {
                if (state.TableNumber != task.TableNumber)
                {
                    int forward = (task.TableNumber - state.TableNumber + 7) % 7;
                    bool next = forward <= 3;
                    return Step(next ? LessonAction.NextTable : LessonAction.PreviousTable,
                        next ? "tableNext" : "tablePrevious", "먼저 맞는 표를 골라 주세요",
                        "오늘 사용할 표는 " + task.TableNumber + "번이에요. 지금은 " + state.TableNumber + "번이 보이네요.\n밝은 [" + (next ? "다음" : "이전") + "]을 눌러 주세요.\n" + task.TableNumber + "번 표가 나올 때까지 같이 찾아볼게요.", "치환표 고르기");
                }
                if (!SubstitutionExplained)
                    return Step(LessonAction.ExplainSubstitution, "table", "치환 = 표를 보고 글자 바꾸기",
                        encrypt
                            ? "이제 평문, 즉 원래 글자를 비밀 글자 2개로 바꿀 거예요.\n표에서 원래 글자를 누르면, 왼쪽과 위쪽의 글자가\n자동으로 기록돼요. 긴 암호를 직접 적지 않아도 돼요!"
                            : "이제 섞였던 글자들이 제자리를 찾았어요.\n암호 글자 2개를 표에서 만나게 하면 원래 글자가 나와요.\n첫 번째 글자는 왼쪽, 두 번째 글자는 위쪽에서 찾아 주세요!",
                        encrypt ? "02  문자 바꾸기" : "03  원래 문자 찾기", true);

                int unit = FindPendingUnit(state);
                int cell = task.Table.IndexOf(encrypt ? task.Input[unit] : task.Result[unit]);
                string pair = task.Intermediate.Substring(unit * 2, 2);
                string position = "문자 찾기  " + (unit + 1) + " / " + state.Entries.Length;
                if (!encrypt && !RowExplained)
                    return Step(LessonAction.ExplainRow, "row:" + cell / 6,
                        "첫 글자 " + pair[0] + "는 왼쪽에서 찾아 주세요",
                        "지금 풀 암호는 " + pair[0] + " " + pair[1] + " 두 글자예요.\n첫 글자 " + pair[0] + "를 표의 왼쪽에서 찾았어요.\n이 글자에서 오른쪽으로 쭉 가는 가로줄을 기억해 주세요.", position, true);
                if (!encrypt && !ColumnExplained)
                    return Step(LessonAction.ExplainColumn, "axis:" + cell % 6,
                        "둘째 글자 " + pair[1] + "는 위쪽에서 찾아 주세요",
                        "이번에는 두 번째 글자 " + pair[1] + "를 표의 위쪽에서 찾아 주세요.\n여기서 아래로 내려오면 아까의 가로줄과 만나요.\n두 줄이 만나는 칸이 우리가 찾는 문자예요!", position, true);
                string text = encrypt
                    ? "원래 글자 " + task.Input[unit] + "를 표에서 찾았어요. 밝은 칸을 눌러 주세요.\n왼쪽의 " + AdfgvxCipher.Alphabet[cell / 6] + "와 위쪽의 " + AdfgvxCipher.Alphabet[cell % 6] + "가 한 쌍으로 기록될 거예요."
                    : "암호쌍 " + pair[0] + " " + pair[1] + "를 따라가면\n왼쪽 " + pair[0] + " 줄과 위쪽 " + pair[1] + " 줄이 만나는 칸은 " + task.Result[unit] + "예요.\n밝은 " + task.Result[unit] + " 칸을 눌러 기록해 주세요.";
                if (!string.IsNullOrEmpty(state.Entries[unit])) text += "\n괜찮아요! 이 칸만 다시 고르면 나머지는 그대로예요.";
                return Step(LessonAction.RecordCell, "cell:" + cell, "밝은 문자 칸을 눌러 주세요", text, position, false, cell);
            }
            return Step(LessonAction.Submit, "submit", "마지막으로 제출해 주세요!",
                "모든 글자를 정확히 바꾸고, 열도 모두 맞추셨어요.\n밝은 [검증 후 제출]을 눌러 마무리해 주세요!", "04  결과 확인 / 제출");
        }

        // Called only by the explanation card; it never fills in an answer or places a column.
        public void Continue(LessonAction action)
        {
            switch (action)
            {
                case LessonAction.ExplainTranspose: TransposeExplained = true; break;
                case LessonAction.ExplainSubstitution: SubstitutionExplained = true; break;
                case LessonAction.ExplainRow: RowExplained = true; break;
                case LessonAction.ExplainColumn: ColumnExplained = true; break;
                case LessonAction.ExplainResult: ResultExplained = true; break;
                case LessonAction.Finish: Active = false; break;
            }
        }

        public static int FindPendingUnit(CipherSession state)
        {
            string[] expected = state.ExpectedEntries;
            if (state.SelectedUnit >= 0 && state.SelectedUnit < expected.Length &&
                state.Entries[state.SelectedUnit] != expected[state.SelectedUnit]) return state.SelectedUnit;
            for (int i = 0; i < expected.Length; i++)
                if (state.Entries[i] != expected[i]) return i;
            return 0;
        }

        public LessonStep ResultStep(CipherSession state)
        {
            int unit = FirstRecordedUnit >= 0 ? FirstRecordedUnit
                : Array.FindIndex(state.Entries, e => !string.IsNullOrEmpty(e));
            return Step(LessonAction.ExplainResult, "result:" + Math.Max(0, unit), "좋아요! 결과 칸에 기록됐어요",
                "방금 고른 글자가 이 칸에 들어왔어요.\n다른 칸을 고치려면 그 결과 칸을 누른 뒤 표에서 다시 골라 주세요.\n이제 남은 글자도 한 칸씩 같이 찾아볼게요!", "첫 문자 성공", true);
        }

        public LessonStep Current(CipherSession state)
        {
            if (FirstCharacterRecorded && !ResultExplained && !state.Submitted &&
                state.Loaded && state.KeyApplied && state.Entries.Any(e => !string.IsNullOrEmpty(e)))
                return ResultStep(state);
            return Next(state);
        }

        private static LessonStep Step(LessonAction action, string target, string title, string text,
            string progress, bool explanation = false, int index = -1) => new LessonStep
        { Action = action, Target = target, Title = title, Text = text, Progress = progress,
            ReadThenContinue = explanation, Index = index };
    }
}
