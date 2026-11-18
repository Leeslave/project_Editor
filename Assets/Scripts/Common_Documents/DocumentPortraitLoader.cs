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
                // This character currently ships as a sliced Chat sheet, not an Image thumbnail.
                if (portrait == null && key == "Price_0")
                {
                    foreach (var candidate in Resources.LoadAll<Sprite>("Chat/Character/Price"))
                        if (candidate.name == "Price_CG 1_0") { portrait = candidate; break; }
                }
                portraits.Add(key, portrait);
                if (portrait == null) EditorLogger.LogWarning("Document portrait not found: " + key);
            }
            return portraits;
        }
    }
}
