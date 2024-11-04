using System;
using System.Collections.Generic;
using AeroliteEngine2DTestbed.Helpers;
using AeroliteSharpEngine.AeroMath;
using AeroliteSharpEngine.Collisions.Detection;
using AeroliteSharpEngine.Collisions.Resolution.Resolvers;
using AeroliteSharpEngine.Core;
using AeroliteSharpEngine.Shapes;
using Flat.Graphics;
using Flat.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace AeroliteEngine2DTestbed.Scenes;

public class ImpulseMethodDebugScene : Scene
{
    private readonly AeroWorld2D world;
    private readonly List<(string name, Func<float, float, AeroShape2D> creator)> shapeCreators;
    private int currentShapeIndex = 0;
    private readonly Color staticColor = new Color(150, 150, 150);
    private readonly Color dynamicColor = new Color(255, 150, 100);

    public ImpulseMethodDebugScene(Game game, Screen screen, Sprites sprites, Shapes shapes)
        : base(game, screen, sprites, shapes)
    {
        // Create shape creators
        shapeCreators = new List<(string, Func<float, float, AeroShape2D>)>
        {
            ("Circle", (x, y) => new AeroCircle(25)),
            ("Box", (x, y) => new AeroBox(40, 40)),
            ("Triangle", (x, y) => new AeroRegularPolygon(3, 30)),
            ("Pentagon", (x, y) => new AeroRegularPolygon(5, 30)),
            ("Hexagon", (x, y) => new AeroRegularPolygon(6, 30))
        };

        // Initialize world with gravity
        var config = AeroWorldConfiguration.Default
            .WithGravity(50)  // Standard gravity
            .WithPerformanceMonitoring(true)
            .WithCollisionSystemConfiguration(CollisionSystemConfiguration.Default()
                .WithCollisionResolver(new ImpulseMethodCollisionResolver()));
        
        world = new AeroWorld2D(config);

        // Create static boundaries
        CreateBoundaries(screen);
        
        _camera.Zoom = 1;
    }

    private void CreateBoundaries(Screen screen)
    {
        // Floor
        var floor = new AeroBody2D(
            screen.Width / 2,  // Center X
            screen.Height - 50, // Near bottom
            0.0f,  // Static mass
            new AeroBox(screen.Width - 100, 50) // Full width, 50 height
        );
        floor.Restitution = 0.3f;  // Some bounce
        floor.Friction = 0.5f;     // Some friction
        world.AddPhysicsObject(floor);

        // Left wall
        var leftWall = new AeroBody2D(
            25,  // Left edge + half width
            screen.Height / 2,
            0.0f,  // Static mass
            new AeroBox(50, screen.Height - 100)  // Tall box
        );
        leftWall.Restitution = 0.3f;
        leftWall.Friction = 0.5f;
        world.AddPhysicsObject(leftWall);

        // Right wall
        var rightWall = new AeroBody2D(
            screen.Width - 25,  // Right edge - half width
            screen.Height / 2,
            0.0f,  // Static mass
            new AeroBox(50, screen.Height - 100)  // Tall box
        );
        rightWall.Restitution = 0.3f;
        rightWall.Friction = 0.5f;
        world.AddPhysicsObject(rightWall);
    }

    public override void Update(GameTime gameTime)
    {
        // Handle shape cycling with space key
        if (FlatKeyboard.Instance.IsKeyClicked(Keys.Space))
        {
            currentShapeIndex = (currentShapeIndex + 1) % shapeCreators.Count;
        }

        // Handle mouse click for spawning shapes
        if (FlatMouse.Instance.IsLeftMouseButtonPressed())
        {
            Vector2 mousePos = FlatMouse.Instance.GetMouseScreenPosition(_game, _screen);
            SpawnShapeAtPosition(mousePos.X, mousePos.Y);
        }

        // Reset scene if R is pressed
        if (FlatKeyboard.Instance.IsKeyClicked(Keys.R))
        {
            ResetScene();
        }

        world.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
    }

    private void SpawnShapeAtPosition(float x, float y)
    {
        var (_, creator) = shapeCreators[currentShapeIndex];
        var body = new AeroBody2D(x, y, 1.0f, creator(x, y))
        {
            // Set some default physical properties
            Restitution = 0.3f, // Some bounce
            Friction = 0.5f, // Some friction
        };

        world.AddPhysicsObject(body);
    }

    private void ResetScene()
    {
        world.ClearWorld(); 
    }

    public override void Draw(GameTime gameTime)
    {
        _screen.Set();
        _game.GraphicsDevice.Clear(new Color(10, 10, 20));
        _shapes.Begin(_camera);

        // Draw static boundaries
        foreach (var body in world.GetObjects())
        {
            if (body is not AeroBody2D aeroBody) continue;
            Color color = aeroBody.IsStatic ? staticColor : dynamicColor;
            AeroDrawingHelpers.DrawBody(aeroBody, color, _shapes, _screen);
        }

        // Draw collision contact points if in debug mode
        foreach (var manifold in world.CollisionSystem.Collisions)
        {
            AeroDrawingHelpers.DrawCollisionInfo(manifold, _screen, _shapes);
        }
        
        _shapes.End();
        _screen.Unset();
        _screen.Present(_sprites);
    }

}