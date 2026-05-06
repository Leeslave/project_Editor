using GameService;
using UnityEngine;

public class TestGame : MonoBehaviour
{
    public void Clear()
    {
        GameSystem.GetService<IWorkService>().ClearWork("TestGame");
        GameSystem.Instance.EnterScene("Screen");
    }
}
