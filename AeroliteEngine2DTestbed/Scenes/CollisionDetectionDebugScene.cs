#nullable enable
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
    private const float CellSize = 100f;
    private bool showGrid = true;
    private bool showCollisionInfo = true;
    private readonly Random random;
    private int currentMouseShapeIndex = 0;
    private readonly List<(string name, Func<float, float, AeroShape2D> creator)> shapeCreators;

    public CollisionDetectionDebugScene(Game game, Screen screen, Sprites sprites, Shapes shapes)
        : base(game, screen, sprites, shapes)
    {
        var config = AeroWorldConfiguration.Default.WithPerformanceMonitoring(true);
        world = new AeroWorld2D(config);
        staticBodies = new List<(AeroBody2D, Color)>();
        random = new Random();
        
        shapeCreators = new List<(string, Func<float, float, AeroShape2D>)>
        {
            ("Circle", (x, y) => new AeroCircle(25)),
            ("Box", (x, y) => new AeroBox(50, 50)),
            ("Triangle", (x, y) => new AeroRegularPolygon(3, 30)),
            ("Pentagon", (x, y) => new AeroRegularPolygon(5, 30)),
            ("Hexagon", (x, y) => new AeroRegularPolygon(6, 30))
        };

        PlaceStaticBodiesInGrid();
        CreateMouseBody();
        
        Camera.Zoom = 1;
    }

    private void CreateMouseBody()
    {
        // Create initial mouse body with first shape type
        mouseBody = new AeroBody2D(0, 0, 0.0f, new AeroBox(50, 50));
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
        // Calculate grid dimensions
        int cols = (int)(Screen.Width / CellSize);
        int rows = (int)(Screen.Height / CellSize);

        // Randomly place bodies in some cells (skip others to leave them empty)
        for (int col = 0; col < cols; col++)
        {
            for (int row = 0; row < rows; row++)
            {
                // 40% chance to place a body in each cell
                if (random.NextDouble() > 0.4) continue;

                // Calculate cell center
                float x = (col + 0.5f) * CellSize;
                float y = (row + 0.5f) * CellSize;

                // Randomly choose shape type
                AeroBody2D body = random.Next(5) switch
                {
                    0 => new AeroBody2D(x, y, 0.0f, shapeCreators[0].creator(col, row)),
                    1 => new AeroBody2D(x, y, 0.0f, shapeCreators[1].creator(col, row)),
                    2 => new AeroBody2D(x, y, 0.0f, shapeCreators[2].creator(col, row)),
                    3 => new AeroBody2D(x, y, 0.0f, shapeCreators[3].creator(col, row)),
                    _ => new AeroBody2D(x, y, 0.0f, shapeCreators[4].creator(col, row)),
                    
                };
                
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
            Vector2 mousePos = FlatMouse.Instance.GetMouseScreenPosition(Game, Screen);
            mouseBody.Position = new AeroVec2(mousePos.X, mousePos.Y);
            mouseBody.Angle = 30.0f;
        }

        world.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
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

        // Draw static bodies
        foreach (var (body, color) in staticBodies)
        {
            bool inCollision = world.CollisionSystem.Collisions.Any(m => 
                Equals(m.ObjectA, body) || Equals(m.ObjectB, body) && m.HasCollision);
            
            Color drawColor = inCollision ? Color.Lerp(color, Color.White, 0.5f) : color;
            AeroDrawingHelpers.DrawBody(body, drawColor, Shapes, Screen);
        }

        // Draw mouse body
        if (mouseBody != null)
        {
            bool inCollision = world.CollisionSystem.Collisions.Any(m => 
                Equals(m.ObjectA, mouseBody) || Equals(m.ObjectB, mouseBody) && m.HasCollision);
            
            AeroDrawingHelpers.DrawBody(mouseBody, inCollision ? Color.White : Color.Orange, Shapes, Screen);
        }

        // Draw collision information
        if (showCollisionInfo)
        {
            foreach (var manifold in world.CollisionSystem.Collisions)
            {
                AeroDrawingHelpers.DrawCollisionInfo(manifold, Screen, Shapes);
            }
        }

        Shapes.End();
        Screen.Unset();
        Screen.Present(Sprites);
    }
}