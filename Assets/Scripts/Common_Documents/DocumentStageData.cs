using System;
using System.Collections.Generic;
using UnityEngine;

namespace EditorGame.Documents
{
    [Serializable]
    public sealed class DocumentSettlementPolicy
    {
        [InspectorName("완료 명성치"), Tooltip("업무 제출을 완료했을 때 더해지는 기본 명성치입니다.")]
        public int Completion;
        [InspectorName("오답 1개당 명성치"), Tooltip("틀린 답 하나당 적용할 명성치 변화량입니다. 감점은 음수로 입력합니다.")]
        public int IncorrectPerCount = -1;
        [InspectorName("극비 임무 추가 명성치"), Tooltip("극비 문서 업무일 때만 더해지는 명성치입니다.")]
        public int ConfidentialMission;
        [InspectorName("발각 1회당 명성치"), Tooltip("발각된 횟수마다 적용할 명성치 변화량입니다. 감점은 음수로 입력합니다.")]
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
        [InspectorName("선행 업무 코드"), Tooltip("먼저 완료해야 하는 업무 코드입니다. 예: Maze, Dodge")]
        public string WorkCode;
        [InspectorName("선행 스테이지"), Tooltip("0 이상이면 그 스테이지의 완료 기록을 확인합니다. -1이면 해당 업무의 어느 스테이지든 완료 기록이 있으면 됩니다.")]
        public int Stage = -1;
    }

    [CreateAssetMenu(fileName = "DocumentStage", menuName = "Game Data/Document Stage")]
    public sealed class DocumentStageData : ScriptableObject
    {
        [InspectorName("업무 코드"), Tooltip("Document 또는 SecureDocument를 입력합니다. 아래 과제의 모드와 일치해야 합니다.")]
        public string WorkCode;
        [InspectorName("스테이지 번호"), Tooltip("0부터 시작하는 스테이지 번호입니다. 아래 과제의 스테이지 번호와 일치해야 합니다.")]
        public int Stage;
        [InspectorName("문서 과제"), Tooltip("플레이어에게 보여 줄 인물, 문서, 정답 및 편집 지시를 설정합니다.")]
        public DocumentAssignment Assignment = new DocumentAssignment();
        [InspectorName("명성치 정산"), Tooltip("업무 제출 결과에 따라 적용할 기본 보상과 감점 규칙입니다.")]
        public DocumentSettlementPolicy Settlement = new DocumentSettlementPolicy();
        [InspectorName("선행 조건 조합"), Tooltip("선행 업무가 여러 개일 때 전부 완료해야 하는지, 하나만 완료해도 되는지 선택합니다.")]
        public UnlockCombination UnlockCombination = UnlockCombination.All;
        [InspectorName("선행 업무 목록"), Tooltip("이 스테이지를 열기 전에 완료해야 할 업무입니다. 비워 두면 제한이 없습니다.")]
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
