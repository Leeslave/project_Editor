using System.Collections.Generic;
using System.IO;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class InfChange : MonoBehaviour
{
    public TMP_Text Name;
    public TMP_Text Age;
    public TMP_Text Sex;
    public TMP_Text Country;
    public TMP_Text Job;
    public Image Face;
    public GameObject Files;
    public GameObject Folders;
    public GameObject Faces;
    public GameObject Drager;
    public GameObject Drager_Image;
    public Image ImageDrager;
    public TabManager_M TM;

    public int s = 0;

    private bool TouchAble = true;

    private GameObject CurFolder = null;
    private GameObject CurFile = null;
    private HighLighter_M CurHighLight = null;
    private Dictionary<string, List<Image>> FaceImages;

    /* private List<List <string>> CommandList_Back = new List<List<string>>();
     private List<List<string>> CommandList_Go = new List<List<string>>();*/

    Peoples PeopleList;

    void Start()
    {
        TextAsset textAsset = Resources.Load<TextAsset>("Manipulation/People");
        print(textAsset.text);
        PeopleList = JsonUtility.FromJson<Peoples>(textAsset.text);
        print(PeopleList.PL.Length);
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
                    s = CurFolder.transform.GetSiblingIndex();
                    TM.ChangeFolder(CurHighLight, CurFolder.name);
                    if(s != 2) OpenFolder(CurHighLight);
                    else
                    {

                    }
                }
                break;
        }

    }

    public void OpenFolder(HighLighter_M ss)
    {
        CloseFolder();
        if(ss == null)
        {
            Folders.SetActive(true);
            Files.SetActive(false);
            return;
        }
        GameObject cnt;
        for(int i = 0; i < ss.Files.Count; i++)
        {
            cnt = Files.transform.GetChild(i).gameObject;
            cnt.SetActive(true);
            cnt.name = ss.Files[i];
            cnt.transform.GetChild(2).GetComponent<TMP_Text>().text = ss.Files[i];
        }
        Folders.SetActive(false);
        Files.SetActive(true);
    }

    public void CloseFolder()
    {
        GameObject cnt;
        for(int i = 0; i < Files.transform.childCount; i++)
        {
            cnt = Files.transform.GetChild(i).gameObject;
            if (!cnt.activeSelf) break;
            cnt.SetActive(false);
        }
        CurFolder = null;
        Files.SetActive(false);
        Folders.SetActive(true);
    }

    public void ChangeInf(PeopleIndex FindPeople)
    {
        Name.text = "name : " + FindPeople.name_k;
        Age.text = "Age : " + FindPeople.age;
        Sex.text = "Sex : " + FindPeople.sex;
        Country.text = "Country : " + FindPeople.country;
        Job.text = "Job : " + FindPeople.job;
        /*Face.sprite = Resources.LoadAll<Sprite>("Manipulation/" + FindPeople.name)[0];*/
    }

    public bool IsTouchAble() { return TouchAble; }
    public void TouchAbleChange() { TouchAble = true; }
}
