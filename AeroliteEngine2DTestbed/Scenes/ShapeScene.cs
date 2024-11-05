using System;
using System.Collections.Generic;
using System.Linq;
using AeroliteEngine2DTestbed.Helpers;
using AeroliteSharpEngine.AeroMath;
using AeroliteSharpEngine.Core;
using AeroliteSharpEngine.Shapes;
using Flat.Graphics;
using Flat.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace AeroliteEngine2DTestbed.Scenes;

public class ShapeScene : Scene
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
    private int colorIndex = 0;

    public ShapeScene(Game game, Screen screen, Sprites sprites, Shapes shapes)
        : base(game, screen, sprites, shapes)
    {
        world = new AeroWorld2D();
        bodies = new List<(AeroBody2D, Color)>();
        random = new Random();

        // Set initial camera zoom
        Camera.Zoom = 1;
    }

    private Color GetRainbowColor()
    {
        colorIndex = (colorIndex + 1) % rainbowColors.Length;
        return rainbowColors[colorIndex];
    } 

    private void SpawnShapeAtMouse(Vector2 screenPos, AeroVec2 velocity)
    {
        AeroBody2D body;
        Color color = GetRainbowColor();

        switch (random.Next(3))
        {
            case 0: // Circle
                float circleRadius = random.Next(15, 35);
                var circle = new AeroCircle(circleRadius);
                body = new AeroBody2D(screenPos.X, screenPos.Y, 1.0f, circle);
                break;

            case 1: // Triangle
                var triangle = new AeroTriangle(100);
                body = new AeroBody2D(screenPos.X, screenPos.Y, 1.0f, triangle);
                break;
            case 2: // Box
                var box = new AeroBox(100, 100);
                body = new AeroBody2D(screenPos.X, screenPos.Y, 1.0f, box);
                break;

            default: // Hexagon
                var hexagon = new AeroRegularPolygon(5, 30);
                body = new AeroBody2D(screenPos.X, screenPos.Y, 1.0f, hexagon);
                break;
        }

        // Set physics properties
        body.Velocity = velocity;
        body.Damping = 0.99f; 
        body.Restitution = 0.8f;  
        body.Friction = 0.2f; 

        // Add to collections
        bodies.Add((body, color));
        world.AddPhysicsObject(body);
    }

    public override void Update(GameTime gameTime)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

        // Camera controls
        if (FlatKeyboard.Instance.IsKeyDown(Keys.A)) Camera.DecZoom();
        if (FlatKeyboard.Instance.IsKeyDown(Keys.Z)) Camera.IncZoom();

        if (FlatMouse.Instance.IsLeftMouseButtonPressed())
        {
            // Get raw screen mouse position
            Vector2 mouseScreenPos = FlatMouse.Instance.GetMouseScreenPosition(Game, Screen);

            // Calculate direction and velocity based on render space coordinates
            Vector2 center = CoordinateSystem.ScreenToRender(
                new Vector2(Screen.Width / 2, Screen.Height / 2),
                Screen.Width,
                Screen.Height
            );
            Vector2 direction = Vector2.Normalize(mouseScreenPos - center);
            Vector2 velocity = direction * SpawnForce;

            // Create shape at render position
            SpawnShapeAtMouse(mouseScreenPos, new AeroVec2(velocity.X, velocity.Y));
        }

        // Screen wrapping
        foreach (var (body, _) in bodies)
        {
            if (!body.IsStatic)
            {
                if (body.Position.X < 0)
                    body.Position = new AeroVec2(Screen.Width, body.Position.Y);
                if (body.Position.X > Screen.Width)
                    body.Position = new AeroVec2(0, body.Position.Y);
                if (body.Position.Y < 0)
                    body.Position = new AeroVec2(body.Position.X, Screen.Height);
                if (body.Position.Y > Screen.Height)
                    body.Position = new AeroVec2(body.Position.X, 0);
            }
        }

        world.Update(dt);
    }

    public override void Draw(GameTime gameTime)
    {
        Screen.Set();
        Game.GraphicsDevice.Clear(new Color(10, 10, 20));
        Shapes.Begin(Camera);

        foreach (var (body, color) in bodies)
        {
            // Convert body position to render space
            Vector2 renderPos = CoordinateSystem.ScreenToRender(
                new Vector2(body.Position.X, body.Position.Y),
                Screen.Width,
                Screen.Height
            );

            switch (body.Shape)
            {
                case AeroCircle circle:
                    Shapes.DrawCircleFill(renderPos, circle.Radius, 32, color);
                    break;

                case AeroBox box:
                    Shapes.DrawBoxFill(
                        renderPos,
                        box.Width,
                        box.Height,
                        body.Angle,
                        color
                    );
                    break;

                case AeroPolygon polygon:
                    var renderVertices = polygon.WorldVertices.Select(v =>
                        CoordinateSystem.ScreenToRender(
                            new Vector2(v.X, v.Y),
                            Screen.Width,
                            Screen.Height
                        )
                    ).ToArray();

                    // Draw vertices as points for debugging
                    foreach (var vertex in renderVertices)
                    {
                        Shapes.DrawCircleFill(vertex, 5, 8, Color.Red);
                    }

                    // Draw lines between vertices
                    for (int i = 0; i < renderVertices.Length; i++)
                    {
                        int nextIndex = (i + 1) % renderVertices.Length;
                        Shapes.DrawLine(renderVertices[i], renderVertices[nextIndex], color);
                    }

                    // Draw center point
                    Shapes.DrawCircleFill(renderPos, 5, 8, Color.Yellow);

                    break;
            }
        }

        Shapes.End();
        Screen.Unset();
        Screen.Present(Sprites);
    }
}