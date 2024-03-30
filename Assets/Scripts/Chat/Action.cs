using System;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;


/// 반응 함수
public abstract class Action
{
    public string param;
    public abstract int Invoke();
}


// 날짜 변경 액션
public class DayChangeAction : Action
{
    public override int Invoke()
    {
        if(int.TryParse(param, out int dayIndex))
        {
            GameSystem.Instance.SetDate(dayIndex);
            SceneManager.LoadScene("DayLoading");
            return 0;
        }
        else
        {
            return -1;
        }
    }
}


// 시간대 변경 액션
public class TimeChangeAction : Action
{
    public override int Invoke()
    {
        if(int.TryParse(param, out int time))
        {
            if (GameSystem.Instance.gameData.time != time - 1)
            {
                return -1;
            }
            GameSystem.Instance.SetTime(time);
            return 0;
        }
        else
        {
            return -1;
        }
    }
}


// 시간대 강제 변경 액션
public class HardTimeChangeAction : Action
{
    public override int Invoke()
    {
        if(int.TryParse(param, out int time))
        {
            GameSystem.Instance.SetTime(time);
            return 0;
        }
        else
        {
            return -1;
        }
    }
}


// 대화 스킵 액션
public class ChatJumpAction : Action
{
    public override int Invoke()
    {
        if(int.TryParse(param, out int count))
        {
            for (int i = 0; i < count; i++)
            {
                Chat.Instance.SkipChat();
            }
            return 0;
        }
        else
        {
            return -1;
        }
        
    }
}


// 오브젝트 삭제 액션
public class RemoveAction : Action
{
    public override int Invoke()
    {
        param = param.Replace(" ", "");
        string[] paramList = param.Split(',');

        int position;
        string name = paramList[1];

        if (!int.TryParse(paramList[0], out position))
        {
            return -1;
        }
        WorldSceneManager.Instance.CurrentLocation.RemoveObject(position, name);
        return 0;
    }
}