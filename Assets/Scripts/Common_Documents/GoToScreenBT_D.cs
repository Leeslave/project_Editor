using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GoToScreenBT_D : Buttons_M
{
    [SerializeField] bool OnType = true;
    private void OnEnable()
    {
        if (!OnType) return;
        SceneManager.LoadScene("Screen");
    }

    protected override void Click(PointerEventData Data)
    {
        SceneManager.LoadScene("Screen");
    }
}