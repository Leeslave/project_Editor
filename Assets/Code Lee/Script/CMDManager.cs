using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;


public class CMDManager : MonoBehaviour
{
    public GameObject MainSceneCamera;
    public GameObject EventSystem;
    public GameObject MainUI;
    public GameObject Power;
    public TMP_InputField CMDText;
    private string InputString;
    private int number;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);
    }


    public void CMDStart()
    {
        gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            InputString = CMDText.text;
            if (int.Parse(InputString) <= 4 || int.Parse(InputString) >= 1)
            {
                number = int.Parse(InputString);
                CMDText.text = "";
                InputNumber(number);
                Debug.Log(number);
            }
            else
            {
                CMDText.text = "";
                Debug.Log("Worng Input!");
            }
        }
    }

    public void InputNumber(int number)
    {
        
        if (number == 1) { Debug.Log("�ӹ��� Ȯ���մϴ�."); }
        else if (number == 2) { Debug.Log("������ Ȯ���մϴ�."); }
        else if (number == 3) 
        { 
            Debug.Log("�ӹ��� �����մϴ�.");
            MainUI.SetActive(false);
            Debug.Log("UI�� ����ϴ�.");
            MainSceneCamera.SetActive(false);
            EventSystem.SetActive(false);
            SceneManager.LoadSceneAsync("TempScene", LoadSceneMode.Additive);
        }
        else if (number == 4) 
        { 
            Debug.Log("��ũ���� �����մϴ�.");
            Power.GetComponent<PowerOn>().ScreenPowerOff();
        }

    }
}
