using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MazeTester : MonoBehaviour
{
    public MakeTile MT;
    public GameObject Canvas;
    public GameObject Player;
    public TMP_Dropdown DR;

    public void GameStart()
    {
        MT.Difficulty = DR.options[DR.value].text[0] - '0';
        MT.gameObject.SetActive(true);
        Canvas.SetActive(true);
        Player.gameObject.SetActive(true);
        gameObject.SetActive(false);
    }
}
