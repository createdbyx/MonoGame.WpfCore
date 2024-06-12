using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.WpfCore.MonoGameControls;

public class MonoGameViewModel : INotifyPropertyChanged, IMonoGameViewModel
{
    private IGraphicsDeviceService graphicsDeviceService;

    protected GraphicsDevice GraphicsDevice
    {
        get
        {
            return this.GraphicsDeviceService?.GraphicsDevice;
        }
    }

    /// <summary>
    /// Gets or sets an instance of <see cref="MonoGameServiceProvider"/>.
    /// This provides functionalities to implement dependency injection in MonoGame applications.
    /// This property is set during the initialization of the game.
    /// </summary>
    protected MonoGameServiceProvider Services { get; private set; }

    protected ContentManager Content { get; set; }

    public void Dispose()
    {
        this.Content?.Dispose();
    }

    public IGraphicsDeviceService GraphicsDeviceService
    {
        get => this.graphicsDeviceService;
        set => this.SetField(ref this.graphicsDeviceService, value);
    }
    
    public virtual void Initialize()
    {
        this.Services = new MonoGameServiceProvider();
        this.Services.AddService(this.GraphicsDeviceService);
        this.Content = new ContentManager(this.Services) { RootDirectory = "Content" };
    }

    public virtual void LoadContent()
    {
    }

    public virtual void UnloadContent()
    {
    }

    public virtual void Update(GameTime gameTime)
    {
    }

    public virtual void Draw(GameTime gameTime)
    {
    }

    public virtual void OnActivated(object sender, EventArgs args)
    {
    }

    public virtual void OnDeactivated(object sender, EventArgs args)
    {
    }

    public virtual void OnExiting(object sender, EventArgs args)
    {
    }

    public virtual void SizeChanged(object sender, SizeChangedEventArgs args)
    {
    }

    /// <summary>
    /// Occurs when a property value changes.
    /// </summary>
    /// <remarks>
    /// This event is meant to be used with the INotifyPropertyChanged interface 
    /// to signal the fact that a property has changed, and thereby causing any 
    /// bindings to this property to be re-evaluated.
    /// </remarks>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary> 
    /// Triggers the <c>PropertyChanged</c> event. 
    /// </summary> 
    /// <param name="propertyName">The name of the property that changed.</param>
    /// <remarks> 
    /// If the <c>PropertyChanged</c> event has subscribers, then this method invokes the event with 
    /// a new instance of <c>PropertyChangedEventArgs</c> containing the name of the property that changed.
    /// </remarks>
    /// <example> 
    /// This method is usually called by properties in the view model in a setter block like:
    /// <code> 
    /// public int SomeProperty
    /// {
    ///    get => _someProperty;
    ///    set
    ///    {
    ///        _someProperty = value;
    ///        OnPropertyChanged();
    ///    }
    /// }
    /// </code> 
    /// </example> 
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    /// <summary>
    /// Sets the value of a field and notifies listeners that a property value has changed.
    /// </summary>
    /// <typeparam name="T">The type of the field.</typeparam>
    /// <param name="field">The field whose value needs to be set.</param>
    /// <param name="value">The new value to be set.</param>
    /// <param name="propertyName">The name of the property that changed. This is optional and it is set by default
    /// to the name of the calling member, that is, the property that is getting changed.</param>
    /// <returns><c>true</c> if the value has changed, otherwise <c>false</c>.</returns>
    /// <remarks>
    /// This method is for properties that raise the <c>PropertyChanged</c> event. The method uses
    /// <c>CallerMemberName</c> attribute that allows you to obtain the method or property name
    /// of the caller to the method, eliminating the need to explicitly supply it.
    /// If the new value is same as the old value, the method simply returns <c>false</c>. Otherwise, it
    /// sets the new value, triggers <c>PropertyChanged</c> event to notify listeners and returns <c>true</c>.
    /// </remarks>
    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
        {
            return false;
        }

        field = value;
        this.OnPropertyChanged(propertyName);
        return true;
    }
}