using GameService;
using System;
using System.Collections.Generic;
using UnityEngine;

public static class ServiceProvider
{
    // 서비스 객체 딕셔너리
    private static readonly Dictionary<Type, object> services = new();
    
    public static event Action<IService> OnServiceRegistered;

    /// <summary>
    /// 서비스 등록
    /// </summary>
    /// <param name="service">해당 서비스 객체</param>
    /// <typeparam name="T">서비스 타입</typeparam>
    public static void Register<T>(T service) where T : IService
    {
        Type type = typeof(T);
        if (services.ContainsKey(type))
        {
            Debug.LogError($"Service already registered: {type.Name}");
        }
        services.Add(type, service);
        
        OnServiceRegistered?.Invoke(service);
    }
    
    /// <summary>
    /// 서비스 등록 해제
    /// </summary>
    /// <typeparam name="T">서비스 타입</typeparam>
    public static void Unregister<T>() where T : IService
    {
        Type type = typeof(T);
        services.Remove(type);
    }
    
    /// <summary>
    /// 서비스 불러오기
    /// </summary>
    /// <typeparam name="T">불러올 서비스 타입</typeparam>
    /// <returns>해당 서비스 객체</returns>
    /// <exception cref="Exception">해당하는 서비스타입이 등록되어있지 않을때</exception>
    public static T Get<T>(Action<T> func = null) where T : IService
    {
        Type type = typeof(T);
    
        // 1. 이미 있는 경우 즉시 실행
        if (services.TryGetValue(type, out var service))
        {
            func?.Invoke((T)service);
            return (T)service;
        }
    
        // 2. Lazy Init (DataService 등)
        if (typeof(IDataService).IsAssignableFrom(type))
        {
            var newInstance = new DataController();
            func?.Invoke((T)(object)newInstance);
            return (T)(object)newInstance; 
        }
    
        // 3. 실패 시: 이벤트 어댑터 생성 및 구독
        if (func != null)
        {
            // 임시 핸들러 생성
            Action<IService> handler = null;
            handler = (registeredType) => 
            {
                // 등록된 타입이 내가 기다리던 T와 일치하는지 확인
                if (registeredType is T)
                {
                    // 다시 Get을 호출하거나 딕셔너리에서 꺼내서 실행
                    T foundService = Get<T>(); 
                    func.Invoke(foundService);

                    // 이벤트 해제
                    OnServiceRegistered -= handler;
                }
            };
            OnServiceRegistered += handler;
        }
        else
        {
            throw new Exception($"Service not registered and no callback provided: {type.Name}");
        }

        return default;
    }
}
