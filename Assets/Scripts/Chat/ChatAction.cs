using System;


/// 반응 함수
public abstract class ChatAction
{
    public string param;
    public abstract int Invoke();
}

public class DayChangeAction : ChatAction
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

public class TimeChangeAction : ChatAction
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

public class ChatJumpAction : ChatAction
{
    public override int Invoke()
    {
        if(int.TryParse(param, out int chatIndex))
        {
            Chat.Instance.index = chatIndex - 1;
            return 0;
        }
        else
        {
            return -1;
        }
        
    }
}