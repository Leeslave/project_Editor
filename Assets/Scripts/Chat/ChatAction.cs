using System;


/// 반응 함수
public abstract class Action
{
    public string param;
    public abstract int Invoke();
}

public class DayChangeAction : Action
{
    public override int Invoke()
    {
        if(int.TryParse(param, out int dayIndex))
        {
            GameSystem.Instance.SetDate(dayIndex);
            return 0;
        }
        else
        {
            return -1;
        }
    }
}

public class TimeChangeAction : Action
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