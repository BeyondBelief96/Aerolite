#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using AeroliteEngine2DTestbed.Helpers;
using AeroliteSharpEngine.AeroMath;
using AeroliteSharpEngine.Collisions.Detection;
using AeroliteSharpEngine.Collisions.Detection.BoundingAreas;
using AeroliteSharpEngine.Collisions.Detection.BroadPhase;
using AeroliteSharpEngine.Collisions.Detection.Interfaces;
using AeroliteSharpEngine.Core;
using AeroliteSharpEngine.Core.Interfaces;
using AeroliteSharpEngine.Shapes;
using Flat.Graphics;
using Flat.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace AeroliteEngine2DTestbed.Scenes;

public class UniformGridDebugScene : Scene
{
    private readonly Color[] cellColors = new[]
    {
        Color.Red,
        Color.Green,
        Color.Blue,
        Color.Yellow,
        Color.Purple
    };

    private readonly AeroWorld2D world;
    private readonly List<(AeroBody2D body, Color color)> staticBodies;
    private AeroBody2D? mouseBody; // Body that follows mouse
    private IBoundingArea? mouseBoundingArea; // Add this field to cache the bounding area
    private readonly float cellSize = 100f;
    private bool showGrid = true;
    private List<(IPhysicsObject2D, IPhysicsObject2D)> potentialCollisions;
    private readonly Random random;

    public UniformGridDebugScene(Game game, Screen screen, Sprites sprites, Shapes shapes)
        : base(game, screen, sprites, shapes)
    {
        // Create world with no gravity and uniform grid
        var config = AeroWorldConfiguration.Default;
        
        world = new AeroWorld2D(config);
        staticBodies = new List<(AeroBody2D, Color)>();
        potentialCollisions = [];
        random = new Random();

        // Setup initial static bodies
        PlaceStaticBodiesInGrid();
        CreateMouseBody();
        
        _camera.Zoom = 1;
    }

    private void CreateMouseBody()
    {
        // Create a body that will follow the mouse
        mouseBody = new AeroBody2D(0, 0, 1.0f, new AeroRegularPolygon(5, 50))
        {
            IsStatic = false // So it can move
        };
        world.AddPhysicsObject(mouseBody);
    }

    private void PlaceStaticBodiesInGrid()
    {
        // Calculate grid dimensions
        int cols = (int)(_screen.Width / cellSize);
        int rows = (int)(_screen.Height / cellSize);

        // Randomly place bodies in some cells (skip others to leave them empty)
        for (int col = 0; col < cols; col++)
        {
            for (int row = 0; row < rows; row++)
            {
                // 40% chance to place a body in each cell
                if (random.NextDouble() > 0.4) continue;

                // Calculate cell center
                float x = (col + 0.5f) * cellSize;
                float y = (row + 0.5f) * cellSize;

                // Randomly choose shape type
                AeroBody2D body = random.Next(3) switch
                {
                    0 => new AeroBody2D(x, y, 1.0f, new AeroCircle(15)),
                    1 => new AeroBody2D(x, y, 1.0f, new AeroBox(50, 50)),
                    _ => new AeroBody2D(x, y, 1.0f, new AeroTriangle(50, 50))
                };

                body.IsStatic = true; // Make it stay in place
                Color color = cellColors[random.Next(cellColors.Length)];
                staticBodies.Add((body, color));
                world.AddPhysicsObject(body);
            }
        }
    }
    
    private void UpdateMouseBoundingArea()
    {
        if (mouseBody == null) return;

        // Create appropriate bounding area based on configuration
        mouseBoundingArea = world.CollisionSystem.Configuration.BoundingAreaType switch
        {
            BoundingAreaType.AABB => AABB2D.CreateFromShape(mouseBody.Shape, mouseBody.Position),
            BoundingAreaType.BoundingCircle => BoundingCircle.CreateFromShape(mouseBody.Shape, mouseBody.Position),
            _ => throw new ArgumentException("Unsupported bounding area type")
        };
    }

    public override void Update(GameTime gameTime)
    {
        // Toggle grid display with G key
        if (FlatKeyboard.Instance.IsKeyClicked(Keys.G))
            showGrid = !showGrid;

        // Reset scene with R key
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
            mouseBody.Position = new AeroVec2(mousePos.X, mousePos.Y);
            
            // Update bounding area
            UpdateMouseBoundingArea();
        }

        // Get potential collisions
        if (world.CollisionSystem.Configuration.BroadPhase is UniformGrid grid)
        {
            potentialCollisions = grid.FindPotentialCollisions().ToList();
        }

        world.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
    }

    private void DrawBody(AeroBody2D body, Color color)
    {
        // Transform position to render coordinates
        Vector2 renderPos = CoordinateSystem.ScreenToRender(
            new Vector2(body.Position.X, body.Position.Y), 
            _screen.Width, 
            _screen.Height
        );
    
        switch (body.Shape)
        {
            case AeroCircle circle:
                _shapes.DrawCircleFill(renderPos, circle.Radius, 32, color);
                break;
    
            case AeroBox box:
                _shapes.DrawBoxFill(renderPos, box.Width, box.Height, body.Angle, color);
                break;
    
            case AeroPolygon polygon:
                polygon.UpdateVertices(body.Angle, body.Position);
                var vertices = polygon.WorldVertices
                    .Select(v => CoordinateSystem.ScreenToRender(
                        new Vector2(v.X, v.Y),
                        _screen.Width,
                        _screen.Height))
                    .ToArray();
    
                // Draw filled polygon
                for (var i = 0; i < vertices.Length - 2; i++)
                {
                    _shapes.DrawTriangleFill(
                        vertices[0],
                        vertices[i + 1],
                        vertices[i + 2],
                        color
                    );
                }
                break;
        }
    }
    
    private void DrawBoundingArea(IBoundingArea boundingArea, Color color)
    {
        var pos = CoordinateSystem.ScreenToRender(
            new Vector2(boundingArea.Center.X, boundingArea.Center.Y),
            _screen.Width,
            _screen.Height
        );

        switch (boundingArea)
        {
            case AABB2D aabb:
                // Convert half extents to render coordinates
                Vector2 halfExtents = new(aabb.HalfExtents.X, aabb.HalfExtents.Y);
                _shapes.DrawBox(
                    pos,
                    halfExtents.X * 2, // Full width
                    halfExtents.Y * 2, // Full height
                    color
                );
                break;

            case BoundingCircle circle:
                _shapes.DrawCircle(
                    pos,
                    circle.Radius,
                    32, // segments
                    color
                );
                break;
        }
    }
    public override void Draw(GameTime gameTime)
    {
        _screen.Set();
        _game.GraphicsDevice.Clear(new Color(10, 10, 20));
        _shapes.Begin(_camera);
    
        // Draw grid with transformed coordinates
        if (showGrid)
        {
            // Draw vertical grid lines
            for (float x = 0; x < _screen.Width; x += cellSize)
            {
                var gridLineRenderXPosStart = CoordinateSystem.ScreenToRender(
                    new Vector2(x, 0), 
                    _screen.Width, 
                    _screen.Height
                );
                var gridLineRenderXPosEnd = CoordinateSystem.ScreenToRender(
                    new Vector2(x, _screen.Height), 
                    _screen.Width, 
                    _screen.Height
                );
                _shapes.DrawLine(
                    gridLineRenderXPosStart,
                    gridLineRenderXPosEnd,
                    new Color(50, 50, 50)
                );
            }
    
            // Draw horizontal grid lines
            for (float y = 0; y < _screen.Height; y += cellSize)
            {
                var gridLineRenderYPosStart = CoordinateSystem.ScreenToRender(
                    new Vector2(0, y), 
                    _screen.Width, 
                    _screen.Height
                );
                var gridLineRenderYPosEnd = CoordinateSystem.ScreenToRender(
                    new Vector2(_screen.Width, y), 
                    _screen.Width, 
                    _screen.Height
                );
                _shapes.DrawLine(
                    gridLineRenderYPosStart,
                    gridLineRenderYPosEnd,
                    new Color(50, 50, 50)
                );
            }
        }
    
        // Draw static bodies
        foreach (var (body, color) in staticBodies)
        {
            bool inCollision = potentialCollisions.Any(pair => 
                Equals(pair.Item1, body) || Equals(pair.Item2, body));
            
            Color drawColor = inCollision ? Color.Lerp(color, Color.White, 0.5f) : color;
            DrawBody(body, drawColor);
        }
    
        // Draw mouse body and its bounding area
        if (mouseBody != null)
        {
            bool inCollision = potentialCollisions.Any(pair => 
                Equals(pair.Item1, mouseBody) || Equals(pair.Item2, mouseBody));
            
            DrawBody(mouseBody, inCollision ? Color.White : Color.Orange);
            
            // Draw bounding area
            if (mouseBoundingArea != null)
            {
                DrawBoundingArea(mouseBoundingArea, Color.Yellow * 0.5f);
            }
        }
    
        _shapes.End();
        _screen.Unset();
        _screen.Present(_sprites);
    }
}