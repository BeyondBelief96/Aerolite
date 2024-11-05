using System;
using System.Collections.Generic;
using AeroliteSharpEngine.AeroMath;
using Flat.Graphics;
using Microsoft.Xna.Framework;

namespace AeroliteEngine2DTestbed.Helpers;

public static class SatDebugDrawing
{
    public static void DrawSatDebug(
        IReadOnlyList<AeroVec2> verticesA,
        IReadOnlyList<AeroVec2> verticesB,
        Screen screen,
        Shapes shapes)
    {
        // Draw normals and edges
        DrawShapeDebug(verticesA, Color.Yellow, screen, shapes);
        DrawShapeDebug(verticesB, Color.Cyan, screen, shapes);
    }

    private static void DrawShapeDebug(
        IReadOnlyList<AeroVec2> vertices,
        Color color,
        Screen screen,
        Shapes shapes)
    {
        const float normalLength = 30f;

        for (int i = 0; i < vertices.Count; i++)
        {
            var v1 = vertices[i];
            var v2 = vertices[(i + 1) % vertices.Count];
            
            // Transform vertices to render space
            var renderV1 = CoordinateSystem.ScreenToRender(
                new Vector2(v1.X, v1.Y),
                screen.Width,
                screen.Height);
            
            var renderV2 = CoordinateSystem.ScreenToRender(
                new Vector2(v2.X, v2.Y),
                screen.Width,
                screen.Height);

            // Draw edge
            shapes.DrawLine(renderV1, renderV2, color * 0.5f);

            // Calculate and draw normal
            var edge = v2 - v1;
            var normal = new AeroVec2(-edge.Y, edge.X).UnitVector();
            var midPoint = (v1 + v2) * 0.5f;
            
            var renderMidPoint = CoordinateSystem.ScreenToRender(
                new Vector2(midPoint.X, midPoint.Y),
                screen.Width,
                screen.Height);
            
            var normalEnd = CoordinateSystem.ScreenToRender(
                new Vector2(
                    midPoint.X + normal.X * normalLength,
                    midPoint.Y + normal.Y * normalLength),
                screen.Width,
                screen.Height);

            shapes.DrawLine(renderMidPoint, normalEnd, color);
            
            // Draw vertex points
            shapes.DrawCircleFill(renderV1, 3, 16, Color.Red);
        }
    }
}