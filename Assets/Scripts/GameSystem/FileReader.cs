using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public static class FileReader
{
    static readonly string SPLIT_RE = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))"; //regular expression
	static readonly string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";
	static readonly char[] TRIM_CHARS = { ' ' };


    /// <summary>
    /// CSV파일 Read, 이중리스트로 반환
    /// </summary>
    /// <param name="file">불러올 파일 경로</param>
    /// <returns></returns>
    public static List<List<string>> ReadCSV(string file)
    {

        TextAsset data = Resources.Load(file) as TextAsset;
        if (data == null)
        {
            Debug.Log("ERROR: CSV FILE CANNOT FOUND");
            return null;
        }

        List<List<string>> rowList = new List<List<string>>();
		string[] lines = Regex.Split(data.text, LINE_SPLIT_RE);

        if (lines.Length <= 1)
        {
            Debug.Log("ERROR: CSV FILE EMPTY");
            return null;
        }

		//첫 줄 Data index
        for (int i = 1; i < lines.Length; i++)
        {
            string[] values = Regex.Split(lines[i], SPLIT_RE);
            if (values.Length == 0 || values[0] == "") continue;

            List<string> columnList = new List<string>();
            for (int j = 0; j < values.Length; j++)
            {
                string value = values[j];
                //공백 제거
                value = value.TrimStart(TRIM_CHARS).TrimEnd(TRIM_CHARS);
                string finalvalue = value;
                columnList.Add(finalvalue);
            }
            rowList.Add(columnList);
        }
        return rowList;
    }
}
