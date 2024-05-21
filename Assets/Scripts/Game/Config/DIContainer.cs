using System;
using System.Collections.Generic;

public static class DIContainer
{
    private static readonly Dictionary<Type, object> _services = new();

    public static void Register<T>(T service)
    {
        var type = typeof(T);
        if (_services.ContainsKey(type))
        {
            _services[type] = service;
        }
        else
        {
            _services.Add(type, service);
        }
    }

    public static T Resolve<T>()
    {
        return (T)_services[typeof(T)];
    }
}
