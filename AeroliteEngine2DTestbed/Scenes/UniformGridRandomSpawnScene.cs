using System;
using System.Collections.Generic;
using System.Linq;
using AeroliteEngine2DTestbed.Helpers;
using AeroliteSharpEngine.AeroMath;
using AeroliteSharpEngine.Collisions.Detection.BoundingAreas;
using AeroliteSharpEngine.Collisions.Detection.BroadPhase;
using AeroliteSharpEngine.Core;
using AeroliteSharpEngine.Core.Interfaces;
using AeroliteSharpEngine.Shapes;
using Flat.Graphics;
using Flat.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace AeroliteEngine2DTestbed.Scenes;

public class UniformGridRandomSpawnScene : Scene
{
    private readonly Color[] rainbowColors = new[]
    {
        Color.Red,
        Color.Orange, 
        Color.Yellow,
        Color.Green,
        Color.Blue,
        Color.Indigo,
        Color.Violet
    };

    private readonly AeroWorld2D world;
    private readonly List<(AeroBody2D body, Color color)> bodies;
    private readonly Random random;
    private const float SpawnForce = 500f;
    private const float CellSize = 100f; // Size of grid cells
    private float spawnTimer = 0f;
    private const float SpawnInterval = 0.1f; // Spawn every 0.1 seconds
    private bool showGrid = true;
    private List<(IPhysicsObject2D, IPhysicsObject2D)> potentialCollisions;

    public UniformGridRandomSpawnScene(Game game, Screen screen, Sprites sprites, Shapes shapes)
        : base(game, screen, sprites, shapes)
    {
        world = new AeroWorld2D(AeroWorldConfiguration.Default.WithPerformanceMonitoring(true));
        bodies = new List<(AeroBody2D, Color)>();
        random = new Random();
        potentialCollisions = [];

        // Set initial camera zoom
        Camera.Zoom = 1;
    }

    private Color GetRandomColor()
    {
        return rainbowColors[random.Next(rainbowColors.Length)];
    }

    private void SpawnRandomShape()
    {
        // Random position within screen bounds
        float x = random.Next(Screen.Width);
        float y = random.Next(Screen.Height);
        
        // Random velocity
        float vx = random.Next(-200, 200);
        float vy = random.Next(-200, 200);
        AeroVec2 velocity = new AeroVec2(vx, vy);

        AeroBody2D body;
        Color color = GetRandomColor();

        switch (random.Next(3))
        {
            case 0: // Circle
                float radius = random.Next(10, 25);
                var circle = new AeroCircle(radius);
                body = new AeroBody2D(x, y, 1.0f, circle);
                break;

            case 1: // Box
                var box = new AeroBox(
                    random.Next(20, 40), 
                    random.Next(20, 40)
                );
                body = new AeroBody2D(x, y, 1.0f, box);
                break;

            default: // Regular polygon
                var polygon = new AeroRegularPolygon(
                    random.Next(3, 7), // 3 to 6 sides
                    random.Next(15, 30) // radius
                );
                body = new AeroBody2D(x, y, 1.0f, polygon);
                break;
        }

        body.Velocity = velocity;
        body.Damping = 0.99f;
        body.Restitution = 0.8f;
        body.Friction = 0.2f;

        bodies.Add((body, color));
        world.AddPhysicsObject(body);
    }

    public override void Update(GameTime gameTime)
    {
        var dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

        // Toggle grid display with G key
        if (FlatKeyboard.Instance.IsKeyClicked(Keys.G))
        {
            showGrid = !showGrid;
        }

        // Clear all with C key
        if (FlatKeyboard.Instance.IsKeyClicked(Keys.C))
        {
            bodies.Clear();
            world.ClearWorld();
        }

        // Auto spawn shapes
        spawnTimer += dt;
        if (spawnTimer >= SpawnInterval && bodies.Count < 100) // Limit to 100 bodies
        {
            SpawnRandomShape();
            spawnTimer = 0;
        }

        // Manual spawn with left click
        if (FlatMouse.Instance.IsLeftMouseButtonPressed())
        {
            for (int i = 0; i < 5; i++) // Spawn 5 at once
            {
                SpawnRandomShape();
            }
        }

        // Get potential collisions
        if (world.CollisionSystem.Configuration.BroadPhase is UniformGrid grid)
        {
            potentialCollisions = grid.FindPotentialCollisions().ToList();
        }

        // Screen wrapping
        foreach (var (body, _) in bodies)
        {
            if (body.IsStatic) continue;
            if (body.Position.X < 0)
                body.Position = new AeroVec2(Screen.Width, body.Position.Y);
            if (body.Position.X > Screen.Width)
                body.Position = new AeroVec2(0, body.Position.Y);
            if (body.Position.Y < 0)
                body.Position = new AeroVec2(body.Position.X, Screen.Height);
            if (body.Position.Y > Screen.Height)
                body.Position = new AeroVec2(body.Position.X, 0);
        }

        world.Update(dt);
    }

    public override void Draw(GameTime gameTime)
    {
        Screen.Set();
        Game.GraphicsDevice.Clear(new Color(10, 10, 20));
        Shapes.Begin(Camera);

        // Draw grid
        if (showGrid)
        {
            AeroDrawingHelpers.DrawGrid(Screen, Shapes, CellSize);
        }

        // Draw all bodies
        foreach (var (body, color) in bodies)
        {
            // Check if this body is in any potential collision pairs
            bool inCollision = potentialCollisions.Any(pair => 
                pair.Item1.Equals(body) || pair.Item2.Equals(body));
            
            // Use brighter color for bodies in potential collisions
            Color drawColor = inCollision ? Color.Lerp(color, Color.White, 1.0f) : color;

            AeroDrawingHelpers.DrawBody(body, drawColor, Shapes, Screen);
            var boundingArea = AABB2D.CreateFromShape(body.Shape, body.Position);
            AeroDrawingHelpers.DrawBoundingArea(boundingArea, Color.Yellow * 0.5f, Screen, Shapes);
        }

        Shapes.End();
        Screen.Unset();
        Screen.Present(Sprites);
    }
}