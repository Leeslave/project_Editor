using GameService;
using System;
using System.Collections.Generic;
using Utility;

namespace GameAction
{
    public static class ActionHandler
    {
        // Action 생성 Delegate 관리
        private static readonly Dictionary<string, Func<string, IGameAction>> Factory = new();

        static ActionHandler()
        {
            // JUMP
            Register("JUMP", s =>
            {
                int param = int.TryParse(s, out int num) ? num : 0;
                return new ChatJumpAction(param);
            });

            // DaySwitch : Force switch Day
            Register("DAYCHANGE", s =>
            {
                int param = int.TryParse(s, out int num) ? num : -1;
                return new DaySwitchAction(param);
            });

            // NEXTDAY
            Register("NEXTDAY", _ => new NextDayAction());

            // TimeSwitch
            Register("TIMECHANGE", s =>
            {
                int param = int.TryParse(s, out int num) ? num : -1;
                return new TimeSwitchAction(param);
            });

            // NEXTTIME
            Register("NEXTTIME", s =>
            {
                int param = int.TryParse(s, out int num) ? num : -1;
                return new NextTimeAction(param);
            });

            // REMOVE : Remove Object Action
            Register("REMOVE", s => new RemoveObjectAction(s));

            // CHATSWAP
            Register("CHATSWAP", s =>
            {
                // NOTE: 월드 객체 탐색 관리 필요 (ObjectService에서 static으로 관리하는 방식
                var str = s.Split(new[] { ' ', ',', '.' }, StringSplitOptions.RemoveEmptyEntries);
                IChatList obj = WorldObjectFactory.Instance?.FindObject(str[0]) as IChatList;
                int index = int.Parse(str[1]);
                return new ChatSwapAction(obj, index);
            });

            // POSCHANGE
            Register("POSCHANGE", s =>
            {
                var str = s.Split(new[] { ' ', ',', '.' }, StringSplitOptions.RemoveEmptyEntries);
                World world = Enum.Parse<World>(str[0]);
                int index = int.Parse(str[1]);
                return new MovePositionAction(world, index);
            });

            // TUTORIAL
            Register("TUTORIAL", s =>
            {
                int param = int.TryParse(s, out int num) ? num : -1;
                return new TutorialAction(param);
            });
            
            // RENOWN
            Register("RENOWN", s =>
            {
                int param = int.TryParse(s, out int num) ? num : 0;
                return new SetRenownAction(param);
            });

            // EXIT
            Register("EXIT", _ => new ExitGameAction());
        }

        private static void Register(string key, Func<string, IGameAction> factory)
            => Factory.Add(key, factory);

        /// <summary>
        /// 액션 레코드 생성
        /// </summary>
        /// <param name="func">생성할 레코드 타입</param>
        /// <param name="param">레코드 매개변수</param>
        /// <returns>실행 가능한 액션 서비스</returns>
        public static IGameAction Create(string func, string param)
        {
            // 빈 문자열, none 예외 처리
            if (string.IsNullOrEmpty(func) || func.Equals("none", StringComparison.OrdinalIgnoreCase))
            {
                return new NotImpletedAction();
            }

            if (Factory.TryGetValue(func, out var factory))
            {
                return factory(param);
            }

            EditorLogger.LogError($"Unknown function: {func}");
            return null;
        }
    }

    /// 반응 함수
    public interface IGameAction
    {
        public bool Invoke();
    }

    /// <summary>
    /// 미구현 Null 대리 객체
    /// </summary>
    public record NotImpletedAction : IGameAction
    {
        public bool Invoke() => false;
    }


    /// <summary>
    /// 게임 종료 액션
    /// </summary>
    public record ExitGameAction : IGameAction
    {
        public bool Invoke()
        {
            GameSystem.Instance.EnterScene("GameStart");
            return true;
        }
    }


    /// <summary>
    /// 날짜 강제 변경 액션
    /// </summary>
    /// <remarks>Param 형식 : int</remarks>
    public record DaySwitchAction(int Day) : IGameAction
    {
        public bool Invoke()
        {
            // input Exception
            if (Day < 0) return false;
            
            GameSystem.SaveService.SelectDay(Day, 0);
            return true;
        }
    }

    /// <summary>
    /// 날짜 변경 액션
    /// </summary>
    public record NextDayAction : IGameAction
    {
        public bool Invoke()
        {
            GameSystem.SaveService.SwitchDay();
            return true;
        }
    }


    /// <summary>
    /// 시간대 변경 액션
    /// </summary>
    /// <remarks>Param 형식 : int</remarks>
    public record NextTimeAction(int Time) : IGameAction
    {
        public bool Invoke()
        {
            IDayService dayService = GameSystem.GetService<IDayService>();
            if (dayService.Time != Time - 1)
            {
                return false;
            }

            dayService.Time = Time;
            return true;
        }
    }

    /// <summary>
    /// 시간대 강제 변경 액션
    /// </summary>
    /// <remarks>Param 형식 : int</remarks>
    public record TimeSwitchAction(int Time) : IGameAction
    {
        public bool Invoke()
        {
            if (Time < 0) return false;
            
            IDayService dayService = GameSystem.GetService<IDayService>();
            dayService.Time = Time;
            return true;
        }
    }


    /// <summary>
    /// 대화 스킵 액션
    /// </summary>
    /// <remarks>Param 형식 : int</remarks>
    public record ChatJumpAction(int Count) : IGameAction
    {
        public bool Invoke()
        {
            if (Count <= 0) return false;
            
            for (int i = 0; i < Count; i++)
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
    public record ChatSwapAction(IChatList ChatObj, int Index) : IGameAction
    {
        public bool Invoke()
        {
            ChatObj.SwapIndex(Index);
            return true;
        }
    }


    /// <summary>
    /// 튜토리얼 생성 액션
    /// </summary>
    /// TODO:<remarks>Param 형식 : Tutorial 인터페이스</remarks>
    public record TutorialAction( /*ITutorial*/int Tutorial) : IGameAction
    {
        public bool Invoke()
        {
            if (Tutorial < 0) return false;
            EditorLogger.Log($"TUTORIAL: {Tutorial}");
            return true;
        }
    }


    /// <summary>
    /// 위치 이동 액션
    /// </summary>
    /// <param name="World">이동할 위치</param>
    /// <param name="Index">이동할 위치 내 좌표</param>
    public record MovePositionAction(World World, int Index) : IGameAction
    {
        public bool Invoke()
        {
            ILocationService location = GameSystem.GetService<ILocationService>();
            if (location == null) return false;

            location.MoveLocation(new WorldVector(World, Index));
            return true;
        }
    }


    /// <summary>
    /// 오브젝트 삭제 액션
    /// </summary>
    /// <remarks>Param 형식 : string</remarks>
    public record RemoveObjectAction(string Name) : IGameAction
    {
        public bool Invoke()
        {
            WorldObjectFactory objFactory = WorldObjectFactory.Instance;
            if (!objFactory) return false;
            
            objFactory.RemoveObject(Name);
            return true;
        }
    }

    /// <summary>
    /// 명성치 변화 액션
    /// </summary>
    /// <param name="Amount">Param 형식 : int</param>
    public record SetRenownAction(int Amount) : IGameAction
    {
        public bool Invoke()
        {
            GameSystem.SaveService.Renown += Amount;
            return true;
        }
    }
}
