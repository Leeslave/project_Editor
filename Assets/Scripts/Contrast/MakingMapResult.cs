using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class MakingMapResult : MonoBehaviour
{
    public string Dest1 = "";
    public string Dest2 = "";
    public TMP_Text D1;
    public TMP_Text D2;

    public void ClickedReact(string Clicked)
    {
        if (Dest1 == "") Dest1 = Clicked;
        else if (Dest2 == "")
        {
            Dest2 = Clicked;
            gameObject.SetActive(true);
            D1.text = Dest1; D2.text = Dest2;
        }
        else
        {
            Dest1 = Clicked; D1.text = Dest1;
            Dest2 = ""; D2.text = "";
        }
    }
    void OnDisable()
    {
        Dest1 = "";
        Dest2 = "";
    }
}
