#if DEBUG

using UnityEngine;
using TMPro;

public class DebugConsole : SingletonObject<DebugConsole>
{
    public GameObject console;
    public ScrollText consoleText;
    public TMP_InputField input;

    
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
        string[] inputArgs = input.text.Split(" ");
        switch(inputArgs[0])
        {
            case "showDay" :
                DailyData dayData;
                if (inputArgs.Length < 2) // no additional args : today
                {
                    dayData = GameSystem.Instance.today;
                }
                else if (int.TryParse(inputArgs[1], out int date)) // with args : certain date
                {
                    dayData = GameSystem.Instance.GetDailyData(date);
                    if (dayData == null)
                    {
                        output += "<color=#ff0000>Invalid date</color>";
                        break;
                    }
                }
                else // invalid args
                {
                    output += "<color=#ff0000>Invalid Parameter</color>";
                    break;
                }
                output += $"{dayData.date.year}년 {dayData.date.month}월 {dayData.date.day}일\n";
                output += $"해당 날짜의 업무\n";
                foreach(var work in dayData.workList.Keys)
                {
                    output += $"WORK: {work.code}, Stage: {work.stage} = Done: {dayData.workList[work]}\n";
                }
                output += "\n";
                break;
            
            case "showData" :
                SaveData player = GameSystem.Instance.player;
                GameData gameData = GameSystem.Instance.gameData;
                output += $"Current Day: {gameData.date} - {gameData.time}\n";
                output += $"Current location: {gameData.location} - {gameData.position}\n";
                output += $"Current renown: {player.renown}\n";
                break;
            
            case "setDay":
                if (inputArgs.Length < 2) // no additional args : nexttime
                {
                    output += "<color=#ff0000>date number required</color>";
                    break;
                }
                
                if (int.TryParse(inputArgs[1], out int dayCount)) // with args : certain date
                {
                    GameSystem.Instance.SetDate(dayCount);
                    GameSystem.LoadScene("DayLoading");
                }
                else // invalid args
                {
                    output += "<color=#ff0000>Invalid Parameter</color>";
                }
                break;
            
            case "setTime":
                if (inputArgs.Length < 2) // no additional args
                {
                    GameSystem.Instance.SetTime(GameSystem.Instance.gameData.time + 1);
                    GameSystem.LoadScene("DayLoading");
                    break;
                }
                
                if (int.TryParse(inputArgs[1], out int timeCount)) // with args : certain date
                {
                    GameSystem.Instance.SetTime(timeCount);
                    GameSystem.LoadScene("DayLoading");
                }
                else // invalid args
                {
                    output += "<color=#ff0000>Invalid Parameter</color>";
                }
                break;
            
            case "objectList":
                for(int i = 0; i < ObjectDatabase.ObjectList.Count; i++)
                {
                    output += $"{(World)i}지역 Object : {ObjectDatabase.ObjectList[i].Count}개\n";
                }
                break;
            
            case "clearTask" :
                foreach(var work in GameSystem.Instance.today.workList.Keys)
                {
                    GameSystem.Instance.ClearTask(work.code);
                    output += $"Clear work : {work.code}";
                }
                break;
            
            default:
                output += GetHelpCommand();
                break;
        }

        consoleText.AddMessage($"{input.text} : {output}");
    }

    private string GetHelpCommand()
    {
        string result = "<color=#00ff00>";
        result += $"showDay <day = today> : show Day data.\n";
        result += $"showData: show current date Index, location, time and renown\n";
        result += $"setDay <day> : skip to certain day\n";
        result += $"setTime <time = next Time> : skip one time\n";
        result += $"objectList: show every world's object counts\n";
        result += $"clearTask: clear all today works\n";

        result += "</color>";
        return result;
    }
}

#endif
