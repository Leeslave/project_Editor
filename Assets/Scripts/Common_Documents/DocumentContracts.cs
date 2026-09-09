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
    public enum FieldOutcome { Unchanged, CorrectEdit, IncorrectEdit, UnchangedTarget, UnrequestedEdit }

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

        public ProfileValues Copy()
        {
            return new ProfileValues { Name = Name, Age = Age, Sex = Sex, Country = Country, Job = Job };
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

    // Keep mutable play state separate from the source content; DTO fields are not read-only.
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
    }

    public static class DocumentContract
    {
        // JSON deep copy includes all lists/values; no Unity objects or $type metadata.
        public static T Copy<T>(T value)
        {
            var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.None };
            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(value, settings), settings);
        }

        /// <summary>NFC comparison only: whitespace and case remain significant; null equals empty.</summary>
        public static bool SameText(string left, string right)
        {
            return string.Equals((left ?? "").Normalize(NormalizationForm.FormC),
                (right ?? "").Normalize(NormalizationForm.FormC), StringComparison.Ordinal);
        }

        /// <summary>Classifies a field without awarding reputation; unchanged unrequested fields are neutral.</summary>
        public static FieldOutcome ClassifyField(string original, string submitted, string expected, bool instructed)
        {
            if (!instructed)
                return SameText(original, submitted) ? FieldOutcome.Unchanged : FieldOutcome.UnrequestedEdit;
            // A no-op instruction must not award a successful-edit event.
            if (SameText(original, expected) && SameText(original, submitted)) return FieldOutcome.Unchanged;
            if (SameText(submitted, expected)) return FieldOutcome.CorrectEdit;
            return SameText(original, submitted) ? FieldOutcome.UnchangedTarget : FieldOutcome.IncorrectEdit;
        }

        // Length-prefixed components avoid separator collisions in persistent keys.
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

        /// <summary>Starts a new work instance only. Re-entry must restore its existing state instead.</summary>
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

        /// <summary>Validates content structure and supplied policy rules; unfinished policy values are allowed.</summary>
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
            ValidatePolicy(content.Policy);
        }

        private static bool IsValidRule(ReputationRule rule)
        {
            switch (rule.Reason)
            {
                case ReputationReason.CorrectPair:
                case ReputationReason.IncorrectPair:
                    return rule.Timing == ReputationTiming.Record && rule.Unit == ReputationUnit.Pair;
                case ReputationReason.UnauthorizedView:
                    return rule.Timing == ReputationTiming.View && rule.Unit == ReputationUnit.View;
                case ReputationReason.MissingPair:
                    return rule.Timing == ReputationTiming.InvestigationComplete && rule.Unit == ReputationUnit.Pair;
                case ReputationReason.CorrectEdit:
                case ReputationReason.IncorrectEdit:
                case ReputationReason.UnrequestedEdit:
                    return rule.Timing == ReputationTiming.Submission && rule.Unit == ReputationUnit.Field;
                case ReputationReason.UnchangedTarget:
                    return rule.Timing == ReputationTiming.Submission &&
                        (rule.Unit == ReputationUnit.Field || rule.Unit == ReputationUnit.Person);
                default:
                    return false;
            }
        }

        private static void ValidatePolicy(DocumentPolicy policy)
        {
            if (policy == null || policy.Reputation == null || policy.PrerequisiteWorkCodes == null)
                throw new ArgumentException("Policy collections are required.");
            if (!Enum.IsDefined(typeof(UnlockCombination), policy.UnlockCombination) ||
                !Enum.IsDefined(typeof(UnlockScope), policy.UnlockScope))
                throw new ArgumentException("Invalid unlock policy.");
            var rules = new HashSet<string>(StringComparer.Ordinal);
            foreach (var rule in policy.Reputation)
            {
                if (rule == null || !IsValidRule(rule)) throw new ArgumentException("Invalid reputation reason/timing/unit.");
                if (!rules.Add(Key(rule.Reason.ToString(), rule.Timing.ToString(), rule.Unit.ToString())))
                    throw new ArgumentException("Duplicate reputation rule: " + rule.Reason);
                bool reward = rule.Reason == ReputationReason.CorrectPair || rule.Reason == ReputationReason.CorrectEdit;
                if (rule.Delta.HasValue && (reward ? rule.Delta.Value < 0 : rule.Delta.Value > 0))
                    throw new ArgumentException("Reputation delta has the wrong sign: " + rule.Reason);
            }
            var codes = new HashSet<string>(StringComparer.Ordinal);
            foreach (var code in policy.PrerequisiteWorkCodes)
                if (string.IsNullOrWhiteSpace(code) || !codes.Add(code))
                    throw new ArgumentException("Empty or duplicate prerequisite work code.");
        }

        /// <summary>
        /// Requires configured reputation and (for forgery) unlock rules before runtime integration.
        /// Does not evaluate prerequisite completion or validate story branch references; services own those checks.
        /// </summary>
        public static void ValidateForExecution(DocumentAssignment content)
        {
            Validate(content);
            var policy = content.Policy;
            foreach (var rule in policy.Reputation)
                if (!rule.Delta.HasValue) throw new ArgumentException("Unconfigured reputation delta: " + rule.Reason);
            RequireRule(policy, ReputationReason.UnauthorizedView, ReputationUnit.View);
            if (content.Mode == DocumentMode.StatementCheck)
            {
                RequireRule(policy, ReputationReason.CorrectPair, ReputationUnit.Pair);
                RequireRule(policy, ReputationReason.IncorrectPair, ReputationUnit.Pair);
                RequireRule(policy, ReputationReason.MissingPair, ReputationUnit.Pair);
            }
            else
            {
                RequireRule(policy, ReputationReason.CorrectEdit, ReputationUnit.Field);
                RequireRule(policy, ReputationReason.IncorrectEdit, ReputationUnit.Field);
                RequireRule(policy, ReputationReason.UnrequestedEdit, ReputationUnit.Field);
                RequireRule(policy, ReputationReason.UnchangedTarget, ReputationUnit.Field);
                RequireRule(policy, ReputationReason.UnchangedTarget, ReputationUnit.Person);
                if (policy.UnlockCombination == UnlockCombination.Unspecified ||
                    policy.UnlockScope == UnlockScope.Unspecified || policy.PrerequisiteWorkCodes.Count == 0)
                    throw new ArgumentException("Forgery unlock policy is not configured.");
            }
        }

        private static void RequireRule(DocumentPolicy policy, ReputationReason reason, ReputationUnit unit)
        {
            // ValidatePolicy already enforces the unique legal timing for this reason/unit.
            if (!policy.Reputation.Any(r => r.Reason == reason && r.Unit == unit))
                throw new ArgumentException("Missing reputation rule: " + reason + "/" + unit);
        }
    }
}
