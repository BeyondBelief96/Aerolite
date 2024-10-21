using Flat.Graphics;
using Flat.Input;
using Microsoft.Xna.Framework;
using AeroliteSharpEngine;
using AeroliteSharpEngine.ForceGenerators;
using AeroliteSharpEngine.AeroMath;
using System;

namespace AeroliteEngine2DTestbed.Scenes
{
    public class BasicGravityTestScene : Scene
    {
        private AeroWorld2D _world;
        private AeroParticle2D _massiveBody;
        private AeroParticle2D _smallBody;
        private const float GRAVITY_CONSTANT = 100;
        private Vector2 _dragStart;
        private bool _isDragging = false;

        public BasicGravityTestScene(Game game, Screen screen, Sprites sprites, Shapes shapes)
            : base(game, screen, sprites, shapes)
        {
            _world = new AeroWorld2D(0);

            // Create bodies in world coordinates
            _massiveBody = new AeroParticle2D(_screen.Width / 2, _screen.Height / 2, 100_000);
            _massiveBody.Radius = 30;
            _world.AddParticle(_massiveBody);

            float orbitRadius = 500;
            _smallBody = new AeroParticle2D(_screen.Width / 2 + orbitRadius, _screen.Height / 2, 500);
            _smallBody.Radius = 10;

            float orbitSpeed = MathF.Sqrt((GRAVITY_CONSTANT * _massiveBody.Mass) / orbitRadius);
            _smallBody.Velocity = new AeroVec2(0, -orbitSpeed * 0.1f);

            _world.AddParticle(_smallBody);

            var gravityGen = new GravitationalForceGenerator(_massiveBody, GRAVITY_CONSTANT);
            _world.AddForceGenerator(_smallBody, gravityGen);
        }

        public override void Update(GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Handle mouse input in screen coordinates
            if (FlatMouse.Instance.IsLeftMouseButtonPressed())
            {
                Vector2 mouseScreen = FlatMouse.Instance.GetMouseScreenPosition(_game, _screen);
                Vector2 mouseWorld = Transforms.ScreenToWorld(mouseScreen.X, mouseScreen.Y, _screen);
                Vector2 bodyPos = new Vector2(_smallBody.Position.X, _smallBody.Position.Y);
                if (Vector2.Distance(mouseWorld, bodyPos) < _smallBody.Radius * 2)
                {
                    _isDragging = true;
                    _dragStart = mouseWorld;
                    _smallBody.Velocity = new AeroVec2(0, 0);
                }
            }

            if (!_isDragging)
            {
                _world.Update(dt);
            }
            else if (FlatMouse.Instance.IsLeftMouseButtonDown())
            {
                Vector2 mouseScreen = FlatMouse.Instance.GetMouseScreenPosition(_game, _screen);
                Vector2 mouseWorld = Transforms.ScreenToWorld(mouseScreen.X, mouseScreen.Y, _screen);
                _smallBody.Position = new AeroVec2(mouseWorld.X, mouseWorld.Y);
            }
            else if (FlatMouse.Instance.IsLeftMouseButtonReleased())
            {
                _isDragging = false;
                Vector2 mouseScreen = FlatMouse.Instance.GetMouseScreenPosition(_game, _screen);
                Vector2 mouseWorld = Transforms.ScreenToWorld(mouseScreen.X, mouseScreen.Y, _screen);
                Vector2 throwVector = (mouseWorld - _dragStart) * 2;
                _smallBody.Velocity = new AeroVec2(throwVector.X, throwVector.Y);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            _screen.Set();
            _game.GraphicsDevice.Clear(Color.Black);

            _shapes.Begin(_camera);

            // Convert world coordinates to screen coordinates for drawing
            Vector2 massiveBodyScreen = Transforms.WorldToScreen(_massiveBody.Position, _screen);
            Vector2 smallBodyScreen = Transforms.WorldToScreen(_smallBody.Position, _screen);

            // Draw massive body
            _shapes.DrawCircle(
                massiveBodyScreen,
                _massiveBody.Radius,
                32,
                Color.Red
            );

            // Draw small body
            _shapes.DrawCircle(
                smallBodyScreen,
                _smallBody.Radius,
                16,
                Color.Blue
            );

            // Draw debug line between bodies
            _shapes.DrawLine(
                massiveBodyScreen,
                smallBodyScreen,
                Color.Yellow
            );

            _shapes.End();

            _screen.Unset();
            _screen.Present(_sprites);
        }
    }
}