using System;
using System.Collections.Generic;

[Serializable]
public class PeopleIndex
{
    public string name_k;
    public string name_e;
    public string job;
    public string sex;
    public string country;
    public string key;
    public string age;
}

public class Peoples
{
    public PeopleIndex[] PL;
}

