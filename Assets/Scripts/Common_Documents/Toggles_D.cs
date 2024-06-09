using System;
using UnityEngine;
using UnityEngine.UI;


// ToggleObject용, 현재는 안쓰지만 나중에 쓸수도 있으니까 일단 내버려둠
public class Toggles_D : MonoBehaviour
{
    public int Ind;
    Toggle TG;
    public delegate void Interact(int Ind, bool IsOn);
    public Interact interact = null;

    private void Awake()
    {
        TG = GetComponent<Toggle>();
        TG.onValueChanged.AddListener((x) => interact.Invoke(Ind,x));
        if (TG.isOn) TG.enabled = false;
    }
    public void AddAction(Action<int,bool> Act)
    {
        interact = new Interact(Act);
    }
}
