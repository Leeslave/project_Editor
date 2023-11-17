using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestGame : MonoBehaviour
{
    public void Clear()
    {
        GameSystem.Instance.player.renown += 10;
        GameSystem.Instance.ClearTask("TestGame");
        GameSystem.LoadScene("Screen");
    }
}
