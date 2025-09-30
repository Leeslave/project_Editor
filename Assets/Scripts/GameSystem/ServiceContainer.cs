using GameService;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServiceContainer : SingletonObject<ServiceContainer>
{
    public List<IDayService> dayService = new();
    public List<ILocationService> locationService = new();
    public List<IObjectDataService> objectService = new();
    public List<ISaveService> saveService = new();
    public List<IWorkService> workService = new();

    /// <summary>
    /// 서비스 등록 함수
    /// </summary>
    /// <param name="service">등록할 서비스</param>
    /// <param name="isDefault">디폴트 서비스 여부</param>
    /// <typeparam name="T">서비스 타입</typeparam>
    public void Register<T>(T service, bool isDefault = false)
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
    
    public T Reslove<T>()
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
            
        }
    }
}
