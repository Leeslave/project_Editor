using System;
using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class DayIntro : MonoBehaviour
{
    /**
    *   하루 시작 인트로 스크립트
        : 씬이 재로딩 될때마다(날짜 변경)마다 실행
    */
    [SerializeField]
    private AudioSource textSFX;   // 인트로용 효과음
    public float textDelay;   // 글자 딜레이
    public float textOnDelay;   // 글자 출력 후 딜레이
    [SerializeField]
    private TMP_Text textUI;      //글자 활성화용 텍스트 오브젝트
    private string[] dayText;      // 실제 날짜 글자
    public bool isFinished = false;     // 인트로 종료 여부

    void OnEnable()
    {
        StartCoroutine(LoadScene("MainWorld"));
    }


    private IEnumerator LoadScene(string scene)
    {
        // 인트로 실행
        StopAllCoroutines();
        StartCoroutine(DayCountIntro());
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(scene);

        if (asyncLoad is null)
        {
            throw new ArgumentException("Invalid Scene");
        }
        asyncLoad.allowSceneActivation = false;
        
        while (asyncLoad.progress <  0.9f)
        {
            yield return null;
        }   

        Debug.Log($"Scene Loaded : {scene}");
        asyncLoad.allowSceneActivation = true;
        yield return new WaitUntil(() => isFinished == true);
    }


    /// <summary>
    /// 날짜 인트로 코루틴
    /// </summary>
    /// <returns></returns>
    private IEnumerator DayCountIntro()
    {
        // 플래그 설정
        isFinished = false;
        
        // 텍스트 UI 설정
        textUI.text = "";
        textUI.gameObject.SetActive(true);

        // 텍스트 세팅
        DailyData today = GameSystem.Instance.dayData;
        dayText = new string[] { "", "", ""};
        dayText[0] = $"제국력 {today.date.year}년 {today.date.month}월 {today.date.day}일";
        dayText[1] = today.dayTimes[GameSystem.Instance.timeIndex].ToString();
        dayText[2] = today.startLocation.name;

        // 한 글자씩 애니메이션
        for(int i = 0; i < 3; i++)
        {
            foreach(char c in dayText[i])
            {
                if (c != ' ')
                {
                    textSFX.Play();
                    yield return new WaitForSeconds(textDelay);
                }
                textUI.text += c;
            }
            textUI.text += "\n";
        }        

        // 대사 출력 후 딜레이
        yield return new WaitForSeconds(textOnDelay);

        //종료 및 flag 설정
        isFinished = true;
    }
}
