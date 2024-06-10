using System;
using System.Collections.Generic;

[Serializable]
public class ADFGVXStageData
{
    public Dictionary<string, Dictionary<string, string>> Encrypt;
    public Dictionary<string, Dictionary<string, string>> Decrypt;
}