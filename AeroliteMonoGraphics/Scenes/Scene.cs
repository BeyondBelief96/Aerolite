using AeroliteMonoGraphics.Graphics;
using AeroliteMonoGraphics.Scenes;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

public abstract class Scene : IScene
{
    protected Game Game { get; private set; }
    protected Screen Screen { get; private set; }
    protected Camera Camera { get; private set; }
    protected Renderer Renderer { get; private set; }
    protected GraphicsDevice GraphicsDevice { get; private set; }
    protected GameTime GameTime { get; private set; }

    protected bool IsActive { get; private set; }
    protected float DeltaTime => (float)GameTime.ElapsedGameTime.TotalSeconds;

    public virtual void Initialize(Game game)
    {
        Game = game;
        GraphicsDevice = game.GraphicsDevice;
        Screen = new Screen(game, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
        Camera = new Camera(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
        Renderer = new Renderer(GraphicsDevice);
        IsActive = true;
    }

    public virtual void Draw()
    {
        if (!IsActive) return;

        Screen.BeginDraw();
        Renderer.Begin(Camera);

        DrawScene();

        Renderer.End();
        Screen.EndDraw();
    }

    protected abstract void DrawScene();

    public virtual void Update(GameTime gameTime)
    {
        if (!IsActive) return;
        GameTime = gameTime;

        HandleInput();
        UpdateScene();
    }

    protected abstract void UpdateScene();

    public virtual void Pause()
    {
        IsActive = false;
    }

    public virtual void Resume()
    {
        IsActive = true;
    }

    public virtual void Reset()
    {
        Camera = new Camera(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
    }

    public virtual void HandleViewportResize(int width, int height)
    {
        Camera = new Camera(width, height);
    }

    public abstract void HandleInput();
    public abstract void LoadContent();

    public virtual void Unload()
    {
        IsActive = false;
        Screen?.Dispose();
        Renderer?.Dispose();
    }
}