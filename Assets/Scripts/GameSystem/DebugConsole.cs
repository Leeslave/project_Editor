using UnityEngine;
using TMPro;

public class DebugConsole : MonoBehaviour
{
    public GameObject console;
    public TMP_Text consoleText;
    public TMP_InputField input;
    private static DebugConsole _instance;
    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            Debug.Log("ConsoleClicked");
            console.SetActive(!console.activeSelf);
        }
    }
    public void GetConsoleInput()
    {
        string output = "";
        switch(input.text)
        {
            case "todayData" :
                DailyData todayData = GameSystem.Instance.today;
                output += $"{todayData.date.year}년 {todayData.date.month}월 {todayData.date.day}일\n";
                output += $"오늘의 업무 현황\n";
                foreach(var work in todayData.workList.Keys)
                {
                    output += $"WORK: {work.code}, Stage: {work.stage.ToString()} = Done: {todayData.workList[work]}";
                }
                output += "\n";
                break;
            case "playerData" :
                SaveData player = GameSystem.Instance.player;
                DailyData today = GameSystem.Instance.today;
                output += $"Current Date Index: {GameSystem.Instance.date}\n";
                output += $"Current location: {today.startLocation}\n";
                output += $"Current time: {GameSystem.Instance.time}\n";
                output += $"Current renown: {player.renown}\n";
                break;
            case "help" :
                output += $"todayData: show today's Date, Work Status\n";
                output += $"playerData: show current date Index, location, time and renown\n";
                output += $"clear: clear all today works\n";
                break;
            case "clear" :
                foreach(var work in GameSystem.Instance.today.workList.Keys)
                {
                    GameSystem.Instance.today.workList[work] = true;
                }
                GameSystem.LoadScene("Screen");
                break;
        }

        consoleText.text = "";
        consoleText.text += output;
    }
}
