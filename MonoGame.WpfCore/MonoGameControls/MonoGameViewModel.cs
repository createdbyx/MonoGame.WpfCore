using System;
using System.Windows;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.WpfCore.MonoGameControls;

public interface IMonoGameViewModel : IDisposable
{
    IGraphicsDeviceService GraphicsDeviceService { get; set; }

    void Initialize();
    void LoadContent();
    void UnloadContent();
    void Update(GameTime gameTime);
    void Draw(GameTime gameTime);
    void OnActivated(object sender, EventArgs args);
    void OnDeactivated(object sender, EventArgs args);
    void OnExiting(object sender, EventArgs args);

    void SizeChanged(object sender, SizeChangedEventArgs args);
}

public class MonoGameViewModel : ViewModel, IMonoGameViewModel
{
    protected GraphicsDevice GraphicsDevice
    {
        get
        {
            return this.GraphicsDeviceService?.GraphicsDevice;
        }
    }

    protected MonoGameServiceProvider Services { get; private set; }
    protected ContentManager Content { get; set; }

    public void Dispose()
    {
        this.Content?.Dispose();
    }

    public IGraphicsDeviceService GraphicsDeviceService { get; set; }

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
}