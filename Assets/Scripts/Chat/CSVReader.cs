using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;

public class CSVReader
{
    static string SPLIT_RE = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
    static string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";
    static char[] TRIM_CHARS = { '\"' };

    public static List<Dictionary<string, object>> Read(string filePath)
    {
        var list = new List<Dictionary<string, object>>();
        //TextAsset data = Resources.Load (file) as TextAsset;

        // 파일 읽어오기
        string source;
        StreamReader sr = new StreamReader(filePath);
        source = sr.ReadToEnd();
        sr.Close();

        //var lines = Regex.Split(data.text, LINE_SPLIT_RE);

        var lines = Regex.Split(source, LINE_SPLIT_RE);     // 줄단위 분리
        if (lines.Length <= 1) return list;     // 내용 없을 시 빈 List 출력 (첫줄은 무조건 header로 간주)

        var header = Regex.Split(lines[0], SPLIT_RE);   // 헤더 구분

        for (var i = 1; i < lines.Length; i++)
        {

            var values = Regex.Split(lines[i], SPLIT_RE);   // 각 칸 내용 분리
            if (values.Length == 0 || values[0] == "") continue;    // 비어있는 칸 스킵

            var entry = new Dictionary<string, object>();
            for (var j = 0; j < header.Length && j < values.Length; j++)
            {
                // 각 줄의 값들 추출
                string value = values[j];
                value = value.TrimStart(TRIM_CHARS).TrimEnd(TRIM_CHARS).Replace("\\", "");
                object finalvalue = value;

                // 값이 숫자이면 숫자로 파싱
                int n;
                float f;
                if (int.TryParse(value, out n))
                {
                    finalvalue = n;
                }
                else if (float.TryParse(value, out f))
                {
                    finalvalue = f;
                }
                entry[header[j]] = finalvalue;
            }
            list.Add(entry);
        }
        return list;
    }
}