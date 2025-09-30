using GameService;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServiceContainer : MonoBehaviour
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
            if (isDefault) dayService[0] = servi

            dayService.Add(day);
        }
        else if (service is ILocationService locate)
        {
            if (isDefault) locationService[0] = locate;
            else locationService.Add(locate);
        }
    }
}
