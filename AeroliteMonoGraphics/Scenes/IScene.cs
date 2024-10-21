using Microsoft.Xna.Framework;

namespace AeroliteMonoGraphics.Scenes
{
    public interface IScene
    {
        void Initialize(Game game);
        void LoadContent();
        void Update(GameTime gameTime);
        void Draw();
        void HandleInput();
        void Unload();
    }
}
