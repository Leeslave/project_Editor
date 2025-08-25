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
    public static GameAction GetAction(string func, string param)
    {
        GameAction result;

        switch (func)
        {
            case "JUMP":
                result = new ChatJumpGameAction();
                result.Param = SetParam<int>(param);
                return result;
            case "DAYCHANGE":
                result = new HardDayChangeGameAction();
                result.Param = SetParam<int>(param);
                return result;
            case "NEXTDAY":
                result = new DayChangeGameAction();
                return result;
            case "TIMECHANGE":
                result = new HardTimeChangeGameAction();
                result.Param = SetParam<int>(param);
                return result;
            case "NEXTTIME":
                result = new TimeChangeGameAction();
                result.Param = SetParam<int>(param);
                return result;
            case "TUTORIAL":
                result = new TutorialGameAction();
                result.Param = SetParam<int>(param);
                return result;
            case "REMOVE":
                result = new RemoveGameAction();
                result.Param = SetParam<string>(param);
                return result;
            case "EXIT":
                result = new ExitGameGameAction();
                return result;
            case "CHATSWAP":
                result = new ChatSwapGameAction();
                var chatData = SetParam<(string, string)>(param);
                result.Param = (WorldObjectFactory.Instance?.FindObject(chatData.Item1) as IChatList, int.Parse(chatData.Item2));
                return result;
            case "POSCHANGE":
                result = new PosChangeGameAction();
                var posData = SetParam<(string, string)>(param);
                result.Param = (Enum.Parse(typeof(World), posData.Item1), int.Parse(posData.Item2));
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
                return (T)(object)(parts[0].Trim(), parts[1].Trim());
            }
        }
    
        throw new InvalidOperationException("Unsupported type");
    }
}


/// 반응 함수
public abstract class GameAction
{
    public object Param;
    public abstract bool Invoke();
}


/// <summary>
/// 게임 종료 액션
/// </summary>
public class ExitGameGameAction : GameAction
{
    public override bool Invoke()
    {
        GameSystem.LoadScene("Start");
        return true;
    }
}


/// <summary>
/// 날짜 강제 변경 액션
/// </summary>
/// <remarks>Param 형식 : int</remarks>
public class HardDayChangeGameAction : GameAction
{
    public override bool Invoke()
    {
        if (Param is not int)
        {
            return false;
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
public class DayChangeGameAction : GameAction
{
    public override bool Invoke()
    {
        if(GameSystem.Instance.timeIndex == 3)
        {
            GameSystem.Instance.SetDate(-1);
            SceneManager.LoadScene("DayLoading");
            return true;
        }
        return false;
    }
}


/// <summary>
/// 시간대 강제 변경 액션
/// </summary>
/// <remarks>Param 형식 : int</remarks>
public class TimeChangeGameAction : GameAction
{
    public override bool Invoke()
    {
        if (Param is not int)
        {
            return false;
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
/// 시간대 변경 액션
/// </summary>
/// <remarks>Param 형식 : int</remarks>
public class HardTimeChangeGameAction : GameAction
{
    public override bool Invoke()
    {
        if (Param is int param)
        {
            GameSystem.Instance.SetTime(param);
            return true;
        }
        return false;
    }
}


/// <summary>
/// 대화 스킵 액션
/// </summary>
/// <remarks>Param 형식 : int</remarks>
public class ChatJumpGameAction : GameAction
{
    public override bool Invoke()
    {
        if (Param is not int)
        {
            return false;
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
public class ChatSwapGameAction : GameAction
{
    public override bool Invoke()
    {
        if(Param is (IChatList trigger, int idx))
        {
            trigger.SwapIndex(idx);
            return true;
        }
        return false;
    }
}


/// <summary>
/// 튜토리얼 생성 액션
/// </summary>
/// TODO:<remarks>Param 형식 : Tutorial 인터페이스</remarks>
public class TutorialGameAction : GameAction
{
    public override bool Invoke()
    {
        if(Param is int)
        {

            return true;
        }
        return false;
    }
}


/// <summary>
/// 위치 이동 액션
/// </summary>
/// <remarks></remarks>
public class PosChangeGameAction : GameAction
{
    public override bool Invoke()
    {
        if(Param is (World world, int idx))
        {
            WorldSceneManager.Instance.MoveLocation(world, idx);
            return true;
        }
        return false;
    }
}


/// <summary>
/// 오브젝트 삭제 액션
/// </summary>
/// <remarks>Param 형식 : string</remarks>
public class RemoveGameAction : GameAction
{
    public override bool Invoke()
    {
        if (Param is not string name)
        {
            return false;
        }

        WorldObjectFactory.Instance.RemoveObject(name);
        return true;
    }
}
