using UnityEngine;

public class TutorialDeactivationTrigger : MonoBehaviour

{

    // 대상 비활성화를 감지해 다음 튜토리얼 단계로 이동한다.
    private void OnDisable()

    {

        TutorialController.Instance.AdvanceTutorial();

        Destroy(this);

    }

}

