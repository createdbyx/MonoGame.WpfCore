using System;
using System.Windows;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.WpfCore.MonoGameControls;

/// <summary>
/// Represents a ViewModel interface for a MonoGame control.
/// </summary>
public interface IMonoGameViewModel : IDisposable
{
    /// <summary>
    /// Represents a service that provides access to the graphics device.
    /// </summary>
    IGraphicsDeviceService GraphicsDeviceService { get; set; }

    /// <summary>
    /// Initializes the ViewModel by creating necessary services.
    /// </summary>
    void Initialize();

    /// <summary>
    /// Loads the content for the MonoGame control.
    /// </summary>
    void LoadContent();

    /// <summary>
    /// Unloads the content for the MonoGame control.
    /// </summary>
    void UnloadContent();

    /// <summary>
    /// Updates the ViewModel by performing any necessary calculations or updates based on the current game time.
    /// </summary>
    /// <param name="gameTime">The elapsed time since the last update.</param>
    void Update(GameTime gameTime);

    /// <summary>
    /// Draws the content for the MonoGame control.
    /// </summary>
    /// <param name="gameTime">The elapsed time since the last draw.</param>
    void Draw(GameTime gameTime);

    /// <summary>
    /// Executes when the control is activated and raises the OnActivated event.
    /// </summary>
    /// <param name="sender">The object that raised the event.</param>
    /// <param name="args">The event arguments.</param>
    void OnActivated(object sender, EventArgs args);

    /// <summary>
    /// Executes when the control is deactivated and raises the OnDeactivated event.
    /// </summary>
    /// <param name="sender">The object that raised the event.</param>
    /// <param name="args">The event arguments.</param>
    void OnDeactivated(object sender, EventArgs args);

    /// <summary>
    /// Event handler for when the application is exiting.
    /// </summary>
    /// <param name="sender">The object that triggered the event.</param>
    /// <param name="args">The event arguments.</param>
    void OnExiting(object sender, EventArgs args);

    /// <summary>
    /// Executes when the size of the control changes and raises the SizeChanged event.
    /// </summary>
    /// <param name="sender">The object that raised the event.</param>
    /// <param name="args">The event arguments.</param>
    void SizeChanged(object sender, SizeChangedEventArgs args);
}