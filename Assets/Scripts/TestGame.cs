using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestGame : MonoBehaviour
{
    public void Clear()
    {
        GameSystem.Instance.ClearTask("TestGame");
        GameSystem.LoadScene("Screen");
    }
}
