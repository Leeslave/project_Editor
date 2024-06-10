using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LoginProcess_D : MonoBehaviour
{
    [SerializeField] TMP_InputField ID;
    [SerializeField] TMP_InputField PW;

    [SerializeField] GameObject Process;
    [SerializeField] TMP_Text TryLeft;

    [SerializeField] string id;
    [SerializeField] string pw;

    [SerializeField] GameObject SecretFolder;
    [SerializeField] Windows_M Secrets;
    [SerializeField] string id_S;
    [SerializeField] string pw_S;

    int LoginTry = 3;

    bool S = false;


    private void OnEnable()
    {
        if (Process.activeSelf) gameObject.SetActive(false);
    }

    public void LoginProcess()
    {
        if (LoginTry < 0)
        {
            gameObject.SetActive(false);
        }
        else if (ID.text == id && PW.text == pw)
        {
            LoginTry = 4;
            Process.SetActive(true);
            gameObject.SetActive(false);
        }
        else if (ID.text == id_S && PW.text == pw_S)
        {
            LoginTry = 4;
            SecretFolder.SetActive(true);
            Process.SetActive(true);
            gameObject.SetActive(false);
        }
        if (ID.text != "" && PW.text != "")
        {
            LoginTry--;
            ID.text = "";
            PW.text = "";
        }
        if (LoginTry > 0) TryLeft.text = $"{LoginTry} Try Left";
        else TryLeft.text = "Locked";
    }
}
