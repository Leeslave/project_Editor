using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Unity.VisualScripting;

public class InfChange : MonoBehaviour
{
    public TMP_Text Name;
    public TMP_Text Age;
    public TMP_Text Sex;
    public TMP_Text Country;
    public TMP_Text Job;
    public TMP_Text TerminalFolder;
    public GameObject Files;
    public GameObject Folders;
    public GameObject ViewContent;
    public UnityEngine.UI.Image Face;

    private GameObject CurDetail;
    private GameObject CurFile;
    private GameObject CurObject;

    private float InputLag = 0.5f;
    private string CurFolder = "Job";
    private List<string> Jobs;
    private List<string> Countries;

    private List<List <string>> CommandList_Back = new List<List<string>>();    // »ý°¢Á» ÇØº½
    private List<List<string>> CommandList_Go = new List<List<string>>();

    public void TouchManager()
    {

    }

    public void ChangeInf(PeopleIndex FindPeople)
    {
        Name.text = "name : " + FindPeople.name;
        Age.text = "Age : " + FindPeople.age;
        Sex.text = "Sex : " + FindPeople.sex;
        Country.text = "Country : " + FindPeople.country;
        Job.text = "Job : " + FindPeople.job;
        Face.sprite = Resources.LoadAll<Sprite>("Manipulation/" + FindPeople.name)[0];
    }

    
}
