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
    public enum UnlockScope { Unspecified, SameDay, Cumulative }
    public enum ReputationReason { CorrectPair, IncorrectPair, UnauthorizedView, MissingPair, CorrectEdit, IncorrectEdit, UnchangedTarget, UnrequestedEdit }
    public enum ReputationTiming { Record, View, InvestigationComplete, Submission }
    public enum ReputationUnit { Pair, View, Field, Person }

    [Serializable]
    public sealed class DocumentAssignment
    {
        public string Id;
        public DocumentMode Mode;
        public int Stage;
        // Content date is independent of the player's progression day.
        public string TargetDateId;
        public string Instructions;
        public List<string> TargetPersonIds = new List<string>();
        public List<PersonContent> People = new List<PersonContent>();
        public List<DocumentContent> Documents = new List<DocumentContent>();
        public List<AnswerPair> Answers = new List<AnswerPair>();
        public List<EditInstruction> Edits = new List<EditInstruction>();
        public DocumentPolicy Policy = new DocumentPolicy();
        [JsonIgnore] public string WorkCode => Mode == DocumentMode.Forgery ? "SecureDocument" : "Document";
    }

    [Serializable]
    public sealed class PersonContent
    {
        public string Id;
        // Asset reference key, never a mutable portrait index or identity key.
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

    [Serializable]
    public sealed class DocumentPolicy
    {
        // No fabricated balance defaults. null means not configured, not zero.
        public List<ReputationRule> Reputation = new List<ReputationRule>();
        public UnlockCombination UnlockCombination = UnlockCombination.Unspecified;
        public UnlockScope UnlockScope = UnlockScope.Unspecified;
        public List<string> PrerequisiteWorkCodes = new List<string>();
        public int? StoryThreshold;
        public string StoryBranchId;
    }

    [Serializable]
    public sealed class ReputationRule
    {
        public ReputationReason Reason;
        public ReputationTiming Timing;
        public ReputationUnit Unit;
        public int? Delta;
    }

    // Mutable DTOs belong to the session/persistence boundary, never to content.
    [Serializable]
    public sealed class DocumentPlayState
    {
        public string WorkInstanceId;
        public string AssignmentId;
        public SubmissionPhase Phase;
        public List<PersonEditState> People = new List<PersonEditState>();
        public List<InvestigationState> Investigations = new List<InvestigationState>();
        public List<ViewRecord> Views = new List<ViewRecord>();
        public List<ReputationEvent> Events = new List<ReputationEvent>();
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
    public sealed class ReputationEvent
    {
        public string Id;
        public ReputationReason Reason;
        public ReputationTiming Timing;
        public ReputationUnit Unit;
        public string SubjectId;
        public int? Delta;
    }

    [Serializable]
    public sealed class FieldResult
    {
        public string PersonId;
        public ProfileField Field;
        public string Original;
        public string Submitted;
        public string Expected;
        public ReputationReason Outcome;
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
    }

    public static class DocumentContract
    {
        // JSON deep copy includes all lists/values; no Unity objects or $type metadata.
        public static T Copy<T>(T value)
        {
            var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.None };
            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(value, settings), settings);
        }

        public static bool SameText(string left, string right)
        {
            return string.Equals((left ?? "").Normalize(NormalizationForm.FormC),
                (right ?? "").Normalize(NormalizationForm.FormC), StringComparison.Ordinal);
        }

        // Length-prefixed components avoid separator collisions in persistent keys.
        public static string Key(params string[] parts)
        {
            var key = new StringBuilder();
            foreach (var part in parts)
            {
                if (string.IsNullOrWhiteSpace(part)) throw new ArgumentException("Key component is required.");
                key.Append(part.Length).Append(':').Append(part);
            }
            return key.ToString();
        }

        public static DocumentPlayState Begin(DocumentAssignment content, string workInstanceId)
        {
            if (content == null) throw new ArgumentNullException(nameof(content));
            Validate(content);
            Key(workInstanceId, content.Id);
            var state = new DocumentPlayState { WorkInstanceId = workInstanceId, AssignmentId = content.Id };
            foreach (var person in content.People)
                state.People.Add(new PersonEditState { PersonId = person.Id, Values = Copy(person.Profile) });
            foreach (var personId in content.TargetPersonIds)
                state.Investigations.Add(new InvestigationState { PersonId = personId });
            return state;
        }

        public static void Validate(DocumentAssignment content)
        {
            if (content == null) throw new ArgumentNullException(nameof(content));
            Key(content.Id, content.TargetDateId);
            if (!Enum.IsDefined(typeof(DocumentMode), content.Mode) || content.Stage < 0)
                throw new ArgumentException("Invalid document mode or stage.");
            if (content.People == null || content.Documents == null || content.Answers == null ||
                content.Edits == null || content.TargetPersonIds == null || content.Policy == null)
                throw new ArgumentException("Content collections and policy are required.");
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
            foreach (var document in content.Documents)
            {
                if (document == null || document.Lines == null) throw new ArgumentException("Missing document/lines.");
                add(document.Id);
                Key(document.DateId);
                if (!people.Contains(document.PersonId) || !Enum.IsDefined(typeof(DocumentKind), document.Kind))
                    throw new ArgumentException("Invalid document owner or kind.");
                foreach (var line in document.Lines)
                {
                    if (line == null) throw new ArgumentException("Missing line.");
                    add(line.Id);
                    lines.Add(line.Id, document);
                }
            }
            var pairs = new HashSet<string>(StringComparer.Ordinal);
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
            }
            var edits = new HashSet<string>(StringComparer.Ordinal);
            foreach (var edit in content.Edits)
            {
                if (edit == null) throw new ArgumentException("Missing edit instruction.");
                add(edit.Id);
                if (!targets.Contains(edit.PersonId) || !Enum.IsDefined(typeof(ProfileField), edit.Field) ||
                    edit.TargetValue == null || !edits.Add(Key(edit.PersonId, edit.Field.ToString())))
                    throw new ArgumentException("Invalid or duplicate field instruction: " + edit.Id);
            }
            foreach (var target in targets)
            {
                if (content.Mode == DocumentMode.StatementCheck && !content.Answers.Any(a => a.PersonId == target))
                    throw new ArgumentException("Statement targets require at least one answer pair.");
                if (content.Mode == DocumentMode.Forgery && !content.Edits.Any(e => e.PersonId == target))
                    throw new ArgumentException("Forgery targets require at least one edit instruction.");
            }
            if ((content.Mode == DocumentMode.StatementCheck && content.Edits.Count != 0) ||
                (content.Mode == DocumentMode.Forgery && content.Answers.Count != 0))
                throw new ArgumentException("Assignment contains instructions for the other mode.");
        }
    }
}
