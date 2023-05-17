using System;
public class PeopleIndex
{
    public string name;
    public string job;
    public string sex;
    public string country;
    public string key;
    public string age;
}
public class Peoples
{
    public PeopleIndex[] pl;

    public PeopleIndex PeopleFind(String Name)
    {
        foreach (PeopleIndex at in pl)
        {
            if (Name == at.name)
                return at;
        }
        return null;
    }
}

