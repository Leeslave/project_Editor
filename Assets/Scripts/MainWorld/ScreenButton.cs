using UnityEngine;
using UnityEngine.UI;

public class ScreenButton : MonoBehaviour
{
    /**
    *   스크린 활성화, 비활성화 트리거 코드
    *   - 스크린 아이콘을 눌러 스크린을 활성화
    *   - down 버튼을 눌러 스크린 비활성화
    */

    /// <summary>
    /// 스크린을 눌러 확대 or 스크린 키기
    /// </summary>
    public void ClickScreen()
    {
        // 스스크린 활성화
        GameSystem.LoadScene("Screen");
    }
}
