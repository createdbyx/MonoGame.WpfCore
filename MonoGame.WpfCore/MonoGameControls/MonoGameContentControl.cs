// Copyright (c) 2010-2013 SharpDX - Alexandre Mutel
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SharpDX.Direct3D9;
using Texture = SharpDX.Direct3D9.Texture;

namespace MonoGame.WpfCore.MonoGameControls;

public sealed class MonoGameContentControl : ContentControl, IDisposable
{
    private static readonly MonoGameGraphicsDeviceService _graphicsDeviceService = new();
    private readonly GameTime _gameTime = new();
    private readonly Stopwatch _stopwatch = new();
    private D3DImage _direct3DImage;
    private int _instanceCount;
    private bool _isFirstLoad = true;
    private bool _isInitialized;
    private RenderTarget2D _renderTarget;
    private Texture _renderTargetD3D9;
    private IMonoGameViewModel _viewModel;

    public MonoGameContentControl()
    {
        if (DesignerProperties.GetIsInDesignMode(this))
        {
            return;
        }

        this._instanceCount++;
        this.Loaded += this.OnLoaded;
        this.Unloaded += this.OnUnloaded;
        this.DataContextChanged += (sender, args) =>
        {
            this._viewModel = args.NewValue as IMonoGameViewModel;

            if (this._viewModel != null)
            {
                this._viewModel.GraphicsDeviceService = _graphicsDeviceService;
            }
        };
        this.SizeChanged += (sender, args) => this._viewModel?.SizeChanged(sender, args);
    }

    public static GraphicsDevice GraphicsDevice
    {
        get
        {
            return _graphicsDeviceService?.GraphicsDevice;
        }
    }

    public bool IsDisposed { get; private set; }

    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (this.IsDisposed)
        {
            return;
        }

        this._viewModel?.Dispose();
        this._renderTarget?.Dispose();
        this._renderTargetD3D9?.Dispose();
        this._instanceCount--;

        if (this._instanceCount <= 0)
        {
            _graphicsDeviceService?.Dispose();
        }

        this.IsDisposed = true;
    }

    ~MonoGameContentControl()
    {
        this.Dispose(false);
    }

    protected override void OnGotFocus(RoutedEventArgs e)
    {
        this._viewModel?.OnActivated(this, EventArgs.Empty);
        base.OnGotFocus(e);
    }

    protected override void OnLostFocus(RoutedEventArgs e)
    {
        this._viewModel?.OnDeactivated(this, EventArgs.Empty);
        base.OnLostFocus(e);
    }

    private void Start()
    {
        if (this._isInitialized)
        {
            return;
        }

        if (Application.Current.MainWindow == null)
        {
            throw new InvalidOperationException("The application must have a MainWindow");
        }

        Application.Current.MainWindow.Closing += (sender, args) => this._viewModel?.OnExiting(this, EventArgs.Empty);
        Application.Current.MainWindow.ContentRendered += (sender, args) =>
        {
            if (this._isFirstLoad)
            {
                _graphicsDeviceService.StartDirect3D(Application.Current.MainWindow);
                this._viewModel?.Initialize();
                this._viewModel?.LoadContent();
                this._isFirstLoad = false;
            }
        };

        this._direct3DImage = new D3DImage();

        this.AddChild(new Image { Source = this._direct3DImage, Stretch = Stretch.None });

        //_direct3DImage.IsFrontBufferAvailableChanged += OnDirect3DImageIsFrontBufferAvailableChanged;

        this._renderTarget = this.CreateRenderTarget();
        CompositionTarget.Rendering += this.OnRender;
        this._stopwatch.Start();
        this._isInitialized = true;
    }

    protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
    {
        base.OnRenderSizeChanged(sizeInfo);

        // sometimes OnRenderSizeChanged happens before OnLoaded.
        this.Start();
        this.ResetBackBufferReference();
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        this.Start();
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        this._viewModel?.UnloadContent();

        if (_graphicsDeviceService != null)
        {
            CompositionTarget.Rendering -= this.OnRender;
            this.ResetBackBufferReference();
            _graphicsDeviceService.DeviceResetting -= this.OnGraphicsDeviceServiceDeviceResetting;
        }
    }

    private void OnGraphicsDeviceServiceDeviceResetting(object sender, EventArgs e)
    {
        this.ResetBackBufferReference();
    }

    private void ResetBackBufferReference()
    {
        if (DesignerProperties.GetIsInDesignMode(this))
        {
            return;
        }

        if (this._renderTarget != null)
        {
            this._renderTarget.Dispose();
            this._renderTarget = null;
        }

        if (this._renderTargetD3D9 != null)
        {
            this._renderTargetD3D9.Dispose();
            this._renderTargetD3D9 = null;
        }

        this._direct3DImage.Lock();
        this._direct3DImage.SetBackBuffer(D3DResourceType.IDirect3DSurface9, IntPtr.Zero);
        this._direct3DImage.Unlock();
    }

    private RenderTarget2D CreateRenderTarget()
    {
        var actualWidth = (int)this.ActualWidth;
        var actualHeight = (int)this.ActualHeight;

        if (actualWidth == 0 || actualHeight == 0)
        {
            return null;
        }

        if (GraphicsDevice == null)
        {
            return null;
        }

        var renderTarget = new RenderTarget2D(GraphicsDevice, actualWidth, actualHeight,
                                              false, SurfaceFormat.Bgra32, DepthFormat.Depth24Stencil8, 1,
                                              RenderTargetUsage.PlatformContents, true);

        var handle = renderTarget.GetSharedHandle();

        if (handle == IntPtr.Zero)
        {
            throw new ArgumentException("Handle could not be retrieved");
        }

        this._renderTargetD3D9 = new Texture(_graphicsDeviceService.Direct3DDevice, renderTarget.Width,
                                             renderTarget.Height,
                                             1, Usage.RenderTarget, Format.A8R8G8B8,
                                             Pool.Default, ref handle);

        using (var surface = this._renderTargetD3D9.GetSurfaceLevel(0))
        {
            this._direct3DImage.Lock();
            this._direct3DImage.SetBackBuffer(D3DResourceType.IDirect3DSurface9, surface.NativePointer);
            this._direct3DImage.Unlock();
        }

        return renderTarget;
    }

    private void OnRender(object sender, EventArgs e)
    {
        this._gameTime.ElapsedGameTime = this._stopwatch.Elapsed;
        this._gameTime.TotalGameTime += this._gameTime.ElapsedGameTime;
        this._stopwatch.Restart();

        if (this.CanBeginDraw())
        {
            try
            {
                this._direct3DImage.Lock();

                if (this._renderTarget == null)
                {
                    this._renderTarget = this.CreateRenderTarget();
                }

                if (this._renderTarget != null)
                {
                    GraphicsDevice.SetRenderTarget(this._renderTarget);
                    this.SetViewport();

                    this._viewModel?.Update(this._gameTime);
                    this._viewModel?.Draw(this._gameTime);

                    GraphicsDevice.Flush();
                    this._direct3DImage.AddDirtyRect(new Int32Rect(0, 0, (int)this.ActualWidth, (int)this.ActualHeight));
                }
            }
            finally
            {
                this._direct3DImage.Unlock();
                GraphicsDevice.SetRenderTarget(null);
            }
        }
    }

    private bool CanBeginDraw()
    {
        // If we have no graphics device, we must be running in the designer.
        if (_graphicsDeviceService == null)
        {
            return false;
        }

        if (!this._direct3DImage.IsFrontBufferAvailable)
        {
            return false;
        }

        // Make sure the graphics device is big enough, and is not lost.
        if (!this.HandleDeviceReset())
        {
            return false;
        }

        return true;
    }

    private void SetViewport()
    {
        // Many GraphicsDeviceControl instances can be sharing the same
        // GraphicsDevice. The device backbuffer will be resized to fit the
        // largest of these controls. But what if we are currently drawing
        // a smaller control? To avoid unwanted stretching, we set the
        // viewport to only use the top left portion of the full backbuffer.
        var width = Math.Max(1, (int)this.ActualWidth);
        var height = Math.Max(1, (int)this.ActualHeight);
        GraphicsDevice.Viewport = new Viewport(0, 0, width, height);
    }

    private bool HandleDeviceReset()
    {
        if (GraphicsDevice == null)
        {
            return false;
        }

        var deviceNeedsReset = false;

        switch (GraphicsDevice.GraphicsDeviceStatus)
        {
            case GraphicsDeviceStatus.Lost:
                // If the graphics device is lost, we cannot use it at all.
                return false;

            case GraphicsDeviceStatus.NotReset:
                // If device is in the not-reset state, we should try to reset it.
                deviceNeedsReset = true;
                break;
        }

        if (deviceNeedsReset)
        {
            _graphicsDeviceService.ResetDevice((int)this.ActualWidth, (int)this.ActualHeight);
            return false;
        }

        return true;
    }
}