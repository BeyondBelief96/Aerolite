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
using AeroliteSharpEngine.Collisions.Detection.BroadPhase;
using AeroliteSharpEngine.Core.Interfaces;
using AeroliteSharpEngine.ForceGenerators;
using AeroliteSharpEngine.Shapes;

public class SolarSystemScene : Scene
{
    private const float GravitationalConstant = 0.1f;  // Reduced from 1e-8
    private const float SunMass = 1000f;              // Reduced from 10000
    private const float PlanetMass = 1f;              // Base mass for planets
    private const float AsteroidMass = 0.1f;          // Small mass for asteroids
    private const float OrbitSpeedMultiplier = 1.0f; 

    private readonly IAeroPhysicsWorld _world;
    private AeroParticle2D _sun;
    private readonly List<AeroParticle2D> _planets;
    private readonly List<AeroParticle2D> _asteroids;
    private readonly Random _random = new Random();

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
    private readonly (float radius, float mass, float size)[] planetConfigs = new[]
    {
        (100f, PlanetMass * 0.4f, 5f),    // Mercury
        (150f, PlanetMass * 0.8f, 8f),    // Venus
        (200f, PlanetMass * 1.0f, 8f),    // Earth
        (250f, PlanetMass * 0.5f, 6f),    // Mars
        (350f, PlanetMass * 4.0f, 20f),   // Jupiter
        (450f, PlanetMass * 3.0f, 16f),   // Saturn
        (550f, PlanetMass * 2.0f, 12f),   // Uranus
        (650f, PlanetMass * 2.0f, 12f)    // Neptune
    };

    public SolarSystemScene(Game game, Screen screen, Sprites sprites, Shapes shapes)
        : base(game, screen, sprites, shapes)
    {
        var config = AeroWorldConfiguration.Default.WithGravity(0.0f);
        _world = new AeroWorld2D(config); // No global gravity
        _planets = [];
        _asteroids = [];

        InitializeSolarSystem();
    }

    private void InitializeSolarSystem()
    {
        // Create sun with adjusted mass
        _sun = new AeroParticle2D(_screen.Width / 2, _screen.Height / 2, SunMass, 40.0f)
        {
            Damping = 1.0f // No damping for orbital mechanics
        };
        _world.AddPhysicsObject(_sun);

        // Create planets using configurations
        for (int i = 0; i < planetConfigs.Length; i++)
        {
            var (orbitRadius, mass, size) = planetConfigs[i];
            float angle = (float)(_random.NextDouble() * MathHelper.TwoPi);

            float x = _sun.Position.X + orbitRadius * (float)Math.Cos(angle);
            float y = _sun.Position.Y + orbitRadius * (float)Math.Sin(angle);

            var planet = new AeroParticle2D(x, y, mass, size);
            planet.Damping = 1.0f; // No damping for orbital mechanics

            // Calculate initial velocity for circular orbit
            // v = sqrt(GM/r) where G is grav constant, M is central mass, r is radius
            float orbitSpeed = OrbitSpeedMultiplier *
                (float)Math.Sqrt((GravitationalConstant * SunMass) / orbitRadius);

            AeroVec2 initialVelocity = new AeroVec2(
                -orbitSpeed * (float)Math.Sin(angle),
                orbitSpeed * (float)Math.Cos(angle)
            );

            planet.Velocity = initialVelocity;
            _world.AddPhysicsObject(planet);
            _planets.Add(planet);

            var gravityGen = new GravitationalForceGenerator(_sun, GravitationalConstant);
            _world.AddForceGenerator(planet, gravityGen);
        }

        // Add asteroids with adjusted mass
        AddAsteroidBelt(300, 320, 50, AsteroidMass);
    }

    private void AddAsteroidBelt(float minRadius, float maxRadius, int count, float mass)
    {
        for (int i = 0; i < count; i++)
        {
            float angle = (float)(_random.NextDouble() * MathHelper.TwoPi);
            float radius = minRadius + (float)_random.NextDouble() * (maxRadius - minRadius);

            float x = _sun.Position.X + radius * (float)Math.Cos(angle);
            float y = _sun.Position.Y + radius * (float)Math.Sin(angle);

            var asteroid = new AeroParticle2D(x, y, mass, 1 + (float)_random.NextDouble() * 2);
            asteroid.Damping = 1.0f; // No damping for orbital mechanics

            // Calculate proper orbital velocity
            float orbitSpeed = OrbitSpeedMultiplier *
                (float)Math.Sqrt((GravitationalConstant * SunMass) / radius);

            asteroid.Velocity = new AeroVec2(
                -orbitSpeed * (float)Math.Sin(angle),
                orbitSpeed * (float)Math.Cos(angle)
            );

            _world.AddPhysicsObject(asteroid);
            _asteroids.Add(asteroid);

            var gravityGen = new GravitationalForceGenerator(_sun, GravitationalConstant);
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
        foreach (var planet in _planets)
        {
            float orbitRadius = Vector2.Distance(
                new Vector2(_sun.Position.X, _sun.Position.Y),
                new Vector2(planet.Position.X, planet.Position.Y)
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
            ((AeroCircle)_sun.Shape).Radius,
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
                ((AeroCircle)_planets[i].Shape).Radius,
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
                ((AeroCircle)asteroid.Shape).Radius,
                8,
                new Color(180, 180, 180)
            );
        }

        _shapes.End();
        _screen.Unset();
        _screen.Present(_sprites);
    }
}