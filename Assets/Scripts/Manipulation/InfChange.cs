using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InfChange : MonoBehaviour
{
    public TMP_Text Name;
    public TMP_Text Age;
    public TMP_Text Sex;
    public TMP_Text Country;
    public TMP_Text Job;
    public TMP_Text TerminalFolder;
    public Image Face;
    public GameObject Files;
    public GameObject Folders;

    private GameObject CurFolder = null;
    private HighLighter_M CurHighLight = null;
    private GameObject CntObject = null;

    private List<List <string>> CommandList_Back = new List<List<string>>();    // »ý°¢Á» ÇØº½
    private List<List<string>> CommandList_Go = new List<List<string>>();

    public void TouchManager(GameObject cnt, int Type)
    {
        switch (Type)
        {
            case 0:             // File
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
                    CurFolder = null;
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
                    Folders.SetActive(false);
                    Files.SetActive(true);
                    OpenFolder();
                    CurFolder = null;
                }
                break;
            case 2:             // Etc
                break;
        }

    }

    private void OpenFolder()
    {
        GameObject cnt;
        for(int i = 0; i < CurHighLight.Files.Count; i++)
        {
            cnt = Files.transform.GetChild(i).gameObject;
            cnt.SetActive(true);
            cnt.transform.GetChild(2).GetComponent<TMP_Text>().text = CurHighLight.Files[i];
        }
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
