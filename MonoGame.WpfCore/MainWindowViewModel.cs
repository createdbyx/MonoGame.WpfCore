using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.WpfCore.MonoGameControls;

namespace MonoGame.WpfCore;

public class MainWindowViewModel : MonoGameViewModel
{
    private Vector2 _origin;
    private Vector2 _position;
    private float _rotation;
    private Vector2 _scale;
    private SpriteBatch _spriteBatch;
    private Texture2D _texture;

    public override void LoadContent()
    {
        this._spriteBatch = new SpriteBatch(this.GraphicsDevice);
        this._texture = this.Content.Load<Texture2D>("monogame-logo");
    }

    public override void Update(GameTime gameTime)
    {
        this._position = this.GraphicsDevice.Viewport.Bounds.Center.ToVector2();
        this._rotation = (float)Math.Sin(gameTime.TotalGameTime.TotalSeconds) / 4f;
        this._origin = this._texture.Bounds.Center.ToVector2();
        this._scale = Vector2.One;
    }

    public override void Draw(GameTime gameTime)
    {
        this.GraphicsDevice.Clear(Color.CornflowerBlue);

        this._spriteBatch.Begin();
        this._spriteBatch.Draw(this._texture, this._position, null, Color.White, this._rotation, this._origin, this._scale, SpriteEffects.None, 0f);
        this._spriteBatch.End();
    }
}