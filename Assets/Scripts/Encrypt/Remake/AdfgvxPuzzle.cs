using System;
using System.Collections.Generic;
using System.Linq;

namespace EditorGame.AdfgvxRemake
{
    public enum CipherDirection { Decrypt, Encrypt }

    // No Unity dependencies: both the original JSON and future tasks use this model.
    public sealed class CipherTask
    {
        public int Stage;
        public CipherDirection Direction;
        public string Title, Input, Key, Table, Intermediate, Result;
        public int TableNumber;
        public string Id => Direction + ":" + Stage;

        public void Validate()
        {
            if (string.IsNullOrEmpty(Input) || string.IsNullOrEmpty(Key) || Key.Length > 9 ||
                Table == null || Table.Length != 36 || Table.Distinct().Count() != 36)
                throw new InvalidOperationException(Id + ": 잘못된 암호 과제입니다.");
            string intermediate = Direction == CipherDirection.Encrypt
                ? AdfgvxCipher.Substitute(Input, Table) : AdfgvxCipher.Untranspose(Input, Key);
            string result = Direction == CipherDirection.Encrypt
                ? AdfgvxCipher.Transpose(intermediate, Key) : AdfgvxCipher.Decode(intermediate, Table);
            if (intermediate != Intermediate || result != Result)
                throw new InvalidOperationException(Id + ": 기존 데이터의 중간 결과/정답이 암호 규칙과 다릅니다.");
        }
    }

    public static class AdfgvxCipher
    {
        public const string Alphabet = "ADFGVX";

        // Equal letters retain their original left-to-right order (e.g. MUNIN).
        public static int[] ColumnOrder(string key) => Enumerable.Range(0, key.Length)
            .OrderBy(i => key[i]).ThenBy(i => i).ToArray();

        public static string Substitute(string text, string table)
        {
            return string.Concat(text.Select(c =>
            {
                int cell = table.IndexOf(c);
                if (cell < 0) throw new ArgumentException("치환표에 없는 문자: " + c);
                return "" + Alphabet[cell / 6] + Alphabet[cell % 6];
            }));
        }

        public static string Decode(string pairs, string table)
        {
            if (pairs.Length % 2 != 0) throw new ArgumentException("암호쌍 길이가 홀수입니다.");
            string result = "";
            for (int i = 0; i < pairs.Length; i += 2)
            {
                int row = Alphabet.IndexOf(pairs[i]), column = Alphabet.IndexOf(pairs[i + 1]);
                if (row < 0 || column < 0) throw new ArgumentException("ADFGVX 외 문자가 있습니다.");
                result += table[row * 6 + column];
            }
            return result;
        }

        public static string Column(string text, int width, int column)
        {
            string result = "";
            for (int i = column; i < text.Length; i += width) result += text[i];
            return result;
        }

        public static string Transpose(string text, string key) =>
            string.Concat(ColumnOrder(key).Select(column => Column(text, key.Length, column)));

        public static string[] SplitCiphertext(string text, string key)
        {
            var chunks = new string[key.Length];
            int offset = 0, rank = 0;
            foreach (int column in ColumnOrder(key))
            {
                int length = text.Length / key.Length + (column < text.Length % key.Length ? 1 : 0);
                chunks[rank++] = text.Substring(offset, length);
                offset += length;
            }
            return chunks;
        }

        public static string Untranspose(string text, string key)
        {
            string[] chunks = SplitCiphertext(text, key);
            var columns = new string[key.Length];
            int[] order = ColumnOrder(key);
            for (int i = 0; i < order.Length; i++) columns[order[i]] = chunks[i];
            string result = "";
            for (int row = 0; row < (text.Length + key.Length - 1) / key.Length; row++)
                for (int column = 0; column < key.Length; column++)
                    if (row < columns[column].Length) result += columns[column][row];
            return result;
        }
    }

    // State is retained per task while the test scene is open; no production saves are touched.
    public sealed class CipherSession
    {
        public readonly CipherTask Task;
        public bool Loaded { get; private set; }
        public bool KeyApplied { get; private set; }
        public bool Submitted { get; private set; }
        public string KeyDraft;
        public int TableNumber;
        public int SelectedUnit;
        public int SelectedChunk;
        public string[] Entries { get; private set; }
        public List<int> PlacedColumns { get; private set; } = new List<int>();
        private readonly Stack<Snapshot> undo = new Stack<Snapshot>();
        private readonly Stack<Snapshot> redo = new Stack<Snapshot>();
        public bool CanUndo => undo.Count > 0 && !Submitted;
        public bool CanRedo => redo.Count > 0 && !Submitted;
        public int[] Order => AdfgvxCipher.ColumnOrder(Task.Key);
        public bool TranspositionComplete => PlacedColumns.Count == Task.Key.Length;
        public string[] ExpectedEntries => Task.Direction == CipherDirection.Encrypt
            ? Enumerable.Range(0, Task.Input.Length).Select(i => Task.Intermediate.Substring(i * 2, 2)).ToArray()
            : Task.Result.Select(c => c.ToString()).ToArray();
        public bool SubstitutionComplete => Entries.SequenceEqual(ExpectedEntries);
        public bool CanSubstitute => Loaded && KeyApplied && !Submitted &&
            (Task.Direction == CipherDirection.Encrypt || TranspositionComplete);
        public bool CanTranspose => Loaded && KeyApplied && !Submitted &&
            (Task.Direction == CipherDirection.Decrypt || SubstitutionComplete);
        public bool CanSubmit => Loaded && KeyApplied && SubstitutionComplete && TranspositionComplete && !Submitted;

        public CipherSession(CipherTask task)
        {
            Task = task;
            KeyDraft = "";
            Entries = new string[task.Direction == CipherDirection.Encrypt ? task.Input.Length : task.Result.Length];
        }

        public void Load()
        {
            if (Loaded || Submitted) return;
            Remember();
            Loaded = true;
        }

        public bool ApplyKey(string draft, out string message)
        {
            if (!Loaded || Submitted)
            { message = Submitted ? "이미 제출한 과제입니다." : "업무 문서를 먼저 불러오세요."; return false; }
            KeyDraft = (draft ?? "").Trim().ToUpperInvariant();
            if (KeyDraft != Task.Key)
            {
                message = "업무 지시의 키를 확인하세요. 작성한 내용은 유지됩니다.";
                return false;
            }
            if (!KeyApplied) { Remember(); KeyApplied = true; }
            message = "키 적용 완료. 같은 글자는 왼쪽 글자부터 순위를 매깁니다.";
            return true;
        }

        public bool RecordCell(int cell, string displayedTable, out string message)
        {
            if (!CanSubstitute || cell < 0 || cell >= 36 || displayedTable == null || displayedTable.Length != 36)
            { message = "앞 단계를 먼저 완료하세요."; return false; }
            if (TableNumber != Task.TableNumber)
            { message = "업무 지시의 치환표 " + Task.TableNumber + "번을 선택하세요."; return false; }
            int unit = SelectedUnit;
            string entry = Task.Direction == CipherDirection.Encrypt
                ? "" + AdfgvxCipher.Alphabet[cell / 6] + AdfgvxCipher.Alphabet[cell % 6]
                : displayedTable[cell].ToString();
            if (Entries[unit] == entry)
            { message = "이미 기록된 값입니다. 다른 칸을 선택해 수정할 수 있습니다."; return entry == ExpectedEntries[unit]; }
            Remember();
            Entries[unit] = entry;
            bool correct = Entries[unit] == ExpectedEntries[unit];
            if (Task.Direction == CipherDirection.Encrypt)
                PlacedColumns.Clear(); // Replacing a character invalidates any derived output.
            if (correct)
            {
                int next = Array.FindIndex(Entries, entry => string.IsNullOrEmpty(entry));
                if (next >= 0) SelectedUnit = next;
                else
                {
                    var expected = ExpectedEntries;
                    for (int i = 0; i < Entries.Length; i++)
                        if (Entries[i] != expected[i]) { SelectedUnit = i; break; }
                }
            }
            message = correct ? "기록했습니다. 수정하려면 원하는 칸을 클릭하세요."
                : (unit + 1) + "번째 칸이 치환표와 다릅니다. 해당 칸에서 다시 선택하세요.";
            return correct;
        }

        public bool PlaceColumn(int column, out string message)
        {
            if (!CanTranspose) { message = "앞 단계를 먼저 완료하세요."; return false; }
            if (PlacedColumns.Contains(column)) { message = "이미 배치한 열입니다. 되돌리기로 회수할 수 있습니다."; return false; }
            int rank = Task.Direction == CipherDirection.Decrypt ? SelectedChunk : PlacedColumns.Count;
            if (rank != PlacedColumns.Count)
            { message = "현재 대기 중인 " + (PlacedColumns.Count + 1) + "번 묶음부터 배치하세요."; return false; }
            if (column != Order[rank])
            { message = (rank + 1) + "순위 열을 선택하세요. 키의 알파벳 순서를 확인하세요."; return false; }
            Remember();
            PlacedColumns.Add(column);
            SelectedChunk = Math.Min(PlacedColumns.Count, Task.Key.Length - 1);
            message = TranspositionComplete ? "전치 완료. 가로로 읽은 암호쌍이 치환표에 연결됩니다."
                : "열을 배치했습니다. 다음 순위의 열을 선택하세요.";
            return true;
        }

        public string TranspositionOutput => Task.Direction == CipherDirection.Encrypt
            ? string.Concat(PlacedColumns.Select(c => AdfgvxCipher.Column(Task.Intermediate, Task.Key.Length, c)))
            : TranspositionComplete ? Task.Intermediate : "";

        public bool Submit(out string message)
        {
            if (!CanSubmit) { message = "전치와 문자 치환을 모두 정확히 완료해야 제출할 수 있습니다."; return false; }
            string actual = Task.Direction == CipherDirection.Encrypt ? TranspositionOutput : string.Concat(Entries);
            if (actual != Task.Result) { message = "결과가 기존 과제의 정답과 일치하지 않습니다."; return false; }
            Submitted = true;
            message = "검증 완료! 기존 데이터의 정답과 일치합니다. 테스트 결과만 보관했습니다.";
            return true;
        }

        public void Undo()
        {
            if (!CanUndo) return;
            redo.Push(Capture());
            Restore(undo.Pop());
        }

        public void Redo()
        {
            if (!CanRedo) return;
            undo.Push(Capture());
            Restore(redo.Pop());
        }

        private void Remember() { undo.Push(Capture()); redo.Clear(); }
        private Snapshot Capture() => new Snapshot
        {
            Loaded = Loaded, KeyApplied = KeyApplied, KeyDraft = KeyDraft,
            TableNumber = TableNumber, SelectedUnit = SelectedUnit, SelectedChunk = SelectedChunk,
            Entries = (string[])Entries.Clone(), Columns = new List<int>(PlacedColumns)
        };
        private void Restore(Snapshot state)
        {
            Loaded = state.Loaded; KeyApplied = state.KeyApplied; KeyDraft = state.KeyDraft;
            TableNumber = state.TableNumber; SelectedUnit = state.SelectedUnit; SelectedChunk = state.SelectedChunk;
            Entries = state.Entries; PlacedColumns = state.Columns;
        }
        private sealed class Snapshot
        {
            public bool Loaded, KeyApplied;
            public string KeyDraft;
            public int TableNumber, SelectedUnit, SelectedChunk;
            public string[] Entries;
            public List<int> Columns;
        }
    }
}
