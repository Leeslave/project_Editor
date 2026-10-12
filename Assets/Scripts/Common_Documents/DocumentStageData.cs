using System;
using System.Collections.Generic;
using UnityEngine;

namespace EditorGame.Documents
{
    [Serializable]
    public sealed class DocumentSettlementPolicy
    {
        public int Completion;
        public int IncorrectPerCount = -1;
        public int ConfidentialMission;
        public int CaughtPerCount = -1;

        public int Calculate(int incorrectCount, int caughtCount, bool confidential)
        {
            return Completion + IncorrectPerCount * incorrectCount +
                (confidential ? ConfidentialMission : 0) + CaughtPerCount * caughtCount;
        }
    }

    [Serializable]
    public sealed class WorkPrerequisite
    {
        public string WorkCode;
        [Tooltip("0 이상이면 해당 스테이지만, 음수이면 해당 업무의 모든 스테이지를 허용합니다.")]
        public int Stage = -1;
    }

    [CreateAssetMenu(fileName = "DocumentStage", menuName = "Game Data/Document Stage")]
    public sealed class DocumentStageData : ScriptableObject
    {
        public string WorkCode;
        public int Stage;
        public DocumentAssignment Assignment = new DocumentAssignment();
        public DocumentSettlementPolicy Settlement = new DocumentSettlementPolicy();
        public UnlockCombination UnlockCombination = UnlockCombination.All;
        public List<WorkPrerequisite> Prerequisites = new List<WorkPrerequisite>();

        public bool IsConfidential => WorkCode == "SecureDocument";

        public void ValidateForExecution()
        {
            if (Assignment == null || Assignment.Stage != Stage || Assignment.WorkCode != WorkCode)
                throw new InvalidOperationException("스테이지 SO와 문서 과제의 업무 코드 또는 스테이지가 일치하지 않습니다.");
            if (Prerequisites == null || Settlement == null)
                throw new InvalidOperationException("스테이지 정책 데이터가 없습니다.");
            if (Prerequisites.Count > 0 && UnlockCombination == UnlockCombination.Unspecified)
                throw new InvalidOperationException("해금 조건 조합 방식이 지정되지 않았습니다.");
            var keys = new HashSet<string>(StringComparer.Ordinal);
            foreach (WorkPrerequisite prerequisite in Prerequisites)
            {
                if (prerequisite == null || string.IsNullOrWhiteSpace(prerequisite.WorkCode) ||
                    !keys.Add(prerequisite.WorkCode + ":" + prerequisite.Stage))
                    throw new InvalidOperationException("비어 있거나 중복된 선행 업무 조건이 있습니다.");
            }
            DocumentContract.Validate(Assignment);
        }

        public bool IsUnlocked(GameService.SaveService saveService)
        {
            if (Prerequisites == null || Prerequisites.Count == 0) return true;
            if (saveService == null) return false;
            if (UnlockCombination == UnlockCombination.Any)
                return Prerequisites.Exists(item => saveService.IsWorkCompleted(item.WorkCode, item.Stage));
            return Prerequisites.TrueForAll(item => saveService.IsWorkCompleted(item.WorkCode, item.Stage));
        }
    }

    public static class DocumentStageRepository
    {
        public static DocumentStageData Load(string workCode, int stage)
        {
            return Resources.Load<DocumentStageData>($"GameData/Document/{workCode}/Stage_{stage}");
        }
    }
}
