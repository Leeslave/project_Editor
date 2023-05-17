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

    private GraphicRaycaster gr;
    private GameObject CurDetail;
    private GameObject CurFile;
    private GameObject CurObject;

    private bool JGDouble = false;
    private float InputLag = 0.5f;
    private string CurFolder = "Job";
    private List<string> Jobs;
    private List<string> Countries;

    private List<List <string>> CommandList_Back = new List<List<string>>();    // »ý°¢Á» ÇØº½
    private List<List<string>> CommandList_Go = new List<List<string>>();

    private void Awake()
    {
        Jobs = new List<string>();
        textReader("Jobs", Jobs);
        Countries = new List<string>();
        textReader("Countries", Countries);
    }

    private void Start()
    {
        gr = GetComponent<GraphicRaycaster>();
        ShowFolders();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (JGDouble)
            {
                CurObject = GRay();
                if (CurObject == null) return;
                switch (CurObject.tag)
                {
                    case "Detail": if (CurObject.name == CurFolder) CurDetail = CurObject; break;
                    case "File": CurFile = CurObject; break;
                    case "Folder":
                        OpenFolder_Sub(CurObject.name);
                        break;
                    case "Terminal":
                        Debug.Log(CurObject.name);
                        switch (CurObject.name)
                        {
                            /*case "TM_Back": if (CommandList_Back.Count != 0)
                                {
                                    CommandList_Back.RemoveAt(0);
                                    if (CommandList_Back[0][0] == "Folder")
                                    {
                                        CommandList_Go.Add(new List<string>{"Main"});
                                        OpenFolder_Sub(CurFolder);
                                    }
                                    else if (CommandList_Back[0][0] == "Main")
                                    {
                                        CommandList_Go.Add(new List<string> {"Folder", CurFolder});
                                        ShowFolders(); break;
                                    }
                                }
                                break;
                            case "TM_Go": if (CommandList_Go.Count != 0)
                                {
                                    CommandList_Go.RemoveAt(0);
                                    if (CommandList_Go[0][0] == "Folder")
                                    {
                                        CommandList_Back.Add(new List<string> { "Main" });
                                        OpenFolder_Sub(CurFolder);
                                    }
                                    else if (CommandList_Go[0][0] == "Main")
                                    {
                                        CommandList_Back.Add(new List<string> { "Folder", CurFolder });
                                        ShowFolders(); break;
                                    }
                                }
                                break;*/
                            case "TM_Main": TerminalFolder.text = ""; CommandList_Back.Add(new List<string> {"Main"});  ShowFolders(); break;
                        }
                        break;
                }
                if (CurDetail != null && CurFile != null)
                {
                    CurDetail.GetComponent<TMP_Text>().text = $"{CurFolder} : {CurFile.name}";
                    CurDetail = null;
                    CurFile = null;
                }
                JGDouble = false;
            }
            else
            {
                JGDouble = true;
                Invoke("DoubleCheck", InputLag);
            }
        }

    }
    void DoubleCheck()
    {
        JGDouble = false;
    }

    public void ChangeInf(PeopleIndex FindPeople)
    {
        Name.text = "name : " + FindPeople.name;
        Age.text = "Age : " + FindPeople.age;
        Sex.text = "Sex : " + FindPeople.sex;
        Country.text = "Country : " + FindPeople.country;
        Job.text = "Job : " + FindPeople.job;
        Face.sprite = Resources.LoadAll<Sprite>("Sprites/" + FindPeople.name)[0];
    }

    void textReader(string filename, List<string> a)
    { 
        TextAsset textFile = Resources.Load("Text/" + filename) as TextAsset;
        if(textFile == null)
        {
            return;
        }
        StringReader stringReader = new StringReader(textFile.text);
        while(stringReader != null)
        {
            string line = stringReader.ReadLine();
            if (line == null) break;
            a.Add(line);
        }
        stringReader.Close();
    }

    void ShowFolders()
    {
        CleanViewContents();
        GameObject EmptySpace1 = Instantiate(Files);
        EmptySpace1.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 0);
        EmptySpace1.transform.SetParent(ViewContent.transform);

        GameObject Folder = Instantiate(Folders);
        Folder.transform.SetParent(ViewContent.transform);

        GameObject EmptySpace2 = Instantiate(Files);
        EmptySpace2.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 0);
        EmptySpace2.transform.SetParent(ViewContent.transform);
    }
    void OpenFolder_Sub(string FolderName)
    {
        CurFolder = FolderName;
        switch (FolderName)
        {
            case "Job": OpenFolder(Jobs); TerminalFolder.text = "> Job"; CommandList_Back.Add(new List<string> { "Folder", "Job" }); break;
            case "Country": OpenFolder(Countries); TerminalFolder.text = "> Country"; CommandList_Back.Add(new List<string> { "Folder", "Country" }); break;
        }
    }
    void OpenFolder(List<string> FolderList)
    {
        CleanViewContents();
        GameObject EmptySpace1 = Instantiate(Files);
        EmptySpace1.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 0);
        EmptySpace1.transform.SetParent(ViewContent.transform);
        for (int i = 0; i < FolderList.Count / 2; i++)
        {
            GameObject cnt = Instantiate(Files);
            cnt.transform.SetParent(ViewContent.transform);
            GameObject Child1 = cnt.transform.GetChild(0).gameObject;
            Child1.SetActive(true);
            Child1.name = FolderList[i * 2];
            Child1.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = FolderList[i * 2];
            if ((i + 1) * 2 != FolderList.Count)
            {
                GameObject Child2 = cnt.transform.GetChild(1).gameObject;
                Child2.SetActive(true);
                Child2.name = FolderList[i * 2 + 1];
                Child2.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = FolderList[i * 2 + 1];
            }
        }
        GameObject EmptySpace2 = Instantiate(Files);
        EmptySpace2.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 0);
        EmptySpace2.transform.SetParent(ViewContent.transform);
    }

    void CleanViewContents()
    {
        for(int i = 0; i< ViewContent.transform.childCount; i++)
        {
            Destroy(ViewContent.transform.GetChild(i).gameObject);
        }
    }

    GameObject GRay()
    {
        var ped = new PointerEventData(null);
        ped.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        if (gr == null) return null;
        gr.Raycast(ped, results);

        if (results.Count <= 0) return null;
        return results[0].gameObject;
    }
}
