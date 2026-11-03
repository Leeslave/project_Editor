using System.Collections.Generic;
using System.Linq;

namespace EditorGame.Documents
{
    public sealed class DocumentLessonStep
    {
        public string Id, Title, Text, Target;
        public bool ReadThenContinue;
        public DocumentLessonStep(string id, string title, string text, string target = null, bool read = false)
        { Id = id; Title = title; Text = text; Target = target; ReadThenContinue = read; }
    }

    /// <summary>실제 문서 상태만 관찰한다. 답 입력·기록·조사 완료·제출을 대신 수행하지 않는다.</summary>
    public sealed class DocumentWorkLesson
    {
        public bool IntroRead;
        public bool Finished;
        public readonly HashSet<string> CommittedFields = new HashSet<string>();

        public DocumentLessonStep Current(DocumentAssignment assignment, DocumentPlayState state,
            ISet<string> openDocuments, string selectedPerson, string statement, string action)
        {
            if (Finished) return null;
            if (state.Phase != SubmissionPhase.Working)
                return new DocumentLessonStep("result", "제출 결과 확인", "정확도와 평판을 확인하세요. 결과창의 '확인 · 업무 종료'로 목록에 돌아갑니다.", read: true);
            if (!IntroRead)
                return new DocumentLessonStep("intro", assignment.Mode == DocumentMode.StatementCheck ? "일반 문서 업무" : "기밀 문서 업무",
                    assignment.Mode == DocumentMode.StatementCheck
                        ? "같은 인물·날짜의 진술과 행동을 대조하고 모순을 기록합니다. 안내는 정답을 대신 선택하지 않습니다."
                        : "오른쪽 지시를 읽고 지정된 프로필 항목을 직접 수정합니다. 지시하지 않은 항목은 유지하세요.", read: true);
            if (assignment.Mode == DocumentMode.Forgery)
            {
                foreach (var edit in assignment.Edits)
                {
                    string key = DocumentContract.Key(edit.PersonId, edit.Field.ToString());
                    var original = assignment.People.First(p => p.Id == edit.PersonId).Profile.Get(edit.Field);
                    var submitted = state.People.First(p => p.PersonId == edit.PersonId).Values.Get(edit.Field);
                    if (DocumentContract.SameText(original, edit.TargetValue)) continue;
                    if (CommittedFields.Contains(key) && !DocumentContract.SameText(original, submitted)) continue;
                    var document = assignment.Documents.First(d => d.PersonId == edit.PersonId &&
                        d.DateId == assignment.TargetDateId && d.Kind == DocumentKind.Profile);
                    if (!openDocuments.Contains(document.Id)) return Open(document);
                    return new DocumentLessonStep("edit:" + key, "수정 지시를 직접 입력", edit.DisplayText +
                        " · 입력을 마치면 다른 곳을 클릭해 확정하세요. 오타 판정은 제출 후 확인합니다.", "profile:" + key);
                }
                return Submit("지정 항목과 원래 유지할 항목을 다시 확인한 뒤 제출하세요.");
            }
            var investigation = state.Investigations.FirstOrDefault(i => !i.Completed);
            if (investigation == null) return Submit("모든 대상의 조사가 끝났습니다. 제출하면 오판·누락과 평판을 확인할 수 있습니다.");
            if (investigation.Records.Count > 0)
                return new DocumentLessonStep("complete:" + investigation.PersonId, "필요한 기록을 확인하고 조사 완료",
                    "모순이 더 있으면 계속 기록하세요. 모두 확인했다면 오른쪽 안내를 스크롤해 대상의 '조사완료'를 누르세요.", "complete:" + investigation.PersonId);
            var statementDocument = assignment.Documents.First(d => d.PersonId == investigation.PersonId &&
                d.DateId == assignment.TargetDateId && d.Kind == DocumentKind.Statement);
            var actionDocument = assignment.Documents.First(d => d.PersonId == investigation.PersonId &&
                d.DateId == assignment.TargetDateId && d.Kind == DocumentKind.Action);
            if (!openDocuments.Contains(statementDocument.Id)) return Open(statementDocument);
            if (!openDocuments.Contains(actionDocument.Id)) return Open(actionDocument);
            if (selectedPerson != investigation.PersonId || string.IsNullOrEmpty(statement))
                return new DocumentLessonStep("statement", "진술 한 줄 선택", "제목 표시줄을 끌어 두 문서를 나란히 놓고, 진술서에서 의심되는 문장을 클릭하세요. 긴 문서는 스크롤할 수 있습니다.", "document:" + statementDocument.Id);
            if (string.IsNullOrEmpty(action))
                return new DocumentLessonStep("action", "대조할 행동 한 줄 선택", "같은 인물·날짜의 행동 기록에서 모순되는 내용을 선택하세요.", "document:" + actionDocument.Id);
            return new DocumentLessonStep("record", "선택한 모순 기록", "진술과 행동이 모순된다고 판단하면 '기록하기'를 누르세요. 정답 여부는 제출 후 공개됩니다.", "record");
        }

        private static DocumentLessonStep Open(DocumentContent document) => new DocumentLessonStep("open:" + document.Id,
            "지정 문서 열기", (document.Kind == DocumentKind.Profile ? "인물" : document.Kind == DocumentKind.Statement ? "진술 파일" : "실제 행동") +
            " 폴더에서 지정된 인물·날짜의 파일을 여세요. 최소화한 창은 작업표시줄에서 복원할 수 있습니다.", "open:" + document.Id);
        private static DocumentLessonStep Submit(string text) => new DocumentLessonStep("submit", "최종 제출", text, "submit");
        public void Continue(string stepId)
        {
            if (stepId == "intro") IntroRead = true;
            if (stepId == "result") Finished = true;
        }
    }
}
