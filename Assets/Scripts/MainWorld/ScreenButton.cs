using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class ScreenButton : MonoBehaviour, IPointerClickHandler
{
    /**
    *   스크린 활성화, 비활성화 트리거 코드
    *   - 스크린 아이콘을 눌러 스크린을 활성화
    *   - down 버튼을 눌러 스크린 비활성화
    */
    
    private Coroutine coroutine;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (coroutine != null) return;
        
        // 스크린 활성화
        coroutine = StartCoroutine(LoadScreenScene());
    }

    private IEnumerator LoadScreenScene()
    {
        // 스크린 활성화
        var load = SceneManager.LoadSceneAsync("Screen", LoadSceneMode.Additive);
        yield return load;
        SceneManager.UnloadSceneAsync("MainWorld");
    }
}
