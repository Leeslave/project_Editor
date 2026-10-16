using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;

namespace EditorGame.Documents
{
    public enum DocumentMode
    {
        [InspectorName("진술 대조")] StatementCheck,
        [InspectorName("극비 문서 조작")] Forgery
    }
    public enum DocumentKind
    {
        [InspectorName("진술 기록")] Statement,
        [InspectorName("실제 행동 기록")] Action,
        [InspectorName("인물 프로필")] Profile
    }
    public enum ProfileField
    {
        [InspectorName("이름")] Name,
        [InspectorName("나이")] Age,
        [InspectorName("성별")] Sex,
        [InspectorName("국적")] Country,
        [InspectorName("직업")] Job
    }
    public enum SubmissionPhase { Working, Submitted, Applied }
    public enum UnlockCombination
    {
        [InspectorName("미설정")] Unspecified,
        [InspectorName("모두 완료")] All,
        [InspectorName("하나 이상 완료")] Any
    }
    public enum FieldOutcome { Unchanged, CorrectEdit, IncorrectEdit, UnchangedTarget, UnrequestedEdit }

    [Serializable]
    public sealed class DocumentAssignment
    {
        [InspectorName("과제 ID"), Tooltip("이 과제를 구분하는 고유 ID입니다. 다른 과제와 중복되지 않게 입력합니다.")]
        public string Id;
        [InspectorName("문서 업무 모드"), Tooltip("진술 대조는 Document, 극비 문서 조작은 SecureDocument 업무 코드와 연결됩니다.")]
        public DocumentMode Mode;
        [InspectorName("과제 스테이지"), Tooltip("위쪽 스테이지 번호와 같은 값을 입력합니다.")]
        public int Stage;
        // 문서 날짜는 플레이 진행 일차와 별도다.
        [InspectorName("조사 날짜"), Tooltip("현재 과제에서 조사할 문서 날짜입니다. 플레이 진행 날짜와는 별개입니다.")]
        public string TargetDateId;
        [InspectorName("플레이어 안내문"), Tooltip("문서 화면 오른쪽 안내 영역에 표시할 업무 설명입니다.")]
        [TextArea(3, 8)]
        public string Instructions;
        [InspectorName("조사 대상 인물"), Tooltip("아래 인물 목록에서 조사할 인물을 선택합니다.")]
        public List<string> TargetPersonIds = new List<string>();
        [InspectorName("인물 목록"), Tooltip("업무에 등장하는 인물과 기본 프로필입니다. 인물 ID는 문서와 정답에서 참조합니다.")]
        public List<PersonContent> People = new List<PersonContent>();
        [InspectorName("문서 목록"), Tooltip("진술, 실제 행동, 인물 프로필 문서를 작성합니다.")]
        public List<DocumentContent> Documents = new List<DocumentContent>();
        [InspectorName("진술 대조 정답"), Tooltip("진술 대조 모드에서 모순이 되는 진술 줄과 행동 줄을 연결합니다. 극비 문서 모드에서는 비워 둡니다.")]
        public List<AnswerPair> Answers = new List<AnswerPair>();
        [InspectorName("프로필 수정 지시"), Tooltip("극비 문서 모드에서 바꿔야 할 인물 정보와 목표값을 지정합니다. 진술 대조 모드에서는 비워 둡니다.")]
        public List<EditInstruction> Edits = new List<EditInstruction>();
        [JsonIgnore] public string WorkCode => DocumentContract.GetWorkCode(Mode);
    }

    [Serializable]
    public sealed class PersonContent
    {
        [InspectorName("인물 ID"), Tooltip("문서와 정답에서 이 인물을 참조할 고유 ID입니다.")]
        public string Id;
        // 변경 가능한 배열 번호 대신 초상화 자산 키를 사용한다.
        [InspectorName("초상화 키"), Tooltip("Resources/Image 아래 초상화 이름입니다. 확장자와 경로 접두사는 입력하지 않습니다.")]
        public string PortraitKey;
        [InspectorName("기본 프로필"), Tooltip("플레이어에게 처음 보여 주는 인물 정보입니다.")]
        public ProfileValues Profile = new ProfileValues();
    }

    [Serializable]
    public sealed class ProfileValues
    {
        [InspectorName("이름"), Tooltip("문서에 표시할 인물 이름입니다.")]
        public string Name = "";
        [InspectorName("나이"), Tooltip("문서에 표시할 나이입니다. 문자열로 입력합니다.")]
        public string Age = "";
        [InspectorName("성별"), Tooltip("문서에 표시할 성별입니다.")]
        public string Sex = "";
        [InspectorName("국적"), Tooltip("문서에 표시할 국적입니다.")]
        public string Country = "";
        [InspectorName("직업"), Tooltip("문서에 표시할 직업입니다.")]
        public string Job = "";

        public ProfileValues Copy()
        {
            return new ProfileValues { Name = Name, Age = Age, Sex = Sex, Country = Country, Job = Job };
        }

        public string Get(ProfileField field)
        {
            switch (field)
            {
                case ProfileField.Name: return Name;
                case ProfileField.Age: return Age;
                case ProfileField.Sex: return Sex;
                case ProfileField.Country: return Country;
                case ProfileField.Job: return Job;
                default: throw new ArgumentOutOfRangeException(nameof(field));
            }
        }

        public void Set(ProfileField field, string value)
        {
            switch (field)
            {
                case ProfileField.Name: Name = value; break;
                case ProfileField.Age: Age = value; break;
                case ProfileField.Sex: Sex = value; break;
                case ProfileField.Country: Country = value; break;
                case ProfileField.Job: Job = value; break;
                default: throw new ArgumentOutOfRangeException(nameof(field));
            }
        }
    }

    [Serializable]
    public sealed class DocumentContent
    {
        [InspectorName("문서 ID"), Tooltip("이 문서를 구분하는 고유 ID입니다.")]
        public string Id;
        [InspectorName("작성 인물"), Tooltip("인물 목록에서 이 문서의 인물을 선택합니다.")]
        public string PersonId;
        [InspectorName("문서 날짜"), Tooltip("과제의 조사 날짜와 같은 날짜의 문서는 현재 조사 자료로 취급합니다.")]
        public string DateId;
        [InspectorName("문서 종류"), Tooltip("진술 기록, 실제 행동 기록, 인물 프로필 중 하나를 선택합니다.")]
        public DocumentKind Kind;
        [InspectorName("문서 내용 줄"), Tooltip("진술·행동 기록에 표시할 시간과 문장을 순서대로 작성합니다.")]
        public List<DocumentLine> Lines = new List<DocumentLine>();
    }

    [Serializable]
    public sealed class DocumentLine
    {
        [InspectorName("줄 ID"), Tooltip("정답에서 이 줄을 참조할 고유 ID입니다.")]
        public string Id;
        [InspectorName("시간"), Tooltip("줄 왼쪽에 표시할 시간 문자열입니다. 예: 09:15")]
        public string Time;
        [InspectorName("내용"), Tooltip("이 줄에 표시할 문장입니다.")]
        [TextArea(2, 5)]
        public string Text;
    }

    [Serializable]
    public sealed class AnswerPair
    {
        [InspectorName("정답 ID"), Tooltip("모순 한 쌍을 구분하는 고유 ID입니다.")]
        public string Id;
        [InspectorName("대상 인물"), Tooltip("조사 대상 인물을 선택합니다.")]
        public string PersonId;
        [InspectorName("진술 줄"), Tooltip("진술 기록 문서에서 모순이 되는 줄을 선택합니다.")]
        public string StatementLineId;
        [InspectorName("행동 줄"), Tooltip("실제 행동 기록 문서에서 대응하는 줄을 선택합니다.")]
        public string ActionLineId;
    }

    [Serializable]
    public sealed class EditInstruction
    {
        [InspectorName("지시 ID"), Tooltip("이 수정 지시를 구분하는 고유 ID입니다.")]
        public string Id;
        [InspectorName("대상 인물"), Tooltip("조사 대상 인물 중 수정할 인물을 선택합니다.")]
        public string PersonId;
        [InspectorName("수정할 항목"), Tooltip("이름, 나이, 성별, 국적, 직업 중 바꿀 프로필 항목입니다.")]
        public ProfileField Field;
        [InspectorName("목표 값"), Tooltip("플레이어가 입력해야 하는 정답 값입니다.")]
        public string TargetValue;
        [InspectorName("표시할 지시문"), Tooltip("문서 화면의 안내 영역에 표시할 설명입니다.")]
        [TextArea(2, 5)]
        public string DisplayText;
    }

    // 실행 상태는 원본 과제에서 복사하며 별도로 저장할 수 있다.
    [Serializable]
    public sealed class DocumentPlayState
    {
        public string WorkInstanceId;
        public string AssignmentId;
        public SubmissionPhase Phase;
        public List<PersonEditState> People = new List<PersonEditState>();
        public List<InvestigationState> Investigations = new List<InvestigationState>();
        public List<ViewRecord> Views = new List<ViewRecord>();
    }

    [Serializable]
    public sealed class PersonEditState
    {
        public string PersonId;
        public ProfileValues Values;
    }

    [Serializable]
    public sealed class InvestigationState
    {
        public string PersonId;
        public bool Completed;
        public List<PairRecord> Records = new List<PairRecord>();
        public List<string> MissingAnswerIds = new List<string>();
    }

    [Serializable]
    public sealed class PairRecord
    {
        public string Id;
        public string StatementLineId;
        public string ActionLineId;
        public string MatchedAnswerId;
        public bool Correct;
    }

    [Serializable]
    public sealed class ViewRecord
    {
        public string Id;
        public string DocumentId;
        public bool Authorized;
    }

    [Serializable]
    public sealed class FieldResult
    {
        public string PersonId;
        public ProfileField Field;
        public string Original;
        public string Submitted;
        public string Expected;
        public FieldOutcome Outcome;
    }

    [Serializable]
    public sealed class DocumentSubmission
    {
        public string WorkInstanceId;
        public string AssignmentId;
        public string WorkCode;
        public int Stage;
        public string TargetDateId;
        public bool Successful;
        public DocumentPlayState Snapshot;
        public List<FieldResult> Fields = new List<FieldResult>();
        public int IncorrectCount;
        public int CaughtCount;
        public int ReputationDelta;
    }

    public static class DocumentContract
    {
        public static string GetWorkCode(DocumentMode mode) =>
            mode == DocumentMode.Forgery ? "SecureDocument" : "Document";

        // Unity 객체와 타입 메타데이터가 없는 DTO를 JSON으로 깊은 복사한다.
        public static T Copy<T>(T value)
        {
            var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.None };
            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(value, settings), settings);
        }

        public static DocumentPlayState CopyState(DocumentPlayState value)
        {
            if (value == null) return null;
            return new DocumentPlayState
            {
                WorkInstanceId = value.WorkInstanceId,
                AssignmentId = value.AssignmentId,
                Phase = value.Phase,
                People = value.People.Select(item => new PersonEditState
                {
                    PersonId = item.PersonId,
                    Values = item.Values.Copy()
                }).ToList(),
                Investigations = value.Investigations.Select(item => new InvestigationState
                {
                    PersonId = item.PersonId,
                    Completed = item.Completed,
                    Records = item.Records.Select(record => new PairRecord
                    {
                        Id = record.Id,
                        StatementLineId = record.StatementLineId,
                        ActionLineId = record.ActionLineId,
                        MatchedAnswerId = record.MatchedAnswerId,
                        Correct = record.Correct
                    }).ToList(),
                    MissingAnswerIds = new List<string>(item.MissingAnswerIds)
                }).ToList(),
                Views = value.Views.Select(item => new ViewRecord
                {
                    Id = item.Id,
                    DocumentId = item.DocumentId,
                    Authorized = item.Authorized
                }).ToList()
            };
        }

        /// <summary>NFC만 정규화하며 공백과 대소문자는 구분하고 null은 빈 문자열로 취급한다.</summary>
        public static bool SameText(string left, string right)
        {
            return string.Equals((left ?? "").Normalize(NormalizationForm.FormC),
                (right ?? "").Normalize(NormalizationForm.FormC), StringComparison.Ordinal);
        }

        /// <summary>평판을 적용하지 않고 필드 결과만 분류한다.</summary>
        public static FieldOutcome ClassifyField(string original, string submitted, string expected, bool instructed)
        {
            if (!instructed)
                return SameText(original, submitted) ? FieldOutcome.Unchanged : FieldOutcome.UnrequestedEdit;
            // 원본과 같은 목표값은 성공 수정으로 보상하지 않는다.
            if (SameText(original, expected) && SameText(original, submitted)) return FieldOutcome.Unchanged;
            if (SameText(submitted, expected)) return FieldOutcome.CorrectEdit;
            return SameText(original, submitted) ? FieldOutcome.UnchangedTarget : FieldOutcome.IncorrectEdit;
        }

        // 길이 접두어로 영속 키 구성 요소의 구분 충돌을 막는다.
        public static string Key(params string[] parts)
        {
            if (parts == null || parts.Length == 0) throw new ArgumentException("Key components are required.");
            var key = new StringBuilder();
            foreach (var part in parts)
            {
                if (string.IsNullOrWhiteSpace(part)) throw new ArgumentException("Key component is required.");
                key.Append(part.Length).Append(':').Append(part);
            }
            return key.ToString();
        }

        /// <summary>새 업무 상태를 만든다. 재진입 상태는 런타임 세션에서 복원한다.</summary>
        public static DocumentPlayState Begin(DocumentAssignment content, string workInstanceId)
        {
            if (content == null) throw new ArgumentNullException(nameof(content));
            Validate(content);
            Key(workInstanceId, content.Id);
            var state = new DocumentPlayState { WorkInstanceId = workInstanceId, AssignmentId = content.Id };
            foreach (var person in content.People)
                state.People.Add(new PersonEditState { PersonId = person.Id, Values = person.Profile.Copy() });
            if (content.Mode == DocumentMode.StatementCheck)
                foreach (var personId in content.TargetPersonIds)
                    state.Investigations.Add(new InvestigationState { PersonId = personId });
            return state;
        }

        /// <summary>문서 콘텐츠의 ID, 참조 관계와 모드별 필수 데이터를 검사한다.</summary>
        public static void Validate(DocumentAssignment content)
        {
            if (content == null) throw new ArgumentNullException(nameof(content));
            Key(content.Id, content.TargetDateId);
            if (!Enum.IsDefined(typeof(DocumentMode), content.Mode) || content.Stage < 0)
                throw new ArgumentException("Invalid document mode or stage.");
            if (content.People == null || content.Documents == null || content.Answers == null ||
                content.Edits == null || content.TargetPersonIds == null)
                throw new ArgumentException("Content collections are required.");
            var ids = new HashSet<string>(StringComparer.Ordinal);
            Action<string> add = id =>
            {
                if (string.IsNullOrWhiteSpace(id) || !ids.Add(id))
                    throw new ArgumentException("Content IDs must be nonempty and unique: " + id);
            };
            foreach (var person in content.People)
            {
                if (person == null || person.Profile == null) throw new ArgumentException("Missing person/profile.");
                add(person.Id);
            }
            var people = new HashSet<string>(content.People.Select(p => p.Id), StringComparer.Ordinal);
            var targets = new HashSet<string>(StringComparer.Ordinal);
            foreach (var target in content.TargetPersonIds)
                if (!people.Contains(target) || !targets.Add(target))
                    throw new ArgumentException("Unknown or duplicate target: " + target);
            if (targets.Count == 0) throw new ArgumentException("At least one target is required.");
            var lines = new Dictionary<string, DocumentContent>(StringComparer.Ordinal);
            var profiles = new HashSet<string>(StringComparer.Ordinal);
            foreach (var document in content.Documents)
            {
                if (document == null || document.Lines == null) throw new ArgumentException("Missing document/lines.");
                add(document.Id);
                Key(document.DateId);
                if (!people.Contains(document.PersonId) || !Enum.IsDefined(typeof(DocumentKind), document.Kind))
                    throw new ArgumentException("Invalid document owner or kind.");
                if (document.Kind == DocumentKind.Profile && document.DateId == content.TargetDateId)
                    profiles.Add(document.PersonId);
                foreach (var line in document.Lines)
                {
                    if (line == null) throw new ArgumentException("Missing line.");
                    add(line.Id);
                    lines.Add(line.Id, document);
                }
            }
            var pairs = new HashSet<string>(StringComparer.Ordinal);
            var answerTargets = new HashSet<string>(StringComparer.Ordinal);
            foreach (var answer in content.Answers)
            {
                if (answer == null) throw new ArgumentException("Missing answer.");
                add(answer.Id);
                DocumentContent statement, action;
                if (!targets.Contains(answer.PersonId) || answer.StatementLineId == null || answer.ActionLineId == null ||
                    !lines.TryGetValue(answer.StatementLineId, out statement) || !lines.TryGetValue(answer.ActionLineId, out action) ||
                    statement.Kind != DocumentKind.Statement || action.Kind != DocumentKind.Action ||
                    statement.PersonId != answer.PersonId || action.PersonId != answer.PersonId ||
                    statement.DateId != content.TargetDateId || action.DateId != content.TargetDateId ||
                    !pairs.Add(Key(answer.StatementLineId, answer.ActionLineId)))
                    throw new ArgumentException("Invalid or duplicate answer pair: " + answer.Id);
                answerTargets.Add(answer.PersonId);
            }
            var edits = new HashSet<string>(StringComparer.Ordinal);
            var editTargets = new HashSet<string>(StringComparer.Ordinal);
            foreach (var edit in content.Edits)
            {
                if (edit == null) throw new ArgumentException("Missing edit instruction.");
                add(edit.Id);
                if (!targets.Contains(edit.PersonId) || !Enum.IsDefined(typeof(ProfileField), edit.Field) ||
                    edit.TargetValue == null || !edits.Add(Key(edit.PersonId, edit.Field.ToString())))
                    throw new ArgumentException("Invalid or duplicate field instruction: " + edit.Id);
                editTargets.Add(edit.PersonId);
            }
            foreach (var target in targets)
            {
                if (content.Mode == DocumentMode.StatementCheck && !answerTargets.Contains(target))
                    throw new ArgumentException("Statement targets require at least one answer pair.");
                if (content.Mode == DocumentMode.Forgery && !editTargets.Contains(target))
                    throw new ArgumentException("Forgery targets require at least one edit instruction.");
                if (content.Mode == DocumentMode.Forgery && !profiles.Contains(target))
                    throw new ArgumentException("Forgery targets require a profile document for the target date: " + target);
            }
            if ((content.Mode == DocumentMode.StatementCheck && content.Edits.Count != 0) ||
                (content.Mode == DocumentMode.Forgery && content.Answers.Count != 0))
                throw new ArgumentException("Assignment contains instructions for the other mode.");
        }
    }
}
