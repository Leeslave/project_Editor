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
    public int face;
}
[Serializable]
public class News
{
    public string publishDay;
    public string Title;
    public string Date;
    public string Reporter;
    public string[] Main; /*= new string[50];*/
    public int CountM; /*= 0;*/
    public string[] Revise; /*= new string[4];*/
    public int CountR; /*= 0;*/
    public List<int> Errors; /*= new List<int>(4);*/
    public News()
    {
        Main = new string[50];
        Revise = new string[4];
        Errors = new List<int>(4);
        CountM = 0;
        CountR = 0;
    }
}

public class Peoples
{
    public PeopleIndex[] PL;
}

