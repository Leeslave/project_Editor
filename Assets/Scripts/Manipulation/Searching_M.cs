using TMPro;
using UnityEngine.UI;
using UnityEngine;
using System.Collections;

public class Searching_M : MonoBehaviour
{
    public DB_M DB;
    public TMP_InputField Name;
    public TMP_InputField Key;
    public SpriteRenderer Image;
    public Animator ImageAnim;
    public GameObject[] Progress;
    public TMP_Text Text;
    public Sprite Warning;


    private PeopleIndex FindPeople;


    string[] Out =
    {
        "Accessing To DB",
        "Checking Credential",
        "Comparing Name",
        "Gatehring Informaiton",
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

    public void SearchStart()
    {
        StartCoroutine(EA());
        StartCoroutine(BA());
    }

    void FindingError()
    {
        Image.sprite = Warning;
        ImageAnim.SetBool("Error", true);
        Text.text = "Information mismatch!";
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
        if(i == Out.Length-1)Text.text = Out[7];
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
