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

    public string ID;
    private string InputID = "";
    public string PW;
    private string InputPW = "";

    private string Name = null;
    private Peoples PeopleList;
    private PeopleIndex FindingPeople;
    private bool IsLogin = true;
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
    private int TextLine = 13;

    private void Awake()
    {
        TextAsset textAsset = Resources.Load<TextAsset>("Json/People");
        PeopleList = JsonUtility.FromJson<Peoples>(textAsset.text);
    }
    private void Start()
    {
        ActiveField();
        MoveInputField();
        SubText.text += " Input User ID>";
    }
    void Update()
    {
        if (ErrorCount == 0)
        {
            SubText.text += "\n\nPress Enter To Continue...";
            Destroy(gameObject);
        }
        if (IsLogin)
        {
            if (InputID == "")
            {
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    if (SearchInput.text != "")
                    {
                        InputID = SearchInput.text;
                        SearchInput.text = "";
                        ActiveField();
                        TextLine++;
                        SubText.text += $"{InputID}\n Input User PW>";
                        MoveInputField();
                    }
                }
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    if(SearchInput.text != "")
                    {
                        InputPW = SearchInput.text;
                        if(InputPW == PW && InputID == ID)
                        {
                            SubText.text = "Welcome Back Clayton\n Input Name...>";
                            TextLine = 2;
                            ActiveField();
                            IsLogin = false;
                        }
                        else
                        {
                            SubText.text = $"No Correct Information.. \n{--ErrorCount} Try Left...\n Input User ID>";
                            TextLine = 3;
                            ActiveField();
                            InputID = "";
                            InputPW = "";
                        }
                    }
                }
            }
        }
        else if (!IsFinding)
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
                    TextLine = 7;
                    Invoke("Finding", 0);                }
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
                    TextLine = 3;
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
                TextLine += 5;
                SubText.text += $"\n\nNo Matching Key...\n{--ErrorCount} Try Left...\n\n Input Name...>";
                ActiveField();
                IsKey = true;
            }
        }
        else
        {
            TextLine += 5;
            SubText.text += $"\n\nNo Matching Key...\n{--ErrorCount} Try Left...\n\n Input Name...>";
            ActiveField();
            IsKey = true;
        }
        SearchInput.text = "";
    }

    void ActiveField()
    {
        SearchInput.text = "";
        SearchInput.ActivateInputField();
        MoveInputField();
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
        if(TimeCnt != 1.0f)
        {
            Invoke("Finding", 0.5f);
        }
        else if (jc!=0)
        {
            TextLine += 2;
            SubText.text += $"\n\n Input Key... >";
            ActiveField();
            IsKey = true;
        }
        else
        {
            TextLine += 5;
            SubText.text += $"\n\nNo People Finded...\n{--ErrorCount} Try Left...\n\n Input Name...>";
            ActiveField();
            IsFinding = false;
        }
        TimeCnt += 0.5f;
    }


    void MoveInputField() 
    {
        SearchInput.transform.position = new Vector3(SearchInput.transform.position.x,376.3f + 198 + 108 - TextLine * 36,0);
    }
}



