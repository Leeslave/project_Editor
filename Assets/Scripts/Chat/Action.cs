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


// 게임 종료 액션
public class ExitGameAction : Action
{
    public override int Invoke()
    {
        GameSystem.LoadScene("Start");
        return 0;
    }
}


// 날짜 강제 변경 액션
public class HardDayChangeAction : Action
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

// 날짜 변경 액션
public class DayChangeAction : Action
{
    public override int Invoke()
    {
        if(int.TryParse(param, out int dayIndex))
        {
            if(GameSystem.Instance.timeIndex == 3)
            {
                GameSystem.Instance.SetDate(dayIndex);
                SceneManager.LoadScene("DayLoading");
                return 0;
            }

            return -1;
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
            if (GameSystem.Instance.timeIndex != time - 1)
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

// 대화 스킵 액션
public class TutorialAction : Action
{
    public override int Invoke()
    {
        if(int.TryParse(param, out int count))
        {
            ChatTutorialManager.Get().Show(count);
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
        
        if (!Enum.TryParse(paramList[1], out World location))
        {
            return -1;
        }
        WorldObjectFactory.Instance.RemoveObject(paramList[0], location);
        return 0;
    }
}