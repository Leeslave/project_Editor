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
        
        if (number == 1) { Debug.Log("임무를 확인합니다."); }
        else if (number == 2) { Debug.Log("메일을 확인합니다."); }
        else if (number == 3) 
        { 
            Debug.Log("임무를 수행합니다.");
            MainUI.SetActive(false);
            Debug.Log("UI를 숨깁니다.");
            MainSceneCamera.SetActive(false);
            EventSystem.SetActive(false);
            SceneManager.LoadSceneAsync("TempScene", LoadSceneMode.Additive);
        }
        else if (number == 4) 
        { 
            Debug.Log("스크린을 종료합니다.");
            Power.GetComponent<PowerOn>().ScreenPowerOff();
        }

    }
}
