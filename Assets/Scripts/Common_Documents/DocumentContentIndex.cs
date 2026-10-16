using System;
using System.Collections.Generic;

namespace EditorGame.Documents
{
    // 과제 콘텐츠가 유지되는 동안 ID 조회용 사전을 재사용한다.
    internal sealed class DocumentContentIndex
    {
        internal readonly Dictionary<string, PersonContent> PeopleById =
            new Dictionary<string, PersonContent>(StringComparer.Ordinal);
        internal readonly Dictionary<string, DocumentContent> DocumentsById =
            new Dictionary<string, DocumentContent>(StringComparer.Ordinal);
        internal readonly Dictionary<string, DocumentLine> LinesById =
            new Dictionary<string, DocumentLine>(StringComparer.Ordinal);
        internal readonly Dictionary<string, DocumentContent> DocumentsByLineId =
            new Dictionary<string, DocumentContent>(StringComparer.Ordinal);
        internal readonly Dictionary<string, AnswerPair> AnswersById =
            new Dictionary<string, AnswerPair>(StringComparer.Ordinal);
        internal readonly Dictionary<string, AnswerPair> AnswersByPair =
            new Dictionary<string, AnswerPair>(StringComparer.Ordinal);
        internal readonly Dictionary<string, EditInstruction> EditsByField =
            new Dictionary<string, EditInstruction>(StringComparer.Ordinal);

        internal DocumentContentIndex(DocumentAssignment assignment)
        {
            foreach (PersonContent person in assignment.People) PeopleById.Add(person.Id, person);
            foreach (DocumentContent document in assignment.Documents)
            {
                DocumentsById.Add(document.Id, document);
                foreach (DocumentLine line in document.Lines)
                {
                    LinesById.Add(line.Id, line);
                    DocumentsByLineId.Add(line.Id, document);
                }
            }
            foreach (AnswerPair answer in assignment.Answers)
            {
                AnswersById.Add(answer.Id, answer);
                AnswersByPair.Add(DocumentContract.Key(answer.StatementLineId, answer.ActionLineId), answer);
            }
            foreach (EditInstruction edit in assignment.Edits)
                EditsByField.Add(DocumentContract.Key(edit.PersonId, edit.Field.ToString()), edit);
        }
    }
}
