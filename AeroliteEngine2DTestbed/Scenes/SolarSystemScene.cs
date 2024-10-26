using AeroliteEngine2DTestbed.Helpers;
using AeroliteEngine2DTestbed.Scenes;
using AeroliteSharpEngine.AeroMath;
using AeroliteSharpEngine.Core;
using AeroliteSharpEngine.Integrators;
using Flat.Graphics;
using Flat.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

public class SolarSystemScene : Scene
{
    private const float GRAVITATIONAL_CONSTANT = 0.1f;  // Reduced from 1e-8
    private const float SUN_MASS = 1000f;              // Reduced from 10000
    private const float PLANET_MASS = 1f;              // Base mass for planets
    private const float ASTEROID_MASS = 0.1f;          // Small mass for asteroids
    private const float ORBIT_SPEED_MULTIPLIER = 1.0f; 

    private IAeroPhysicsWorld _world;
    private AeroParticle2D _sun;
    private List<AeroParticle2D> _planets;
    private List<AeroParticle2D> _asteroids;
    private Random _random = new Random();

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

    // Planet configuration
    private readonly (float radius, float mass, float size)[] PLANET_CONFIGS = new[]
    {
        (100f, PLANET_MASS * 0.4f, 5f),    // Mercury
        (150f, PLANET_MASS * 0.8f, 8f),    // Venus
        (200f, PLANET_MASS * 1.0f, 8f),    // Earth
        (250f, PLANET_MASS * 0.5f, 6f),    // Mars
        (350f, PLANET_MASS * 4.0f, 20f),   // Jupiter
        (450f, PLANET_MASS * 3.0f, 16f),   // Saturn
        (550f, PLANET_MASS * 2.0f, 12f),   // Uranus
        (650f, PLANET_MASS * 2.0f, 12f)    // Neptune
    };

    public SolarSystemScene(Game game, Screen screen, Sprites sprites, Shapes shapes)
        : base(game, screen, sprites, shapes)
    {
        _world = new AeroWorld2D(0); // No global gravity
        _world.SetIntegrator(new EulerIntegrator());
        _planets = new List<AeroParticle2D>();
        _asteroids = new List<AeroParticle2D>();

        InitializeSolarSystem();
    }

    private void InitializeSolarSystem()
    {
        // Create sun with adjusted mass
        _sun = new AeroParticle2D(_screen.Width / 2, _screen.Height / 2, SUN_MASS);
        _sun.Radius = 40;
        _sun.Damping = 1.0f; // No damping for orbital mechanics
        _world.AddPhysicsObject(_sun);

        // Create planets using configurations
        for (int i = 0; i < PLANET_CONFIGS.Length; i++)
        {
            var (orbitRadius, mass, size) = PLANET_CONFIGS[i];
            float angle = (float)(_random.NextDouble() * MathHelper.TwoPi);

            float x = _sun.Position.X + orbitRadius * (float)Math.Cos(angle);
            float y = _sun.Position.Y + orbitRadius * (float)Math.Sin(angle);

            var planet = new AeroParticle2D(x, y, mass);
            planet.Radius = size;
            planet.Damping = 1.0f; // No damping for orbital mechanics

            // Calculate initial velocity for circular orbit
            // v = sqrt(GM/r) where G is grav constant, M is central mass, r is radius
            float orbitSpeed = ORBIT_SPEED_MULTIPLIER *
                (float)Math.Sqrt((GRAVITATIONAL_CONSTANT * SUN_MASS) / orbitRadius);

            AeroVec2 initialVelocity = new AeroVec2(
                -orbitSpeed * (float)Math.Sin(angle),
                orbitSpeed * (float)Math.Cos(angle)
            );

            planet.Velocity = initialVelocity;
            _world.AddPhysicsObject(planet);
            _planets.Add(planet);

            var gravityGen = new GravitationalForceGenerator(_sun, GRAVITATIONAL_CONSTANT);
            _world.AddForceGenerator(planet, gravityGen);
        }

        // Add asteroids with adjusted mass
        AddAsteroidBelt(300, 320, 50, ASTEROID_MASS);
    }

    private void AddAsteroidBelt(float minRadius, float maxRadius, int count, float mass)
    {
        for (int i = 0; i < count; i++)
        {
            float angle = (float)(_random.NextDouble() * MathHelper.TwoPi);
            float radius = minRadius + (float)_random.NextDouble() * (maxRadius - minRadius);

            float x = _sun.Position.X + radius * (float)Math.Cos(angle);
            float y = _sun.Position.Y + radius * (float)Math.Sin(angle);

            var asteroid = new AeroParticle2D(x, y, mass);
            asteroid.Radius = 1 + (float)_random.NextDouble() * 2;
            asteroid.Damping = 1.0f; // No damping for orbital mechanics

            // Calculate proper orbital velocity
            float orbitSpeed = ORBIT_SPEED_MULTIPLIER *
                (float)Math.Sqrt((GRAVITATIONAL_CONSTANT * SUN_MASS) / radius);

            asteroid.Velocity = new AeroVec2(
                -orbitSpeed * (float)Math.Sin(angle),
                orbitSpeed * (float)Math.Cos(angle)
            );

            _world.AddPhysicsObject(asteroid);
            _asteroids.Add(asteroid);

            var gravityGen = new GravitationalForceGenerator(_sun, GRAVITATIONAL_CONSTANT);
            _world.AddForceGenerator(asteroid, gravityGen);
        }
    }

    public override void Update(GameTime gameTime)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        // Camera controls
        if (FlatKeyboard.Instance.IsKeyDown(Keys.A)) _camera.DecZoom();
        if (FlatKeyboard.Instance.IsKeyDown(Keys.Z)) _camera.IncZoom();

        _world.Update(dt);
    }

    public override void Draw(GameTime gameTime)
    {
        _screen.Set();
        _game.GraphicsDevice.Clear(new Color(2, 2, 10));
        _shapes.Begin(_camera);

        // Draw orbit trails
        for (int i = 0; i < _planets.Count; i++)
        {
            float orbitRadius = Vector2.Distance(
                new Vector2(_sun.Position.X, _sun.Position.Y),
                new Vector2(_planets[i].Position.X, _planets[i].Position.Y)
            );

            // Convert sun's screen position to render position
            Vector2 sunRenderPos = CoordinateSystem.ScreenToRender(
                new Vector2(_sun.Position.X, _sun.Position.Y),
                _screen.Width,
                _screen.Height
            );

            _shapes.DrawCircle(
                sunRenderPos,
                orbitRadius,
                64,
                new Color(255, 255, 255, 20)
            );
        }

        // Draw sun
        _shapes.DrawCircleFill(
            CoordinateSystem.ScreenToRender(
                new Vector2(_sun.Position.X, _sun.Position.Y),
                _screen.Width,
                _screen.Height
            ),
            _sun.Radius,
            32,
            Color.Yellow
        );

        // Draw planets
        for (int i = 0; i < _planets.Count; i++)
        {
            _shapes.DrawCircleFill(
                CoordinateSystem.ScreenToRender(
                    new Vector2(_planets[i].Position.X, _planets[i].Position.Y),
                    _screen.Width,
                    _screen.Height
                ),
                _planets[i].Radius,
                16,
                _planetColors[i]
            );
        }

        // Draw asteroids
        foreach (var asteroid in _asteroids)
        {
            _shapes.DrawCircleFill(
                CoordinateSystem.ScreenToRender(
                    new Vector2(asteroid.Position.X, asteroid.Position.Y),
                    _screen.Width,
                    _screen.Height
                ),
                asteroid.Radius,
                8,
                new Color(180, 180, 180)
            );
        }

        _shapes.End();
        _screen.Unset();
        _screen.Present(_sprites);
    }
}