using System.Collections;
using UnityEngine;

public abstract class TutorialManager : MonoBehaviour
{
    public GameObject blocker;
    private GameObject _currentPopUp;
    private float _shiftingTime;

    protected IEnumerator ShowPopUp(GameObject go)
    {
        _currentPopUp = go;
        _currentPopUp.SetActive(true);
        yield return WaitUntilActiveSelf();
    }
    
    private IEnumerator WaitUntilActiveSelf()
    {
        blocker.gameObject.SetActive(false);
        yield return new WaitUntil(() => !_currentPopUp.activeSelf);
        _currentPopUp = null;
        blocker.gameObject.SetActive(true);
        yield return new WaitForSeconds(_shiftingTime);
        _shiftingTime = 0f;
    }
    
    public void MoveToNextTutorialPhase(float shiftingTime = 0f)
    {
        _shiftingTime = shiftingTime;
        _currentPopUp.SetActive(false);
    }
}
