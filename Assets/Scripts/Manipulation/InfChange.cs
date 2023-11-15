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

    Peoples PeopleList;
    PeopleIndex CurPeople;


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
                            = DB.FaceImages[CurPeople.name_e][CurFile.transform.GetSiblingIndex()];
                        Drager_Image.SetActive(true);
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
            Files.SetActive(true);
        }
        else
        {
            for (int i = 0; i < DB.FaceImages[CurPeople.name_e].Length; i++)
            {
                cnt = Faces.transform.GetChild(i).gameObject;
                cnt.SetActive(true);
                cnt.name = $"Face{i+1}";
                cnt.transform.GetChild(2).GetComponent<TMP_Text>().text = $"Face{i + 1}";
                cnt.transform.GetChild(1).GetComponent<Image>().sprite = DB.FaceImages[CurPeople.name_e][i];
            }
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
        Folders.SetActive(true);
    }

    public void ChangeInf(PeopleIndex FindPeople)
    {
        CurPeople = FindPeople;
        Name.text = "name : " + FindPeople.name_k;
        Age.text = "Age : " + FindPeople.age;
        Sex.text = "Sex : " + FindPeople.sex;
        Country.text = "Country : " + FindPeople.country;
        Job.text = "Job : " + FindPeople.job;
        Face.sprite = DB.FaceImages[FindPeople.name_e][FindPeople.face];
    }

    public void SaveChange()
    {
        var s = Country.text.Split(' ');
        var s2 = Job.text.Split(' ');
        DB.ChangeInfo(CurPeople.name_e, s[s.Length - 1], s2[s2.Length-1],FaceNum);
    }

    public bool IsTouchAble() { return TouchAble; }
    public void TouchAbleChange() { TouchAble = true; }
}
