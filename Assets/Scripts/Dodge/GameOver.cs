using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOver : MonoBehaviour
{
    public GameObject Player;
    // Update is called once per frame
    void Update()
    {
        // �ƹ� ��ư�̳� ������ �� ������ �������(��Ȱ)
        if (Input.anyKeyDown)
        {
            Player.SetActive(true);
            gameObject.SetActive(false);
        }
    }
}
