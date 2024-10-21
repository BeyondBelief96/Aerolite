using Flat.Graphics;
using Flat.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace AeroliteEngine2DTestbed.Scenes
{
    public class TestScene : Scene  
    {
        public TestScene(Game game, Screen screen, Sprites sprites, Shapes shapes)
        : base(game, screen, sprites, shapes)
        {

        }

        public override void Update(GameTime gameTime)
        {
            // Camera zoom controls
            if (FlatKeyboard.Instance.IsKeyDown(Keys.A))
            {
                _camera.IncZoom();
            }
            if (FlatKeyboard.Instance.IsKeyDown(Keys.Z))
            {
                _camera.DecZoom();
            }
        }

        public override void Draw(GameTime gameTime)
        {
            _screen.Set();
            _game.GraphicsDevice.Clear(new Color(50, 60, 70));
            _shapes.Begin(_camera);
            _shapes.DrawCircle(0, 0, 32, 32, Color.White);
            _shapes.End();
            _screen.Unset();
            _screen.Present(_sprites);
        }
    }
}
