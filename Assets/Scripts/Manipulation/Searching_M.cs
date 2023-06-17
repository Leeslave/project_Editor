using TMPro;
using UnityEngine.UI;
using UnityEngine;
using System.Collections;

public class Searching_M : MonoBehaviour
{
    public DB_M DB;
    public TMP_InputField Name;
    public TMP_InputField Key;
    public GameObject Search;
    public GameObject Imagee;
    public GameObject Success;
    public GameObject[] Progress;
    public TMP_Text Text;
    public GameObject Searching;
    public GameObject Portal;
    public InfChange Inf;
    public GameObject Detail;


    private PeopleIndex FindPeople;
    int ErrorCount = 3;
    bool IsError = false;
    bool IsInput = false;


    string[] Out =
    {
        "Accessing To DB",
        "Checking Credential",
        "Comparing Name",
        "Gathering Informaiton",
        "Comparing Key",
        "Decoding Information",
        "Configuring Information",
        "Success! Press Enter To Continue"
    };
    float[] NextTime =
    {
        0.5f,
        0.5f,
        1.0f,
        2.0f,
        0.5f,
        2.0f,
        2.5f
    };

    private void OnEnable()
    {
        Name.text = "";
        Key.text = "";
        Portal.SetActive(true);
        Searching.SetActive(false);
    }

    private void Update()
    {
        if (IsInput)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                if (IsError)
                {
                    Searching.SetActive(false);
                    Portal.SetActive(true);
                    IsError = false;
                }
                else
                {
                    Detail.gameObject.SetActive(true);
                    Inf.ChangeInf(FindPeople);
                    gameObject.SetActive(false);
                }
            }
        }
    }

    public void SearchStart()
    {
        foreach (GameObject a in Progress) a.SetActive(false);
        Search.SetActive(true);
        Success.SetActive(false);
        Imagee.SetActive(false);
        StartCoroutine(EA());
        StartCoroutine(BA());
    }

    void FindingError()
    {
        Search.SetActive(false);
        Imagee.SetActive(true);
        Text.text = "Information mismatch! Enter To Continue";
        IsError = true;
    }

    IEnumerator EA()
    {
        int i;
        for(i = 0; i < Out.Length-1; i++)
        {
            Text.text = Out[i];
            yield return new WaitForSeconds(NextTime[i]);
            if(i == 4)
            {
                FindPeople = DB.FindPeople(Name.text, Key.text);
                if (FindPeople == null)
                {
                    FindingError();
                    StopAllCoroutines();
                    break;
                }
            }
        }
        if (i == Out.Length - 1)
        {
            Success.SetActive(true);
            Search.SetActive(false);
            Text.text = Out[7];
        }
        IsInput = true;
    }
    IEnumerator BA()
    {
        WaitForSeconds WFS = new WaitForSeconds(1);
        for(int i = 0; i < Progress.Length; i++)
        {
            Progress[i].SetActive(true);
            yield return WFS;
        }
    }
}
