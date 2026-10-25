// Only the Unity host is stubbed. Data classes and progression services compile from the real sources.
using System;
using System.Collections.Generic;

namespace UnityEngine
{
    public class MonoBehaviour { }
    [AttributeUsage(AttributeTargets.Field)] public sealed class SerializeField : Attribute { }
    public struct Vector2 { public Vector2(float x, float y) { } }
}

public enum World { Street, Bar, Cafe, Restaurant, Temple, Hallway, Office, Office3, Interrogate, Max }
public static class GameSystem
{
    private static readonly Dictionary<Type, object> Services = new();
    public static T GetService<T>() where T : class => Services[typeof(T)] as T;
    public static void RegisterService<T>(T service) => Services[typeof(T)] = service;
    public static void UnRegister<T>() => Services.Remove(typeof(T));
}
namespace Utility
{
    public static class EditorLogger { public static void Log(string value) { } }
}
