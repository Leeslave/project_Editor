using UnityEngine;
using TMPro;
using System;
using Unity.VisualScripting;
using UnityEditor.Build.Content;
using UnityEngine.SceneManagement;

public class DebugConsole : SingletonObject<DebugConsole>
{
    public GameObject console;
    public TMP_Text consoleText;
    public TMP_InputField input;

    new void Awake()
    {
        base.Awake();
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
                    output += $"WORK: {work.code}, Stage: {work.stage} = Done: {todayData.workList[work]}";
                }
                output += "\n";
                break;
            case "playerData" :
                SaveData player = GameSystem.Instance.player;
                output += $"Current Date Index: {GameSystem.Instance.gameData.date}\n";
                output += $"Current location: {GameSystem.Instance.gameData.location}\n";
                output += $"Current time: {GameSystem.Instance.gameData.time}\n";
                output += $"Current renown: {player.renown}\n";
                break;
            case "worldObjects":
                for(int i = 0; i < ObjectDatabase.List.Count; i++)
                {
                    output += $"{(World)i}지역 Object : {ObjectDatabase.List[i].Count}개\n";
                }
                break;
            case "help" :
                output += $"todayData: show today's Date, Work Status\n";
                output += $"playerData: show current date Index, location, time and renown\n";
                output += $"worldObjects: show every world's object counts\n";
                output += $"clear: clear all today works\n";
                break;
            case "clear" :
                foreach(var work in GameSystem.Instance.today.workList.Keys)
                {
                    GameSystem.Instance.today.workList[work] = true;
                }
                GameSystem.LoadScene("Screen");
                break;
            case "DaySkip":
                GameSystem.Instance.SetDate();
                SceneManager.LoadScene("DayLoading");
                break;
        }

        consoleText.text = "";
        consoleText.text += output;
    }
}
