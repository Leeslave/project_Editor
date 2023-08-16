using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TestGameManager : MiniGame
{
    public override void LoadGameData()
    {
        throw new System.NotImplementedException();
    }

    public override void ClearTask()
    {
        base.ClearTask();
        SceneManager.LoadScene("Screen");
    }
}
