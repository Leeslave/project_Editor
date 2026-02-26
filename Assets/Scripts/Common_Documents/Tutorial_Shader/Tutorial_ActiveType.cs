
using UnityEngine;

public class Tutorial_ActiveType : MonoBehaviour
{
    private void OnEnable()
    {
        TutorialSetting.instance.GoNextTutorial();
        Destroy(this);
    }
}
