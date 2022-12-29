using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using System;
using UnityEngine.UIElements;
using Unity.VisualScripting;
using Random = UnityEngine.Random;

public class SearchPeople: MonoBehaviour
{
    public TMP_Text SubText;
    public TMP_InputField SearchInput;

    public GameObject NextScene;
    

    private string Name = null;
    private Peoples PeopleList;
    private PeopleIndex FindingPeople;
    private bool IsFinding = false;
    private bool IsKey = false;
    private bool IsNext = false;
    /*private bool IsFail = false;*/

    private int ErrorCount = 3;

    private float TimeCnt = 0;
    private string cnt1 = "";
    private string cnt2 = "";
    private int cnt3 = 1;
    private int jc = 1;

    private void Awake()
    {
        TextAsset textAsset = Resources.Load<TextAsset>("Json/People");
        PeopleList = JsonUtility.FromJson<Peoples>(textAsset.text);
    }
    private void Start()
    {
        ActiveField();
    }
    void Update()
    {
        if (ErrorCount == 0)
        {
            Debug.Log("Game Over");
            Application.Quit();
        }
        if (!IsFinding)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                if (SearchInput.text != "")
                {
                    Name = SearchInput.text;
                    FindingPeople = PeopleList.PeopleFind(Name);
                    if (FindingPeople == null)
                    {
                        jc = 0; cnt3 = 0;
                    }
                    else
                    {
                        jc = 1; cnt3 = 1;
                    }
                    SearchInput.transform.Translate(new Vector3(0, 0, -10));
                    SearchInput.text = "";
                    IsFinding = true;
                    TimeCnt = 0;
                    cnt2 = "";
                    Invoke("Finding", 0);
                    
                }
            }
        }
        else if(IsKey)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                if (SearchInput.text != "")
                {
                    SubText.text = $"Current Finding People : {Name}\n\nComparing Key...";
                    SearchInput.transform.Translate(new Vector3(0, 0, -10));
                    IsKey = false;
                    Invoke("SubKey", 1);
                }
            }
        }
        else if (IsNext)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                GameObject.Find("Main Camera").GetComponent<Camera>().transform.Translate(new Vector3(2000, 0, 0));
                gameObject.SetActive(false);
            }
        }
    }

    void SubKey()
    {
        if (FindingPeople != null)
        {
            if (SearchInput.text == FindingPeople.key)
            {
                SubText.text += "\n\nPress Enter To Continue...";
                NextScene.SetActive(true);
                NextScene.GetComponent<InfChange>().ChangeInf(FindingPeople);
                IsNext = true;
                IsKey = false;
            }
            else
            {
                SubText.text += $"\n\nNo Matching Key...\n{--ErrorCount} Try Left...\n\n $ftpuser>";
                SearchInput.transform.position = new Vector3(SearchInput.transform.position.x, 283.6f + 108, 0);
                ActiveField();
                IsKey = true;
            }
        }
        else
        {
            SubText.text += $"\n\nNo Matching Key...\n{--ErrorCount} Try Left...\n\n $ftpuser>";
            SearchInput.transform.position = new Vector3(SearchInput.transform.position.x, 283.6f + 108, 0);
            ActiveField();
            IsKey = true;
        }
        SearchInput.text = "";
    }

    void ActiveField()
    {
        SearchInput.ActivateInputField();
    }

    void Finding()
    {
        int tmp = (int)(TimeCnt * 10 / 5);
        cnt1 = "";
        for (int i = 0; i < tmp % 3 + 1; i++)
        {
            cnt1 += ".";
        }
        cnt2 += "бсбс";
        cnt3 += Random.Range(0,2) * Random.Range(0,5) * jc;
        SubText.text = $"Target : {Name}\n\nFinding{cnt1}\n\n [{cnt2.PadRight(60 - (tmp+1) * 2,' ')}]\n\n{cnt3} People Finded";
        if(TimeCnt != 7.0f)
        {
            Invoke("Finding", 0.5f);
        }
        else if (jc!=0)
        {
            SubText.text += $"\n\nInput Key...\n\n $ftpuser>";
            SearchInput.transform.position = new Vector3(SearchInput.transform.position.x, 283.6f, 0);
            ActiveField();
            IsKey = true;
        }
        else
        {
            SubText.text += $"\n\nNo People Finded...\n{--ErrorCount} Try Left...\n\n $ftpuser>";
            SearchInput.transform.position = new Vector3(SearchInput.transform.position.x, 283.6f - 36, 0);
            ActiveField();
            IsFinding = false;
        }
        TimeCnt += 0.5f;
    }
}
