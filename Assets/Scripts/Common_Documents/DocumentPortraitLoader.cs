using System;
using System.Collections.Generic;
using UnityEngine;
using Utility;

namespace EditorGame.Documents
{
    internal static class DocumentPortraitLoader
    {
        internal static Dictionary<string, Sprite> Load(DocumentAssignment assignment)
        {
            var portraits = new Dictionary<string, Sprite>(StringComparer.Ordinal);
            foreach (PersonContent person in assignment.People)
            {
                string key = person.PortraitKey;
                if (string.IsNullOrWhiteSpace(key) || portraits.ContainsKey(key)) continue;
                Sprite portrait = Resources.Load<Sprite>("Image/" + key);
                portraits.Add(key, portrait);
                if (portrait == null) EditorLogger.LogWarning("Document portrait not found: " + key);
            }
            return portraits;
        }
    }
}
