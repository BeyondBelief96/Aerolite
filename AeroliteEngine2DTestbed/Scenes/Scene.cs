using Flat.Graphics;
using Microsoft.Xna.Framework;

namespace AeroliteEngine2DTestbed.Scenes
{
    public abstract class Scene
    {
        protected double _nextLogTime = 0;  // Track when to log next
        protected const double LOG_INTERVAL = 1.0;  // Log every second

        protected Game _game;
        protected Screen _screen;
        protected Sprites _sprites;
        protected Shapes _shapes;
        protected Camera _camera;

        public Scene(Game game, Screen screen, Sprites sprites, Shapes shapes)
        {
            _game = game;
            _screen = screen;
            _sprites = sprites;
            _shapes = shapes;
            _camera = new Camera(screen);
            _camera.Zoom = 1;
        }


        public abstract void Update(GameTime gameTime);
        public abstract void Draw(GameTime gameTime);
    }
}
