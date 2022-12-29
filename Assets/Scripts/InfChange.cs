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
    public GameObject Files;
    public GameObject ViewContent;
    public UnityEngine.UI.Image Face;

    private GraphicRaycaster gr;
    private GameObject BfObject;
    private GameObject CurObject;
    private List<string> Jobs;
    private List<string> Countries;

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
        /*OpenFolder(Jobs);*/
    }

    private void Update()
    {
        
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
        EmptySpace1.transform.SetParent(ViewContent.transform);

        GameObject EmptySpace2 = Instantiate(Files);
        EmptySpace2.transform.SetParent(ViewContent.transform);
    }
    void OpenFolder(List<string> FolderList)
    {
        CleanViewContents();
        GameObject EmptySpace1 = Instantiate(Files);
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
