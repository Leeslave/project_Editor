using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ReportText : MonoBehaviour
{
    public string Time;
    public string Place;
    public string Action;
    public string ErrorType = "";

    public void ChangeText()
    {
        gameObject.GetComponent<TMP_Text>().text = $" {Time} {Place}¿¡¼­ {Action}";
    }

    public void ChangeCard()
    {
        gameObject.tag = "CardText";
        int pl = (19 - Place.Length)/2 + Place.Length;
        gameObject.GetComponent<TMP_Text>().text = $"    {Time.PadRight(7,' ')}|{Place.PadRight(pl,' ').PadLeft(19,' ')}|     10000¿ø";
    }
}
