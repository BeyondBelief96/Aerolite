using System.Linq;
using AeroliteSharpEngine.Collisions.Detection.BoundingAreas;
using AeroliteSharpEngine.Collisions.Detection.CollisionPrimitives;
using AeroliteSharpEngine.Collisions.Detection.Interfaces;
using AeroliteSharpEngine.Core.Interfaces;
using AeroliteSharpEngine.Shapes;
using Flat.Graphics;
using Microsoft.Xna.Framework;

namespace AeroliteEngine2DTestbed.Helpers;

public static class AeroDrawingHelpers
{
    /// <summary>
    /// Helper function to draw an <see cref="IBody2D"/> to the screen using monogame.
    /// </summary>
    /// <param name="body">The body being drawn.</param>
    /// <param name="color">The color to draw the body.</param>
    /// <param name="shapes">Helper class to draw shapes to the screen.</param>
    /// <param name="screen">Helper class containing information about the simulated screen.</param>
    public static void DrawBody(IBody2D body, Color color, Shapes shapes, Screen screen)
    {
        Vector2 renderPos = CoordinateSystem.ScreenToRender(
            new Vector2(body.Position.X, body.Position.Y),
            screen.Width,
            screen.Height
        );

        switch (body.Shape)
        {
            case AeroCircle circle:
                shapes.DrawCircleFill(renderPos, circle.Radius, 32, color);
                break;

            // The draw function rotates counter-clockwise, where my physics engine rotation is clockwise, so we negate the angle.
            // case AeroBox box:
            //     shapes.DrawBoxFill(renderPos, box.Width, box.Height, -body.Angle, color);
            //     break;

            case AeroPolygon polygon:
                var vertices = polygon.WorldVertices
                    .Select(v => CoordinateSystem.ScreenToRender(
                        new Vector2(v.X, v.Y),
                        screen.Width,
                        screen.Height))
                    .ToArray();
                for (var i = 0; i < vertices.Length - 2; i++)
                {
                    shapes.DrawTriangleFill(
                        vertices[0],
                        vertices[i + 1],
                        vertices[i + 2],
                        color
                    );
                }
                break;
        }
    }
    
    public static void DrawBoundingArea(IBoundingArea boundingArea, Color color, Screen screen, Shapes shapes)
    {
        var pos = CoordinateSystem.ScreenToRender(
            new Vector2(boundingArea.Center.X, boundingArea.Center.Y),
            screen.Width,
            screen.Height
        );

        switch (boundingArea)
        {
            case AABB2D aabb:
                // Convert half extents to render coordinates
                Vector2 halfExtents = new(aabb.HalfExtents.X, aabb.HalfExtents.Y);
                shapes.DrawBox(
                    pos,
                    halfExtents.X * 2, // Full width
                    halfExtents.Y * 2, // Full height
                    color
                );
                break;

            case BoundingCircle circle:
                shapes.DrawCircle(
                    pos,
                    circle.Radius,
                    32, // segments
                    color
                );
                break;
        }
    }
    
    public static void DrawCollisionInfo(CollisionManifold manifold, Screen screen, Shapes shapes)
    {
        if (!manifold.HasCollision) return;

        // Draw points on both bodies
        var pointOnA = CoordinateSystem.ScreenToRender(
            new Vector2(manifold.Contact.StartPoint.X, manifold.Contact.StartPoint.Y),
            screen.Width,
            screen.Height);

        var pointOnB = CoordinateSystem.ScreenToRender(
            new Vector2(manifold.Contact.EndPoint.X, manifold.Contact.EndPoint.Y),
            screen.Width,
            screen.Height);
        
        var normalEndPoint = CoordinateSystem.ScreenToRender(
            new Vector2(manifold.Contact.StartPoint.X + manifold.Normal.X * 15, manifold.Contact.StartPoint.Y + manifold.Normal.Y * 15),
            screen.Width, screen.Height);

        // Draw contact points
        shapes.DrawCircleFill(pointOnA, 3, 16, Color.Magenta); // Point on A
        shapes.DrawCircleFill(pointOnB, 3, 16, Color.Magenta);  // Point on B
        shapes.DrawLine(pointOnA, normalEndPoint, Color.Magenta);
    }

    public static void DrawGrid(Screen screen, Shapes shapes, float cellSize)
    {
        for (float x = 0; x <= screen.Width; x += cellSize)
        {
            var start = CoordinateSystem.ScreenToRender(
                new Vector2(x, 0),
                screen.Width,
                screen.Height
            );
            var end = CoordinateSystem.ScreenToRender(
                new Vector2(x, screen.Height),
                screen.Width,
                screen.Height
            );
            shapes.DrawLine(start, end, new Color(50, 50, 50));
        }

        for (float y = 0; y <= screen.Height; y += cellSize)
        {
            var start = CoordinateSystem.ScreenToRender(
                new Vector2(0, y),
                screen.Width,
                screen.Height
            );
            var end = CoordinateSystem.ScreenToRender(
                new Vector2(screen.Width, y),
                screen.Width,
                screen.Height
            );
            shapes.DrawLine(start, end, new Color(50, 50, 50));
        }
    }
}