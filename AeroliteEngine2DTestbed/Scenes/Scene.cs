using Flat.Graphics;
using Microsoft.Xna.Framework;

namespace AeroliteEngine2DTestbed.Scenes
{
    public abstract class Scene
    {
        protected readonly Game Game;
        protected readonly Screen Screen;
        protected readonly Sprites Sprites;
        protected readonly Shapes Shapes;
        protected readonly Camera Camera;

        protected Scene(Game game, Screen screen, Sprites sprites, Shapes shapes)
        {
            Game = game;
            Screen = screen;
            Sprites = sprites;
            Shapes = shapes;
            Camera = new Camera(screen);
            Camera.Zoom = 1;
        }

        public abstract void Update(GameTime gameTime);
        protected abstract void DrawScene(GameTime gameTime);
        public void Draw(GameTime gameTime)
        {
            Screen.Set();
            Game.GraphicsDevice.Clear(new Color(10, 10, 20));
            Shapes.Begin(Camera);
            
            DrawScene(gameTime);
            
            Shapes.End();
            Screen.Unset();
            Screen.Present(Sprites);
        }
    }
}
