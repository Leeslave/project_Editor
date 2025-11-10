using GameService;
using System.Collections.Generic;
using UnityEngine;

public class ServiceContainer : SingletonObject<ServiceContainer>
{
    private List<IDayService> dayService = new();
    private List<ILocationService> locationService = new();
    private List<IObjectDataService> objectService = new();
    private List<ISaveService> saveService = new();
    private List<IWorkService> workService = new();

    
    /// <summary>
    /// 서비스 등록 함수
    /// </summary>
    /// <param name="service">등록할 서비스</param>
    /// <param name="isDefault">디폴트 서비스 여부</param>
    /// <typeparam name="T">서비스 타입</typeparam>
    public void Register<T>(T service, bool isDefault = false) where T : IService
    {
        if (service is IDayService day)
        {
            if (isDefault) dayService[0] = day;
            else dayService.Add(day);
        }
        else if (service is ILocationService locate)
        {
            if (isDefault) locationService[0] = locate;
            else locationService.Add(locate);
        }
        else if (service is IObjectDataService objData)
        {
            if (isDefault) objectService[0] = objData;
            else objectService.Add(objData);
        }
        else if (service is ISaveService save)
        {
            if (isDefault) saveService[0] = save;
            else saveService.Add(save);
        }
        else if (service is IWorkService work)
        {
            if (isDefault) workService[0] = work;
            else workService.Add(work);
        }
        else
        {
            Debug.Log("Invalid service type");
        }
    }

    /// <summary>
    /// 서비스 해제 함수
    /// </summary>
    /// <param name="service">등록 해제할 서비스</param>
    /// <typeparam name="T">서비스 타입</typeparam>
    public void UnRegister<T>(T service) where T : IService
    {
        if (service is IDayService day)
        {
            dayService.Remove(day);
        }
        else if (service is ILocationService locate)
        {
            locationService.Remove(locate);
        }
        else if (service is IObjectDataService objData)
        {
            objectService.Remove(objData);
        }
        else if (service is ISaveService save)
        {
            saveService.Remove(save);
        }
        else if (service is IWorkService work)
        {
            workService.Remove(work);
        }
        else
        {
            Debug.Log("Invalid service type");
        }
    }
    
    /// <summary>
    /// 서비스 할당
    /// </summary>
    /// <typeparam name="T">할당받을 서비스 타입</typeparam>
    /// <returns>서비스</returns>
    public T Resolve<T>() where T : IService
    {
        if (typeof(T) == typeof(IDayService))
        {
            return (T)dayService[^1];
        }
        if (typeof(T) == typeof(ILocationService))
        {
            return (T)locationService[^1];
        }
        if  (typeof(T) == typeof(IObjectDataService))
        {
            return (T)objectService[^1];
        }
        if (typeof(T) == typeof(ISaveService))
        {
            return (T)saveService[^1];
        }
        if (typeof(T) == typeof(IWorkService))
        {
            return (T)workService[^1];
        }
        
        Debug.LogWarning("Invalid service type");
        return default;
    }
}
