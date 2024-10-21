using AeroliteMonoGraphics.Graphics;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

public class Screen : IDisposable
{
    private bool _disposed;
    private readonly Game _game;
    private RenderTarget2D _renderTarget;
    private bool _renderTargetSet;
    private readonly SpriteBatch _spriteBatch;

    public readonly int Width;
    public readonly int Height;

    public Screen(Game game, int width, int height)
    {
        _disposed = false;
        _game = game ?? throw new ArgumentNullException(nameof(game));
        Width = width;
        Height = height;
        _renderTarget = new RenderTarget2D(game.GraphicsDevice, Width, Height);
        _spriteBatch = new SpriteBatch(game.GraphicsDevice);
    }

    public void BeginDraw()
    {
        if (_renderTargetSet)
            throw new Exception("Screen is already in drawing state.");

        _game.GraphicsDevice.SetRenderTarget(_renderTarget);
        _game.GraphicsDevice.Clear(Color.Black);  // Clear render target
        _renderTargetSet = true;
    }


    public void EndDraw()
    {
        if (!_renderTargetSet)
            throw new Exception("BeginDraw must be called before EndDraw.");

        _game.GraphicsDevice.SetRenderTarget(null);
        _renderTargetSet = false;

        // Draw render target to screen
        _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
        _spriteBatch.Draw(_renderTarget, Vector2.Zero, Color.White);
        _spriteBatch.End();
    }

    public void Draw(SpriteBatch targetBatch)
    {
        targetBatch.Draw(_renderTarget, Vector2.Zero, Color.White);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed) return;

        if (disposing)
        {
            _renderTarget?.Dispose();
            _spriteBatch?.Dispose();
        }

        _disposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}