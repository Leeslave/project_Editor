using JetBrains.Annotations;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SerializeField]
public class Polices
{
    [JsonProperty("Police")]
    public Police[] PoliceList;
}

[SerializeField]
public class Police
{
    public int HP;
    public int Start;
    public int End;
    public bool IsCar;
}