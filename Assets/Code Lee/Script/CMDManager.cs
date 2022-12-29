using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class CMDManager : MonoBehaviour
{
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
        else if (number == 3) { Debug.Log("�ӹ��� �����մϴ�."); }
        else if (number == 4) 
        { 
            Debug.Log("��ũ���� �����մϴ�.");
            Power.GetComponent<PowerOn>().ScreenPowerOff();
        }

    }
}
