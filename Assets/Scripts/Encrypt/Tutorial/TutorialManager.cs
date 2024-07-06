using System.Collections;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    private GameObject _currentPopUp;
    private float _shiftingTime;
    
    protected IEnumerator WaitUntilActiveSelf(GameObject go)
    {
        _currentPopUp = go;
        ADFGVXGameManager.Blocker.gameObject.SetActive(false);
        yield return new WaitUntil(() => !_currentPopUp.activeSelf);
        _currentPopUp = null;
        ADFGVXGameManager.Blocker.gameObject.SetActive(true);
        yield return new WaitForSeconds(_shiftingTime);
        _shiftingTime = 0f;
    }
    public void MoveToNextTutorialPhase(float shiftingTime = 0f)
    {
        _shiftingTime = shiftingTime;
        _currentPopUp.SetActive(false);
    }
}
