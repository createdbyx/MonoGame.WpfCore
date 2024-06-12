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
    public MonoGameViewModel()
    {
    }

    public void Dispose()
    {
        this.Content?.Dispose();
    }

    private IGraphicsDeviceService graphicsDeviceService;

    public IGraphicsDeviceService GraphicsDeviceService
    {
        get => this.graphicsDeviceService;
        set => this.SetField(ref this.graphicsDeviceService, value);
    }
    
    protected GraphicsDevice GraphicsDevice => this.GraphicsDeviceService?.GraphicsDevice;
    protected MonoGameServiceProvider Services { get; private set; }
    protected ContentManager Content { get; set; }

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

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        this.OnPropertyChanged(propertyName);
        return true;
    }
}