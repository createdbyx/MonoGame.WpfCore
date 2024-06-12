using System;
using System.Collections.Generic;

namespace MonoGame.WpfCore.MonoGameControls;

/// <summary>
/// Represents a service provider for a MonoGame application.
/// </summary>
public class MonoGameServiceProvider : IServiceProvider
{
    private readonly Dictionary<Type, object> services;

    /// <summary>
    /// Initializes a new instance of the <see cref="MonoGameServiceProvider"/> class.
    /// </summary>
    /// <remarks>
    /// This constructor creates a new instance of the <see cref="Dictionary{TKey, TValue}"/> class for services field,
    /// a private field used for storing instances of services.
    /// </remarks>
    public MonoGameServiceProvider()
    {
        this.services = new Dictionary<Type, object>();
    }

    /// <summary>
    /// Retrieves the service object of the specified type.
    /// </summary>
    /// <param name="type">The type of the service object to get.</param>
    /// <returns>
    /// A service object of type <paramref name="type"/>.
    /// -or- 
    /// null if there is no service object of type <paramref name="type"/>.
    /// </returns>
    public object GetService(Type type)
    {
        if (this.services.TryGetValue(type, out var service))
        {
            return service;
        }

        return null;
    }

    /// <summary>
    /// Adds the given service provider to the services collection for the specified service type.
    /// </summary>
    /// <param name="type">The type of service to add.</param>
    /// <param name="provider">The provider of the service to add.</param>
    /// <exception cref="ArgumentNullException">Occurs when either the <paramref name="type"/> or the <paramref name="provider"/> is null.</exception>
    /// <exception cref="ArgumentException">Occurs when a service provider has already been registered for <paramref name="type"/>.</exception>
    /// <remarks>
    /// The AddService method is provided to allow services to be added to the internal services collection. 
    /// The services are stored as key-value pairs, with the service type as the key and the service provider as the value.
    ///</remarks>
    public void AddService(Type type, object provider)
    {
        this.services.Add(type, provider);
    }

    public void RemoveService(Type type)
    {
        this.services.Remove(type);
    }

    public void AddService<T>(T service)
    {
        this.AddService(typeof(T), service);
    }

    public T GetService<T>() where T : class
    {
        var service = this.GetService(typeof(T));
        return (T)service;
    }
}