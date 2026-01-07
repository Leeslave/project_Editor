using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private GameObject menuWindow;

    // Start is called before the first frame update
    void Start()
    {
        menuWindow.SetActive(false);    
    }

    public void ActivateWindow() 
    {
        if (!menuWindow.activeSelf) { menuWindow.SetActive(true); }
        else if (menuWindow.activeSelf) { menuWindow.SetActive(false); }
    }

}
