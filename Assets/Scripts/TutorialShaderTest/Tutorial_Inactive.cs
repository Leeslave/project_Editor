using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial_Inactive : MonoBehaviour
{
    private void OnDisable()
    {
        TutorialSetting.instance.GoNextTutorial();
        Destroy(this);
    }
}
