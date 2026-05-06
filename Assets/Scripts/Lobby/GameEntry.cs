using UnityEngine;

public class GameEntry : MonoBehaviour
{
    [SerializeField] private string saveScene;

    public void OnStart()
    {
        GameSystem.Instance.EnterScene(saveScene);
    }

    public void OnQuit()
    {
        Application.Quit();
    }
}
