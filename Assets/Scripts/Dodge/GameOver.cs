using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOver : MonoBehaviour
{
    public GameObject Player;
    // Update is called once per frame
    void Update()
    {
        // 아무 버튼이나 눌렀을 때 게임을 재시작함(부활)
        if (Input.anyKeyDown)
        {
            Player.SetActive(true);
            gameObject.SetActive(false);
        }
    }
}
