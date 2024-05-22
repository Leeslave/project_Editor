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
    [SerializeField] TMP_Text country;
    [SerializeField] TMP_Text belong;
    [SerializeField] TMP_Text part;
    [SerializeField] TMP_Text job;
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
                    if (s != 4)
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
                        ValidData(4, "");
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

    // Get Folder's Contents & Make Files
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
        if (s != 4)
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
        else if(CurPeople != null)
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
        Name.text = "이름 : " + FindPeople.name_k;
        Age.text = "나이 : " + FindPeople.age;
        Sex.text = "성별 : " + (FindPeople.isMan ? "남성" : "여성");
        country.text = "국가 : " + FindPeople.country;
        job.text = "직급 : " + FindPeople.job;
        belong.text = "부서 : " + FindPeople.belong;
        part.text = "소속 : " + FindPeople.part;
        Face.sprite = FindPeople.Faces[FindPeople.curFace];
    }

    public void ApplyChange()
    {
        CurPeople.country = (Country)Enum.Parse(typeof(Country),country.text.Split(" ")[2]);
        CurPeople.job = (Job)Enum.Parse(typeof(Job), job.text.Split(" ")[2]);
        CurPeople.belong = (Belonging)Enum.Parse(typeof(Belonging), belong.text.Split(" ")[2]);
        CurPeople.part = (Part)Enum.Parse(typeof(Part), part.text.Split(" ")[2]);
        CurPeople.curFace = FaceNum;
    }

    private void OnDisable()
    {
        if (PeopleName != "") ApplyChange();
    }


    public void ValidData(int ind, string text)
    {
        foreach(var k in PeopleCorrect)
        {
            if(k.Item1.Equals(CurPeople.name_e) && k.Item2 == ind)
            {
                switch (ind)
                {
                    case 4: // Image
                        if (FaceNum == k.Item3) TDN.CheckList(2, -1, true, k.Item4);
                        else TDN.CheckList(2, -1, false, k.Item4);
                        break;
                    default:    // ETC
                        var j = text.Split(" ")[2];
                        if (DB.InfSub[ind][k.Item3].TrimEnd() == j) TDN.CheckList(2, 01, true, k.Item4);
                        else TDN.CheckList(2, -1, false, k.Item4);
                        break;
                    /*case 0:
                        if (DB.InfSub[0][k.Item3].TrimEnd() == text[10..]) TDN.CheckList(2, -1, true, k.Item4);
                        else TDN.CheckList(2, -1, false, k.Item4);
                        break;
                    case 1:
                        if (DB.InfSub[1][k.Item3].TrimEnd() == country.text[6..]) TDN.CheckList(2, -1, true, k.Item4);
                        else TDN.CheckList(2, -1, false, k.Item4);
                        break;
                    case 2:
                        break;
                    case 3:
                        break;*/
                        
                }
            }
        }
    }

    public bool IsTouchAble() { return TouchAble; }
    public void TouchAbleChange() { TouchAble = true; }
}
