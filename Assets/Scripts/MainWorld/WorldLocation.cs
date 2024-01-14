using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldLocation : MonoBehaviour
{
    [SerializeField]
    int connectLen;

    void OnEnable()
    {
        MovePosition(GameSystem.Instance.currentPosition);
    }

    public void MovePosition(int newPos)
    {
        if (newPos < 0 || newPos >= transform.childCount)
        {
            Debug.Log($"WORLD MOVE ERROR : Invalid position {newPos}");
            return;
        }

        StartCoroutine(WorldSceneManager.Instance.FadeInOut());

        transform.GetChild(newPos).gameObject.SetActive(true);

        for(int i = 0; i < transform.childCount; i++)
        {
            if (i == newPos)
            {
                continue;
            }
            transform.GetChild(i).gameObject.SetActive(false);
        }

        GameSystem.Instance.currentPosition = newPos;
    }

    public void MoveLocation(string location)
    {
        if(Enum.TryParse(location, out World newLocation))
        {
            WorldSceneManager.Instance.MoveLocation(newLocation);
        }
    }

    // 연결된 맵 왼쪽으로 이동
    public void MoveLeft()
    {
        int newPos = GameSystem.Instance.currentPosition - 1;
        if (newPos < 0)
        {
            Debug.Log($"WORLD MOVE ERROR : Invalid position {newPos}");
            return;
        }

        MovePosition(newPos);
    }

    // 연결된 맵 오른쪽으로 이동
    public void MoveRight()
    {
        int newPos = GameSystem.Instance.currentPosition + 1;
        if (newPos > connectLen)
        {
            Debug.Log($"WORLD MOVE ERROR : Invalid position {newPos}");
            return;
        }

        MovePosition(newPos);
    }
}
