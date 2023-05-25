using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DB_M : MonoBehaviour
{
    Peoples PeopleList;
    PeopleIndex CurPeople;
    Dictionary<string, Sprite[]> FaceImages = new Dictionary<string, Sprite[]>();
    void Awake()
    {
        TextAsset textAsset = Resources.Load<TextAsset>("Manipulation/People");
        PeopleList = JsonUtility.FromJson<Peoples>(textAsset.text);
        foreach (PeopleIndex a in PeopleList.PL) FaceImages.Add(a.name_e, Resources.LoadAll<Sprite>("Manipulation/" + a.name_e));
    }

    

    public PeopleIndex FindPeople(string name,string key)
    {
        foreach(PeopleIndex a in PeopleList.PL)
        {
            if ((a.name_e == name || a.name_k == name) && a.key == key) return a;
        }
        return null;
    }
}
