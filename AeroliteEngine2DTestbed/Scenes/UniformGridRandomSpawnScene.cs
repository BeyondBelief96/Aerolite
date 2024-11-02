using System;
using System.Collections.Generic;
using System.Linq;
using AeroliteEngine2DTestbed.Helpers;
using AeroliteSharpEngine.AeroMath;
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
        world = new AeroWorld2D();
        bodies = new List<(AeroBody2D, Color)>();
        random = new Random();
        potentialCollisions = [];

        // Set initial camera zoom
        _camera.Zoom = 1;
    }

    private Color GetRandomColor()
    {
        return rainbowColors[random.Next(rainbowColors.Length)];
    }

    private void SpawnRandomShape()
    {
        // Random position within screen bounds
        float x = random.Next(_screen.Width);
        float y = random.Next(_screen.Height);
        
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
                body.Position = new AeroVec2(_screen.Width, body.Position.Y);
            if (body.Position.X > _screen.Width)
                body.Position = new AeroVec2(0, body.Position.Y);
            if (body.Position.Y < 0)
                body.Position = new AeroVec2(body.Position.X, _screen.Height);
            if (body.Position.Y > _screen.Height)
                body.Position = new AeroVec2(body.Position.X, 0);
        }

        world.Update(dt);
    }

    public override void Draw(GameTime gameTime)
    {
        _screen.Set();
        _game.GraphicsDevice.Clear(new Color(10, 10, 20));
        _shapes.Begin(_camera);

        // Draw grid
        if (showGrid)
        {
            // Draw vertical grid lines
            for (float x = 0; x < _screen.Width; x += CellSize)
            {
                var gridLineRenderXPosStart = CoordinateSystem.ScreenToRender(new Vector2(x, 0), _screen.Width, _screen.Height);
                var gridLineRenderXPosEnd = CoordinateSystem.ScreenToRender(new Vector2(x, _screen.Height), _screen.Width, _screen.Height);
                _shapes.DrawLine(
                    gridLineRenderXPosStart,
                    gridLineRenderXPosEnd,
                    new Color(50, 50, 50)
                );
            }

            // Draw horizontal grid lines
            for (float y = 0; y < _screen.Height; y += CellSize)
            {
                var gridLineRenderYPosStart = CoordinateSystem.ScreenToRender(new Vector2(0, y), _screen.Width, _screen.Height);
                var gridLineRenderYPosEnd = CoordinateSystem.ScreenToRender(new Vector2(_screen.Width, y), _screen.Width, _screen.Height);
                _shapes.DrawLine(
                    gridLineRenderYPosStart,
                    gridLineRenderYPosEnd,
                    new Color(50, 50, 50)
                );
            }
        }

        // Draw all bodies
        foreach (var (body, color) in bodies)
        {
            // Check if this body is in any potential collision pairs
            bool inCollision = potentialCollisions.Any(pair => 
                pair.Item1 == body || pair.Item2 == body);
            
            // Use brighter color for bodies in potential collisions
            Color drawColor = inCollision ? Color.Lerp(color, Color.White, 1.0f) : color;

            Vector2 renderPos = CoordinateSystem.ScreenToRender( new Vector2(body.Position.X, body.Position.Y), _screen.Width, _screen.Height);

            switch (body.Shape)
            {
                case AeroCircle circle:
                    _shapes.DrawCircleFill(renderPos, circle.Radius, 32, drawColor);
                    break;

                case AeroBox box:
                    _shapes.DrawBoxFill(renderPos, box.Width, box.Height, body.Angle, drawColor);
                    break;

                case AeroPolygon polygon:
                    var vertices = polygon.WorldVertices
                        .Select(v => new Vector2(v.X, v.Y))
                        .ToArray();

                    // Draw filled polygon
                    for (var i = 0; i < vertices.Length - 2; i++)
                    {
                        _shapes.DrawTriangleFill(
                            vertices[0],
                            vertices[i + 1],
                            vertices[i + 2],
                            drawColor
                        );
                    }
                    break;
            }
        }

        // Draw debug info
        _shapes.End();
        _screen.Unset();
        _screen.Present(_sprites);
    }
}