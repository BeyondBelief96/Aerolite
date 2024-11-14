using System;
using System.Collections.Generic;
using System.Linq;
using AeroliteEngine2DTestbed.Helpers;
using AeroliteSharpEngine.AeroMath;
using AeroliteSharpEngine.Collisions.Detection;
using AeroliteSharpEngine.Collisions.Detection.BroadPhase;
using AeroliteSharpEngine.Collisions.Resolution.Resolvers.Impulse;
using AeroliteSharpEngine.Core;
using AeroliteSharpEngine.Core.Interfaces;
using AeroliteSharpEngine.Integrators;
using AeroliteSharpEngine.Shapes;
using Flat;
using Flat.Graphics;
using Flat.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace AeroliteEngine2DTestbed.Scenes;

public class QuadTreeDebugScene : Scene
{
    private readonly AeroWorld2D world;
    private readonly DynamicQuadTree quadTree;
    private bool isParticleScene = false;
    private float particleSpawnTimer = 0f;

    // Colors for visualization
    private readonly Color nodeColor = new(40, 40, 40, 50);
    private readonly Color leafNodeColor = new(20, 100, 20, 60);
    private readonly Color particleColor = new(255, 200, 100, 180);
    private readonly Color bodyColor = new(100, 150, 255);
    private const int MAX_PARTICLES = 10_000;

    private readonly List<(string name, Func<float, float, AeroShape2D> creator)> shapeCreators;
    private int currentShapeIndex = 0;

    // Keep track of static boundaries
    private readonly List<IPhysicsObject2D> bodySceneBoundaries = new();

    public QuadTreeDebugScene(Game game, Screen screen, Sprites sprites, Shapes shapes)
        : base(game, screen, sprites, shapes)
    {
        shapeCreators = new List<(string, Func<float, float, AeroShape2D>)>
        {
            ("Box", (x, y) => new AeroBox(40, 40)),
            ("Triangle", (x, y) => new AeroRegularPolygon(3, 30)),
            ("Hexagon", (x, y) => new AeroRegularPolygon(6, 30)),
        };

        quadTree = new DynamicQuadTree(
            screen.Width, 
            screen.Height,
            new AeroVec2(0, 0),
            BoundingAreaType.AABB,
            maxObjectsPerNode: 3,
            maxDepth: 6
        );
        
        var uniformGrid = new UniformGrid(BoundingAreaType.AABB);

        var config = AeroWorldConfiguration.Default
            .WithPerformanceMonitoring(true)
            .WithIntegrator(new RK4Integrator())
            .WithCollisionSystemConfiguration(CollisionSystemConfiguration.Default()
                .WithBroadPhase(quadTree)
                .WithCollisionResolver(new ImpulseMethodCollisionResolver()));
        
        world = new AeroWorld2D(config);
        CreateBoundaries();
        SetupBodyScene(); // Start with body scene
        Camera.Zoom = 1;
    }

    private void CreateBoundaries()
    {
        const float wallThickness = 20.0f;
        
        // Floor - raised and narrower
        var floor = new AeroBody2D(
            Screen.Width / 2.0f, 
            Screen.Height - 100,
            0.0f, 
            new AeroBox(Screen.Width - 100, wallThickness));
        
        // Left wall
        var leftWall = new AeroBody2D(
            wallThickness / 2, 
            Screen.Height / 2.0f, 
            0.0f, 
            new AeroBox(wallThickness, Screen.Height));
        
        // Right wall
        var rightWall = new AeroBody2D(
            Screen.Width - wallThickness / 2, 
            Screen.Height / 2.0f, 
            0.0f, 
            new AeroBox(wallThickness, Screen.Height));

        bodySceneBoundaries.Add(floor);
        bodySceneBoundaries.Add(leftWall);
        bodySceneBoundaries.Add(rightWall);
    }

    private void SetupBodyScene()
    {
        world.ClearWorld();
        world.GetConfiguration()
            .WithGravity(500f);
            
        // Add boundaries
        foreach (var boundary in bodySceneBoundaries)
        {
            world.AddPhysicsObject(boundary);
        }
    }

    private void SetupParticleScene()
    {
        world.ClearWorld();
        world.GetConfiguration()
            .WithGravity(0f);
            
        // No boundaries needed for particle scene
    }

    public override void Update(GameTime gameTime)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

        // Toggle between scenes
        if (FlatKeyboard.Instance.IsKeyClicked(Keys.Space))
        {
            isParticleScene = !isParticleScene;
            if (isParticleScene)
                SetupParticleScene();
            else
                SetupBodyScene();
        }

        // Cycle through shapes in body scene
        if (!isParticleScene && FlatKeyboard.Instance.IsKeyClicked(Keys.Tab))
        {
            currentShapeIndex = (currentShapeIndex + 1) % shapeCreators.Count;
        }

        // Handle mouse click for spawning bodies or particles
        if (FlatMouse.Instance.IsLeftMouseButtonPressed())
        {
            Vector2 mousePos = FlatMouse.Instance.GetMouseScreenPosition(Game, Screen);
            if (isParticleScene)
            {
                SpawnParticle(mousePos.X, mousePos.Y);
            }
            else
            {
                SpawnBody(mousePos.X, mousePos.Y);
            }
        }

        // Automatic particle spawning in particle scene
        if (isParticleScene)
        {
            particleSpawnTimer += dt;
            if (particleSpawnTimer >= 0.001f)
            {
                SpawnParticle(Screen.Width / 2.0f + RandomHelper.RandomSingle(-50, 50), Screen.Height - 150);
                particleSpawnTimer = 0f;
            }
        }

        // Reset scene if R is pressed
        if (FlatKeyboard.Instance.IsKeyClicked(Keys.R))
        {
            if (isParticleScene)
                SetupParticleScene();
            else
                SetupBodyScene();
        }

        world.Update(dt);

        if (isParticleScene)
        {
            AeroDrawingHelpers.KeepParticleInScreenBounds(world.GetDynamicParticles(), Screen);
        }
    }

    private void SpawnParticle(float x, float y)
    {
        if (world.GetDynamicObjects().Count() >= MAX_PARTICLES) return;

        var particle = new AeroParticle2D(
            x, y, 0.1f,
            restitution: 0.7f,
            friction: 0.1f,
            radius: RandomHelper.RandomSingle(3, 8));

        particle.Velocity = new AeroVec2(
            RandomHelper.RandomSingle(-300, 300),
            RandomHelper.RandomSingle(-600, -400)
        );

        world.AddPhysicsObject(particle);
    }

    private void SpawnBody(float x, float y)
    {
        var (_, creator) = shapeCreators[currentShapeIndex];
        
        var body = new AeroBody2D(x, y, 1.0f, creator(x, y), 0.5f, 0.5f)
        {
            AngularVelocity = RandomHelper.RandomSingle(-1f, 1f)
        };

        world.AddPhysicsObject(body);
    }

    protected override void DrawScene(GameTime gameTime)
    {
        // Draw quadtree using helper
        AeroDrawingHelpers.DrawQuadTree(quadTree, nodeColor, leafNodeColor, Screen, Shapes);

        // Draw physics objects
        foreach (var obj in world.GetObjects())
        {
            if (obj.IsStatic)
            {
                AeroDrawingHelpers.DrawPhysicsObject2D(obj, Color.DarkGray, Shapes, Screen);
            }
            else if (obj is AeroParticle2D)
            {
                AeroDrawingHelpers.DrawPhysicsObject2D(obj, particleColor, Shapes, Screen);
            }
            else
            {
                AeroDrawingHelpers.DrawPhysicsObject2D(obj, bodyColor, Shapes, Screen);
            }
        }
    }
}