using UnityEngine;

namespace Utility
{
    public static class EditorLogger
    {
        /// <summary>
        /// 일반 로그 출력
        /// </summary>
        public static void Log<T>(T message)
        {
            #if DEBUG
            Debug.Log($"[LOG] {message?.ToString()}");
            #endif
            // NOTE: External Log Trace
        }

        /// <summary>
        /// 주의 로그 출력
        /// </summary>
        public static void LogWarning<T>(T message)
        {
            #if DEBUG
            Debug.LogWarning($"[WARNING] {message?.ToString()}");
            #endif
            // NOTE: External Warning Trace
        }
        
        /// <summary>
        /// 경고 로그 출력
        /// </summary>
        public static void LogError<T>(T message)
        {
            #if DEBUG
            Debug.LogError($"[ERROR] {message?.ToString()}");
            #endif
            // NOTE: External Error Trace
        }
    }
}