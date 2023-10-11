
using UnityEngine;

[System.Serializable]
public abstract class MiniGame: MonoBehaviour
{
    public abstract void LoadGameData();

    public virtual void ClearTask()
    {
        GameSystem.Instance.ClearTask();
    }
}