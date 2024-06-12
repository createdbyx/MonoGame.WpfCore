using System;
using System.Collections.Generic;

namespace MonoGame.WpfCore.MonoGameControls;

public class MonoGameServiceProvider : IServiceProvider
{
    private readonly Dictionary<Type, object> _services;

    public MonoGameServiceProvider()
    {
        this._services = new Dictionary<Type, object>();
    }

    public void AddService(Type type, object provider)
    {
        this._services.Add(type, provider);
    }

    public object GetService(Type type)
    {
        if (this._services.TryGetValue(type, out var service))
            return service;

        return null;
    }

    public void RemoveService(Type type)
    {
        this._services.Remove(type);
    }

    public void AddService<T>(T service)
    {
        this.AddService(typeof(T), service);
    }

    public T GetService<T>() where T : class
    {
        var service = this.GetService(typeof(T));
        return (T) service;
    }
}