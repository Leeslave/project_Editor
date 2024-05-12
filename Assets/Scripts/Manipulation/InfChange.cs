using System.Collections.Generic;
using System.IO;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using Unity.VisualScripting;

public class InfChange : MonoBehaviour
{
    [SerializeField] DB_M DB;
    [SerializeField] TMP_Text Name;
    [SerializeField] TMP_Text Age;
    [SerializeField] TMP_Text Sex;
    [SerializeField] TMP_Text Country;
    [SerializeField] TMP_Text Job;
    [SerializeField] Image Face;
    [SerializeField] GameObject Files;
    [SerializeField] GameObject Faces;
    [SerializeField] GameObject Folders;
    [SerializeField] GameObject Drager;
    [SerializeField] GameObject Drager_Image;
    [SerializeField] GameObject Terminal_Folder;
    [SerializeField] Image ImageDrager;
    [SerializeField] TMP_Text Terminal;
    [SerializeField] GameObject Reviser;
    [SerializeField] ToDoList_N TDN;

    [NonSerialized] public int s = 0;
    [NonSerialized] public int FaceNum = 0;
    [NonSerialized]public string PeopleName = "";

    private bool TouchAble = true;

    private GameObject CurFolder = null;
    private GameObject CurFile = null;
    private HighLighter_M CurHighLight = null;
    private Dictionary<string, Sprite[]> FaceImages = new Dictionary<string, Sprite[]>();

    /* private List<List <string>> CommandList_Back = new List<List<string>>();
     private List<List<string>> CommandList_Go = new List<List<string>>();*/

    PeopleIndex CurPeople;

    [NonSerialized] public List<Tuple<string, int, int, int>> PeopleCorrect = new List<Tuple<string, int, int,int>>();

    private void Start()
    {
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        transform.position = Vector3.zero;
        if (PeopleName != "")
        {
            CurPeople = DB.FindPeople(PeopleName);
            ChangeInf(CurPeople);
        }
    }

    public void TouchManager(GameObject cnt, int Type)
    {
        switch (Type)
        {
            case 0:             // File
                if (CurFile == null)
                {
                    CurFile = cnt;
                    CurHighLight = CurFile.GetComponent<HighLighter_M>( );
                }
                else if (CurFile != cnt)
                {
                    CurHighLight.HighLightOff();
                    CurFile = cnt;
                    CurHighLight = CurFile.GetComponent<HighLighter_M>();
                }
                else
                {
                    CurHighLight.HighLightOff();
                    if (s != 2)
                    {
                        Drager.name = CurFile.name;
                        Drager.SetActive(true);
                    }
                    else
                    {
                        Drager_Image.name = CurFile.transform.GetSiblingIndex().ToString();
                        Drager_Image.transform.GetChild(1).GetComponent<Image>().sprite
                            = CurPeople.Faces[CurFile.transform.GetSiblingIndex()];
                        Drager_Image.SetActive(true);
                        ValidData(2, "");
                    }
                    CurFile = null;
                    TouchAble = false;
                }
                break;
            case 1:             // Folder
                if (CurFolder == null)
                {
                    CurFolder = cnt;
                    CurHighLight = CurFolder.GetComponent<HighLighter_M>();
                }
                else if (CurFolder != cnt)
                {
                    CurHighLight.HighLightOff();
                    CurFolder = cnt;
                    CurHighLight = CurFolder.GetComponent<HighLighter_M>();
                }
                else
                {
                    CurHighLight.HighLightOff();
                    OpenFolder(CurHighLight);
                }
                break;
        }
    }

    [SerializeField] GetOptionFile_D GD;

    public void OpenFolder(HighLighter_M ss)
    {
        Reviser.SetActive(false);
        Reviser.SetActive(true);
        if (ss == null)
        {
            Folders.SetActive(true);
            Files.SetActive(false);
            CloseFolder();
            return;
        }
        s = ss.transform.GetSiblingIndex();
        Terminal.text = $"> {ss.gameObject.name}";
        Terminal_Folder.SetActive(true);
        GameObject cnt;
        if (s != 2)
        {
            for (int i = 0; i < ss.Files.Count; i++)
            {
                cnt = Files.transform.GetChild(i).gameObject;
                cnt.SetActive(true);
                cnt.name = ss.Files[i];
                cnt.transform.GetChild(2).GetComponent<TMP_Text>().text = ss.Files[i];
            }
            GD.Tabs[2].Subs[0] = Files;
            Files.SetActive(true);
        }
        else
        {
            for (int i = 0; i < CurPeople.Faces.Count; i++)
            {
                cnt = Faces.transform.GetChild(i).gameObject;
                cnt.SetActive(true);
                cnt.name = $"Face{i+1}";
                cnt.transform.GetChild(2).GetComponent<TMP_Text>().text = $"Face{i + 1}";
                cnt.transform.GetChild(1).GetComponent<Image>().sprite = CurPeople.Faces[i];
            }
            GD.Tabs[2].Subs[0] = Faces;
            Faces.SetActive(true);
        }
        Folders.SetActive(false);
    }

    public void CloseFolder()
    {
        Terminal_Folder.SetActive(false);
        GameObject cnt;
        for(int i = 0; i < Files.transform.childCount; i++)
        {
            cnt = Files.transform.GetChild(i).gameObject;
            if (!cnt.activeSelf) break;
            cnt.SetActive(false);
        }
        CurFolder = null;
        Files.SetActive(false);
        Faces.SetActive(false);
        GD.Tabs[2].Subs[0] = Folders;
        Folders.SetActive(true);
    }

    public void ChangeInf(PeopleIndex FindPeople)
    {
        CurPeople = FindPeople;
        Name.text = "name : " + FindPeople.name_k;
        Age.text = "Age : " + FindPeople.age;
        Sex.text = "Sex : " + (FindPeople.isMan ? "Male" : "Female");
        Country.text = "Country : " + FindPeople.country;
        if (FindPeople.belong != Belonging.Unknown) Job.text = "Job : " + FindPeople.belong + " " + (FindPeople.part == Part.None ? "": FindPeople.part + " ") + FindPeople.job;
        else Job.text = "Job : Unknown";
        Face.sprite = FindPeople.Faces[FindPeople.curFace];
    }

    public void SaveChange()
    {
        var s = Country.text.Split(' ');
        var s2 = Job.text.Split(' ');
    }

    public void ValidData(int ind, string text)
    {
        foreach(var k in PeopleCorrect)
        {
            if(k.Item1.Equals(CurPeople.name_e) && k.Item2 == ind)
            {
                switch (ind)
                {
                    case 0:
                        if (DB.InfSub[0][k.Item3].TrimEnd() == text[10..]) TDN.CheckList(2, -1, true, k.Item4);
                        else TDN.CheckList(2, -1, false, k.Item4);
                        break;
                    case 1:
                        if (DB.InfSub[1][k.Item3].TrimEnd() == Country.text[6..]) TDN.CheckList(2, -1, true, k.Item4);
                        else TDN.CheckList(2, -1, false, k.Item4);
                        break;
                    default:
                        if (FaceNum == k.Item3) TDN.CheckList(2, -1, true, k.Item4);
                        else TDN.CheckList(2, -1, false, k.Item4);
                        break;
                }
            }
        }
    }

    public bool IsTouchAble() { return TouchAble; }
    public void TouchAbleChange() { TouchAble = true; }
}
