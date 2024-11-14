using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using AeroliteSharpEngine.AeroMath;
using AeroliteSharpEngine.Collisions.Detection.BoundingAreas;
using AeroliteSharpEngine.Collisions.Detection.BroadPhase;
using AeroliteSharpEngine.Collisions.Detection.CollisionPrimitives;
using AeroliteSharpEngine.Collisions.Detection.Interfaces;
using AeroliteSharpEngine.Core;
using AeroliteSharpEngine.Core.Interfaces;
using AeroliteSharpEngine.Shapes;
using Flat.Graphics;
using Microsoft.Xna.Framework;

namespace AeroliteEngine2DTestbed.Helpers;

public static class AeroDrawingHelpers
{
    /// <summary>
    /// Helper function to draw an <see cref="IPhysicsObject2D"/> to the screen using monogame.
    /// </summary>
    /// <param name="physicsObject">The body being drawn.</param>
    /// <param name="color">The color to draw the body.</param>
    /// <param name="shapes">Helper class to draw shapes to the screen.</param>
    /// <param name="screen">Helper class containing information about the simulated screen.</param>
    public static void DrawPhysicsObject2D(IPhysicsObject2D physicsObject, Color color, Shapes shapes, Screen screen)
    {
        Vector2 renderPos = CoordinateSystem.ScreenToRender(
            new Vector2(physicsObject.Position.X, physicsObject.Position.Y),
            screen.Width,
            screen.Height
        );

        if (physicsObject is AeroParticle2D particle)
        {
            shapes.DrawCircleFill(renderPos, ((AeroCircle)particle.Shape).Radius, 32, color);
        }
        else if(physicsObject is IBody2D body)
        {
            switch (physicsObject.Shape)
            {
                case AeroCircle circle:
                    shapes.DrawCircle(renderPos, circle.Radius, 32, color);
                
                    // Calculate the end point of the rotation line
                    var direction = new Vector2(
                        MathF.Cos(-body.Angle), // Negate angle to match your clockwise rotation
                        MathF.Sin(-body.Angle)
                    );
                    var lineEnd = renderPos + direction * circle.Radius;
                
                    shapes.DrawLine(renderPos, lineEnd, color);
                    break;

                // The draw function rotates counter-clockwise, where my physics engine rotation is clockwise, so we negate the angle.
                case AeroBox box:
                    shapes.DrawBoxFill(renderPos, box.Width, box.Height, -body.Angle, color);
                    break;
                case AeroTriangle triangle:
                    var triangleVertices = triangle.WorldVertices
                        .Select(v => CoordinateSystem.ScreenToRender(
                            new Vector2(v.X, v.Y),
                            screen.Width,
                            screen.Height))
                        .ToArray();
                    for (var i = 0; i < triangleVertices.Length - 2; i++)
                    {
                        shapes.DrawTriangleFill(
                            triangleVertices[0],
                            triangleVertices[i + 1],
                            triangleVertices[i + 2],
                            color
                        );
                    }

                    break;

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
    }
    
    public static void KeepParticleInScreenBounds(IEnumerable<IPhysicsObject2D> physicsObjects, Screen screen)
    {
        foreach (var physicsObject in physicsObjects)
        {
            var radius = ((AeroCircle)physicsObject.Shape).Radius;
            bool bounced = false;
            var pos = physicsObject.Position;
            var vel = physicsObject.Velocity;

            if (pos.X < radius || pos.X > screen.Width - radius)
            {
                vel.X *= -1;
                bounced = true;
            }
            if (pos.Y < radius || pos.Y > screen.Height - radius)
            {
                vel.Y *= -1;
                bounced = true;
            }

            if (bounced)
            {
                physicsObject.Position = new AeroVec2(
                    Math.Clamp(pos.X, radius, screen.Width - radius),
                    Math.Clamp(pos.Y, radius, screen.Height - radius)
                );
                physicsObject.Velocity = vel;
            }
            
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
    
    public static void DrawQuadTree(DynamicQuadTree quadTree, Color nodeColor, Color leafNodeColor, Screen screen, Shapes shapes)
    {
        foreach (var node in quadTree.GetAllNodes())
        {
            var center = CoordinateSystem.ScreenToRender(new Vector2(node.Center.X, node.Center.Y), screen.Width, screen.Height);

            shapes.DrawBox(center, node.HalfDimension.X * 2.0f, node.HalfDimension.Y * 2.0f, Color.Green);
        }
    }
}