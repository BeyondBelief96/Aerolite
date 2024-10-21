using Flat.Graphics;
using Flat.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using AeroliteSharpEngine;
using AeroliteSharpEngine.ForceGenerators;
using AeroliteSharpEngine.AeroMath;
using System.Linq;
using System.Collections.Generic;
using System;

namespace AeroliteEngine2DTestbed.Scenes
{
    public class SolarSystemScene : Scene
    {
        private AeroWorld2D _world;
        private AeroParticle2D _sun;
        private List<(AeroParticle2D planet, float orbitRadius, float orbitSpeed)> _planets;
        private const float GRAVITY_CONSTANT = 50;
        private Vector2 _lastMousePos;
        private bool _dragging = false;
        private Random _random = new Random();

        // Planet colors for variety
        private readonly Color[] _planetColors = new Color[]
        {
            new Color(255, 198, 73),   // Mercury - yellowish
            new Color(255, 198, 198),  // Venus - pale pink
            new Color(100, 149, 237),  // Earth - blue
            new Color(193, 68, 14),    // Mars - red
            new Color(255, 187, 51),   // Jupiter - orange
            new Color(238, 232, 205),  // Saturn - pale yellow
            new Color(173, 216, 230),  // Uranus - light blue
            new Color(0, 0, 128)       // Neptune - dark blue
        };

        public SolarSystemScene(Game game, Screen screen, Sprites sprites, Shapes shapes)
            : base(game, screen, sprites, shapes)
        {
            _world = new AeroWorld2D(0); // No global gravity
            _planets = new List<(AeroParticle2D, float, float)>();

            InitializeSolarSystem();
        }

        private void InitializeSolarSystem()
        {
            // Reduce gravity constant
            const float GRAVITY_CONSTANT = 5f;

            // Create sun with increased mass
            _sun = new AeroParticle2D(_screen.Width / 2, _screen.Height / 2, 10000);
            _sun.Radius = 40;
            _sun.IsStatic = true;
            _world.AddParticle(_sun);

            // Create planets with varying sizes and orbits
            float[] orbitRadii = { 100, 150, 200, 250, 350, 450, 550, 650 };
            float[] planetMasses = { 0.1f, 0.2f, 0.2f, 0.15f, 0.8f, 0.6f, 0.4f, 0.4f };
            float[] planetSizes = { 5, 8, 8, 6, 20, 16, 12, 12 };

            for (int i = 0; i < 8; i++)
            {
                float angle = (float)(_random.NextDouble() * MathHelper.TwoPi);
                float orbitRadius = orbitRadii[i];

                // Calculate initial position
                float x = _sun.Position.X + orbitRadius * (float)Math.Cos(angle);
                float y = _sun.Position.Y + orbitRadius * (float)Math.Sin(angle);

                var planet = new AeroParticle2D(x, y, planetMasses[i]);
                planet.Radius = planetSizes[i];

                // Calculate orbital velocity using Kepler's Third Law
                float orbitPeriod = 2 * MathHelper.Pi * (float)Math.Sqrt(orbitRadius * orbitRadius * orbitRadius / (GRAVITY_CONSTANT * _sun.Mass));
                float orbitSpeed = 2 * MathHelper.Pi * orbitRadius / orbitPeriod;

                planet.Velocity = new AeroVec2(
                    -orbitSpeed * (float)MathF.Sin(angle),
                    orbitSpeed * (float)MathF.Cos(angle)
                );

                _world.AddParticle(planet);
                _planets.Add((planet, orbitRadius, orbitSpeed));

                // Add gravitational force generator
                var gravityGen = new GravitationalForceGenerator(_sun, GRAVITY_CONSTANT);
                _world.AddForceGenerator(planet, gravityGen);
            }

            // Add some asteroids in the asteroid belt
            AddAsteroidBelt(300, 320, 50);
        }

        private void AddAsteroidBelt(float minRadius, float maxRadius, int count)
        {
            for (int i = 0; i < count; i++)
            {
                float angle = (float)(_random.NextDouble() * MathHelper.TwoPi);
                float radius = minRadius + (float)_random.NextDouble() * (maxRadius - minRadius);

                float x = _sun.Position.X + radius * (float)Math.Cos(angle);
                float y = _sun.Position.Y + radius * (float)Math.Sin(angle);

                var asteroid = new AeroParticle2D(x, y, 0.01f);
                asteroid.Radius = 1 + (float)_random.NextDouble() * 2;

                // Calculate orbital velocity using Kepler's Third Law
                float orbitPeriod = 2 * MathHelper.Pi * (float)Math.Sqrt(radius * radius * radius / (GRAVITY_CONSTANT * _sun.Mass));
                float orbitSpeed = 2 * MathHelper.Pi * radius / orbitPeriod;

                asteroid.Velocity = new AeroVec2(
                    -orbitSpeed * (float)Math.Sin(angle),
                    orbitSpeed * (float)Math.Cos(angle)
                );

                _world.AddParticle(asteroid);
                var gravityGen = new GravitationalForceGenerator(_sun, GRAVITY_CONSTANT);
                _world.AddForceGenerator(asteroid, gravityGen);
            }
        }

        public override void Update(GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Camera controls
            if (FlatKeyboard.Instance.IsKeyDown(Keys.A)) _camera.DecZoom();
            if (FlatKeyboard.Instance.IsKeyDown(Keys.Z)) _camera.IncZoom();

            // Sun dragging
            //HandleSunDragging();

            _world.Update(dt);
        }

        private void HandleSunDragging()
        {
            if (FlatMouse.Instance.IsLeftMouseButtonPressed())
            {
                _lastMousePos = FlatMouse.Instance.GetMouseScreenPosition(_game, _screen);
                _dragging = true;
            }
            if (FlatMouse.Instance.IsLeftMouseButtonReleased())
            {
                _dragging = false;
            }
            if (_dragging)
            {
                Vector2 currentPos = FlatMouse.Instance.GetMouseScreenPosition(_game, _screen);
                Vector2 delta = currentPos - _lastMousePos;
                _sun.Position = new AeroVec2(_sun.Position.X + delta.X, _sun.Position.Y + delta.Y);
                _lastMousePos = currentPos;
            }
        }

        public override void Draw(GameTime gameTime)
        {
            _screen.Set();
            _game.GraphicsDevice.Clear(new Color(2, 2, 10));

            _shapes.Begin(_camera);

            _shapes.DrawCircleFill(
                Transforms.WorldToScreen(_sun.Position, _screen),
                _sun.Radius,
                32,
                Color.Yellow
            );

            // Draw orbit trails
            for (int i = 0; i < _planets.Count; i++)
            {
                var (planet, orbitRadius, _) = _planets[i];
                _shapes.DrawCircle(
                    Transforms.WorldToScreen(_sun.Position, _screen),
                    orbitRadius,
                    64,
                    new Color(255, 255, 255, 20)
                );
            }

            // Draw all particles
            foreach (var particle in _world.GetParticles())
            {
                if (particle == _sun) continue;

                var planetInfo = _planets.FirstOrDefault(p => p.planet == particle);
                if (planetInfo.planet != null)
                {
                    int index = _planets.IndexOf(planetInfo);
                    Color planetColor = _planetColors[index];
                    _shapes.DrawCircleFill(
                        Transforms.WorldToScreen(particle.Position, _screen),
                        particle.Radius,
                        16,
                        planetColor
                    );
                }
                else
                {
                    _shapes.DrawCircleFill(
                        Transforms.WorldToScreen(particle.Position, _screen),
                        particle.Radius,
                        8,
                        new Color(180, 180, 180)
                    );
                }
            }

            _shapes.End();

            _screen.Unset();
            _screen.Present(_sprites);
        }
    }
}