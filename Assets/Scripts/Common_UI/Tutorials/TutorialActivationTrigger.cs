using UnityEngine;

public class TutorialActivationTrigger : MonoBehaviour

{

    // 대상 활성화를 감지해 다음 튜토리얼 단계로 이동한다.
    private void OnEnable()

    {

        TutorialController.Instance.AdvanceTutorial();

        Destroy(this);

    }

}

