using GameService;
using System;
using System.Collections.Generic;
using UnityEngine;

public static class ServiceProvider
{
    // 서비스 객체 딕셔너리
    private static readonly Dictionary<Type, object> services = new();

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
        Debug.Log($"Registered service: {type.Name}");
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
    public static T Get<T>() where T : IService
    {
        Type type = typeof(T);
        
        if (services.TryGetValue(type, out var service))
        {
            return (T)service;
        }
        
        // DataService에 한해 Lazy Init 적용 (단순 POCO)
        if (typeof(T) == typeof(IDataService))
        {
            var newInstance = new DataController();
            return (T)(object)newInstance;              // T 캐스팅을 위해서 
        }
        
        throw new Exception($"Service not registered: {type.Name}");
    }
}
