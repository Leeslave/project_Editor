using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class ActionHandler
{
    /// <summary>
    /// 반응 함수 할당
    /// </summary>
    /// <param name="func"></param>
    /// <param name="param"></param>
    /// <returns></returns>
    public static Action GetAction(string func, string param)
    {
        Action result;

        switch (func)
        {
            case "Jump":
                result = new ChatJumpAction();
                result.Param = SetParam<int>(param);
                return result;
            case "DayChange":
                result = new HardDayChangeAction();
                result.Param = SetParam<int>(param);
                return result;
            case "TimeChange":
                result = new TimeChangeAction();
                result.Param = SetParam<int>(param);
                return result;
            case "Tutorial":
                result = new TutorialAction();
                result.Param = SetParam<int>(param);
                return result;
            case "Remove":
                result = new RemoveAction();
                result.Param = SetParam<string>(param);
                return result;
            case "ExitGame":
                result = new ExitGameAction();
                return result;
            case "ChatSwap":
                result = new ChatSwapAction();
                var data = SetParam<(string, string)>(param);
                result.Param = (WorldObjectFactory.Instance?.FindObject(data.Item1) as IChatList, int.Parse(data.Item2));
                return result;
            default:
                return null;
        }
    }


    /// <summary>
    /// 액션 매개변수 형식 파싱
    /// </summary>
    /// <param name="param">문자열 형식의 매개변수</param>
    /// <typeparam name="T">매개변수 형식</typeparam>
    /// <returns>파싱된 매개변수</returns>
    /// <exception cref="InvalidOperationException">잘못된 매개변수 형식</exception>
    private static T SetParam<T>(string param)
    {
        // 단순 객체일 경우
        if (typeof(T) == typeof(int))
        {
            return (T)(object)int.Parse(param);
        }
        else if (typeof(T) == typeof(string))
        {
            return (T)(object)param;
        }
        else if (typeof(T).IsInterface) // 특정 인터페이스인 경우
        {
            // 튜토리얼 타입 지원
            // if (typeof(T) == typeof(ITutorial))
            // {
            //     return (T)(object)Convert.ChangeType(param, typeof(T));
            // }
        }
        
        // 튜플인 경우
        else if (typeof(T).IsGenericType && typeof(T).GetGenericTypeDefinition() == typeof(ValueTuple<,>))
        {
            var parts = param.Split(',', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 2)
            {
                Debug.Log($"param: {param} - {parts[0].Trim()}, {parts[1].Trim()}");
                return (T)(object)(parts[0].Trim(), parts[1].Trim());
            }
        }
    
        throw new InvalidOperationException("Unsupported type");
    }
}


/// 반응 함수
public abstract class Action
{
    public object Param;
    public abstract object Invoke();
}


/// <summary>
/// 게임 종료 액션
/// </summary>
public class ExitGameAction : Action
{
    public override object Invoke()
    {
        GameSystem.LoadScene("Start");
        return true;
    }
}


/// <summary>
/// 날짜 강제 변경 액션
/// </summary>
/// <remarks>Param 형식 : int</remarks>
public class HardDayChangeAction : Action
{
    public override object Invoke()
    {
        if (Param is not int)
        {
            return "Error";
        }
        
        GameSystem.Instance.SetDate((int)Param);
        
        // TODO: 로딩씬 진입
        // SceneManager.LoadScene("DayLoading");
        WorldSceneManager.Instance.ReloadWorld();
        return true;
    }
}

/// <summary>
/// 날짜 변경 액션
/// </summary>
/// <remarks>Param 형식 : int</remarks>
public class DayChangeAction : Action
{
    public override object Invoke()
    {
        if (Param is not int)
        {
            return "Error";
        }
        
        if(GameSystem.Instance.timeIndex == 3)
        {
            GameSystem.Instance.SetDate((int)Param);
            SceneManager.LoadScene("DayLoading");
            return true;
        }
        return false;
    }
}


/// <summary>
/// 시간대 변경 액션
/// </summary>
/// <remarks>Param 형식 : int</remarks>
public class TimeChangeAction : Action
{
    public override object Invoke()
    {
        if (Param is not int)
        {
            return "Error";
        }

        if (GameSystem.Instance.timeIndex != (int)Param - 1)
        {
            return false;
        }
        
        GameSystem.Instance.SetTime((int)Param);
        return true;
    }
}


/// <summary>
/// 시간대 강제 변경 액션
/// </summary>
/// <remarks>Param 형식 : int</remarks>
public class HardTimeChangeAction : Action
{
    public override object Invoke()
    {
        if (Param is not int)
        {
            return "Error";
        }
        
        GameSystem.Instance.SetTime((int)Param);
        return true;
    }
}


/// <summary>
/// 대화 스킵 액션
/// </summary>
/// <remarks>Param 형식 : int</remarks>
public class ChatJumpAction : Action
{
    public override object Invoke()
    {
        if (Param is not int)
        {
            return "Error";
        }
        
        for (int i = 0; i < (int)Param; i++)
        {
            Chat.Instance.SkipChat();
        }
        return true;
    }
}


/// <summary>
/// 대화 스킵 액션
/// </summary>
/// <remarks>Param 형식 : IChatList, string</remarks>
public class ChatSwapAction : Action
{
    public override object Invoke()
    {
        if(Param is (IChatList trigger, int idx))
        {
            trigger.SwapIndex(idx);
            return true;
        }
        return "Error";
    }
}


/// <summary>
/// 튜토리얼 생성 액션
/// </summary>
/// TODO:<remarks>Param 형식 : Tutorial 인터페이스</remarks>
public class TutorialAction : Action
{
    public override object Invoke()
    {
        if(Param is int)
        {
            ChatTutorialManager.Get().Show((int) Param);
            return true;
        }
        return "Error";
    }
}


/// <summary>
/// 오브젝트 삭제 액션
/// </summary>
/// <remarks>Param 형식 : string</remarks>
public class RemoveAction : Action
{
    public override object Invoke()
    {
        if (Param is not string name)
        {
            return "Error";
        }

        WorldObjectFactory.Instance.RemoveObject(name);
        return true;
    }
}
