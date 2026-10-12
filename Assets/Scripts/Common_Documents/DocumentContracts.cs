using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace EditorGame.Documents
{
    public enum DocumentMode { StatementCheck, Forgery }
    public enum DocumentKind { Statement, Action, Profile }
    public enum ProfileField { Name, Age, Sex, Country, Job }
    public enum SubmissionPhase { Working, Submitted, Applied }
    public enum UnlockCombination { Unspecified, All, Any }
    public enum FieldOutcome { Unchanged, CorrectEdit, IncorrectEdit, UnchangedTarget, UnrequestedEdit }

    [Serializable]
    public sealed class DocumentAssignment
    {
        public string Id;
        public DocumentMode Mode;
        public int Stage;
        // 문서 날짜는 플레이 진행 일차와 별도다.
        public string TargetDateId;
        public string Instructions;
        public List<string> TargetPersonIds = new List<string>();
        public List<PersonContent> People = new List<PersonContent>();
        public List<DocumentContent> Documents = new List<DocumentContent>();
        public List<AnswerPair> Answers = new List<AnswerPair>();
        public List<EditInstruction> Edits = new List<EditInstruction>();
        [JsonIgnore] public string WorkCode => Mode == DocumentMode.Forgery ? "SecureDocument" : "Document";
    }

    [Serializable]
    public sealed class PersonContent
    {
        public string Id;
        // 변경 가능한 배열 번호 대신 초상화 자산 키를 사용한다.
        public string PortraitKey;
        public ProfileValues Profile = new ProfileValues();
    }

    [Serializable]
    public sealed class ProfileValues
    {
        public string Name = "";
        public string Age = "";
        public string Sex = "";
        public string Country = "";
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
        public string Id;
        public string PersonId;
        public string DateId;
        public DocumentKind Kind;
        public List<DocumentLine> Lines = new List<DocumentLine>();
    }

    [Serializable]
    public sealed class DocumentLine
    {
        public string Id;
        public string Time;
        public string Text;
    }

    [Serializable]
    public sealed class AnswerPair
    {
        public string Id;
        public string PersonId;
        public string StatementLineId;
        public string ActionLineId;
    }

    [Serializable]
    public sealed class EditInstruction
    {
        public string Id;
        public string PersonId;
        public ProfileField Field;
        public string TargetValue;
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
