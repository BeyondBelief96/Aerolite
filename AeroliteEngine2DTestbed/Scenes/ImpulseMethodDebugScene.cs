using System;
using System.Collections.Generic;
using AeroliteEngine2DTestbed.Helpers;
using AeroliteSharpEngine.AeroMath;
using AeroliteSharpEngine.Collisions.Detection;
using AeroliteSharpEngine.Collisions.Detection.BroadPhase;
using AeroliteSharpEngine.Collisions.Resolution.Resolvers.Impulse;
using AeroliteSharpEngine.Core;
using AeroliteSharpEngine.Core.Configuration;
using AeroliteSharpEngine.Integrators;
using AeroliteSharpEngine.Shapes;
using Flat;
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
            ("Hexagon", (x, y) => new AeroRegularPolygon(6, 30)),
        };

        // Initialize world with gravity
        var config = AeroWorldConfiguration.Default
            .WithGravity(500.0f)  // Standard gravity
            .WithPerformanceMonitoring(true)
            .WithIntegrator(new RK4Integrator())
            .WithCollisionSystemConfiguration(CollisionSystemConfiguration.Default()
                .WithBroadPhase(new DynamicQuadTree(screen.Width, screen.Height, new AeroVec2(0, 0), BoundingAreaType.AABB))
                .WithCollisionResolver(new ImpulseMethodCollisionResolver()));
        
        world = new AeroWorld2D(config);
        
        // Add large static object body in middle.
        var staticObject = new AeroBody2D(Screen.Width / 2.0f, Screen.Height / 2.0f, 0.0f, new AeroBox(300.0f, 300.0f), 0.5f, 0.5f);
        world.AddPhysicsObject(staticObject);
        
        // Create thinner boundaries
        const float wallThickness = 20.0f; // Reduced thickness for all boundaries
        
        // Floor
        var floor = new AeroBody2D(
            Screen.Width / 2.0f, 
            Screen.Height - 100.0f, 
            0.0f, 
            new AeroBox(Screen.Width - 100.0f, wallThickness), 
            0.5f, 
            0.5f);
        
        // Left wall
        var leftWall = new AeroBody2D(
            wallThickness / 2, 
            Screen.Height / 2.0f, 
            0.0f, 
            new AeroBox(wallThickness, Screen.Height), 
            1.0f, 
            0.5f);
        
        // Right wall
        var rightWall = new AeroBody2D(
            Screen.Width - wallThickness / 2, 
            Screen.Height / 2.0f, 
            0.0f, 
            new AeroBox(wallThickness, Screen.Height), 
            1.0f, 
            0.5f);

        world.AddPhysicsObject(floor);
        world.AddPhysicsObject(leftWall);
        world.AddPhysicsObject(rightWall);
        
        Camera.Zoom = 1;
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
            Vector2 mousePos = FlatMouse.Instance.GetMouseScreenPosition(Game, Screen);
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
        var (type, creator) = shapeCreators[currentShapeIndex];
        
        var body = new AeroBody2D(x, y, 1.0f, creator(x, y), 0.5f, 0.5f)
        {
            Velocity = new AeroVec2(RandomHelper.RandomSingle(5, 800), 0.0f)
        };
        world.AddPhysicsObject(body);
        
        
    }

    private void ResetScene()
    {
        world.ClearWorld(); 
    }

    protected override void DrawScene(GameTime gameTime)
    {
        foreach (var physicsObject in world.GetObjects())
        {
            Color color = physicsObject.IsStatic ? staticColor : dynamicColor;
            AeroDrawingHelpers.DrawPhysicsObject2D(physicsObject, color, Shapes, Screen);
        }

        // Draw collision contact points if in debug mode
        foreach (var manifold in world.CollisionSystem.Collisions)
        {
            AeroDrawingHelpers.DrawCollisionInfo(manifold, Screen, Shapes);
        }
    }
}