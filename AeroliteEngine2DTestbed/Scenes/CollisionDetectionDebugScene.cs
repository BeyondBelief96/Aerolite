#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using AeroliteEngine2DTestbed.Helpers;
using AeroliteSharpEngine.AeroMath;
using AeroliteSharpEngine.Collisions.Detection.CollisionPrimitives;
using AeroliteSharpEngine.Core;
using AeroliteSharpEngine.Shapes;
using Flat.Graphics;
using Flat.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace AeroliteEngine2DTestbed.Scenes;

public class CollisionDetectionDebugScene : Scene
{
    private readonly Color[] shapeColors = new[]
    {
        Color.Red,
        Color.Green,
        Color.Blue,
        Color.Yellow,
        Color.Purple
    };

    private readonly AeroWorld2D world;
    private readonly List<(AeroBody2D body, Color color)> staticBodies;
    private AeroBody2D? mouseBody;
    private readonly float cellSize = 100f;
    private bool showGrid = true;
    private bool showCollisionInfo = true;
    private readonly Random random;
    private IEnumerable<CollisionManifold> collisionManifolds;
    private int currentMouseShapeIndex = 0;
    private readonly List<(string name, Func<float, float, AeroShape2D> creator)> shapeCreators;

    public CollisionDetectionDebugScene(Game game, Screen screen, Sprites sprites, Shapes shapes)
        : base(game, screen, sprites, shapes)
    {
        // Create shape creators
        shapeCreators = new List<(string, Func<float, float, AeroShape2D>)>
        {
            ("Circle", (x, y) => new AeroCircle(25)),
            ("Box", (x, y) => new AeroBox(50, 50)),
            ("Triangle", (x, y) => new AeroRegularPolygon(3, 30)),
            ("Pentagon", (x, y) => new AeroRegularPolygon(5, 30)),
            ("Hexagon", (x, y) => new AeroRegularPolygon(6, 30))
        };

        var config = AeroWorldConfiguration.Default.WithPerformanceMonitoring(false);
        world = new AeroWorld2D(config);
        staticBodies = new List<(AeroBody2D, Color)>();
        collisionManifolds = [];
        random = new Random();

        PlaceStaticBodiesInGrid();
        CreateMouseBody();
        
        _camera.Zoom = 1;
    }

    private void CreateMouseBody()
    {
        // Create initial mouse body with first shape type
        var (_, creator) = shapeCreators[currentMouseShapeIndex];
        mouseBody = new AeroBody2D(0, 0, 1.0f, creator(0, 0));
        world.AddPhysicsObject(mouseBody);
    }

    private void CycleMouseShape()
    {
        if (mouseBody == null) return;
        
        // Remove current mouse body
        world.RemovePhysicsObject(mouseBody);

        // Cycle to next shape
        currentMouseShapeIndex = (currentMouseShapeIndex + 1) % shapeCreators.Count;
        var (_, creator) = shapeCreators[currentMouseShapeIndex];

        // Create new mouse body with next shape
        mouseBody = new AeroBody2D(
            mouseBody.Position.X,
            mouseBody.Position.Y,
            1.0f,
            creator(0, 0)
        );
        world.AddPhysicsObject(mouseBody);
    }

    private void PlaceStaticBodiesInGrid()
    {
        int cols = (int)(_screen.Width / cellSize);
        int rows = (int)(_screen.Height / cellSize);

        for (int col = 0; col < cols; col++)
        {
            for (int row = 0; row < rows; row++)
            {
                if (random.NextDouble() > 0.3) continue;

                float x = (col + 0.5f) * cellSize;
                float y = (row + 0.5f) * cellSize;

                // Get random shape creator
                var (_, creator) = shapeCreators[random.Next(shapeCreators.Count)];
                var body = new AeroBody2D(x, y, 0.0f, creator(x, y));

                Color color = shapeColors[random.Next(shapeColors.Length)];
                staticBodies.Add((body, color));
                world.AddPhysicsObject(body);
            }
        }
    }

    public override void Update(GameTime gameTime)
    {
        if (FlatKeyboard.Instance.IsKeyClicked(Keys.G))
            showGrid = !showGrid;

        if (FlatKeyboard.Instance.IsKeyClicked(Keys.C))
            showCollisionInfo = !showCollisionInfo;

        if (FlatKeyboard.Instance.IsKeyClicked(Keys.Space))
            CycleMouseShape();

        if (FlatKeyboard.Instance.IsKeyClicked(Keys.R))
        {
            world.ClearWorld();
            staticBodies.Clear();
            PlaceStaticBodiesInGrid();
            CreateMouseBody();
        }

        // Update mouse body position
        if (mouseBody != null)
        {
            Vector2 mousePos = FlatMouse.Instance.GetMouseScreenPosition(_game, _screen);
            mouseBody.Angle += (float)gameTime.ElapsedGameTime.TotalSeconds;
            mouseBody.Position = new AeroVec2(mousePos.X, mousePos.Y);

            // Get collisions
            collisionManifolds = world.CollisionSystem.Collisions;
        }

        world.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
    }
    
    public override void Draw(GameTime gameTime)
    {
        _screen.Set();
        _game.GraphicsDevice.Clear(new Color(10, 10, 20));
        _shapes.Begin(_camera);

        // Draw grid
        if (showGrid)
        {
            AeroDrawingHelpers.DrawGrid(_screen, _shapes, cellSize);
        }

        // Draw static bodies
        foreach (var (body, color) in staticBodies)
        {
            bool inCollision = collisionManifolds.Any(m => 
                Equals(m.ObjectA, body) || Equals(m.ObjectB, body) && m.HasCollision);
            
            Color drawColor = inCollision ? Color.Lerp(color, Color.White, 0.5f) : color;
            AeroDrawingHelpers.DrawBody(body, drawColor, _shapes, _screen);
        }

        // Draw mouse body
        if (mouseBody != null)
        {
            bool inCollision = collisionManifolds.Any(m => 
                Equals(m.ObjectA, mouseBody) || Equals(m.ObjectB, mouseBody) && m.HasCollision);
            
            AeroDrawingHelpers.DrawBody(mouseBody, inCollision ? Color.White : Color.Orange, _shapes, _screen);
        }

        // Draw collision information
        if (showCollisionInfo)
        {
            foreach (var manifold in collisionManifolds)
            {
                AeroDrawingHelpers.DrawCollisionInfo(manifold, _screen, _shapes);
            }
        }

        _shapes.End();
        _screen.Unset();
        _screen.Present(_sprites);
    }
}