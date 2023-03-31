using UnityEngine;
using System.Collections;

public class AnimBase : MonoBehaviour {
    /**
    *   코루틴 애니메이션 부모클래스
    *   - 애니메이션 실행용 코루틴함수
    */
    public UnityEngine.UI.Text GUITextCtrl;

    public virtual IEnumerator Play() { yield return 0; }
}
