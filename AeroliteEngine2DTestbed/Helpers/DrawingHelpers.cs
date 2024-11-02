using System.Linq;
using AeroliteSharpEngine.Core;
using AeroliteSharpEngine.Core.Interfaces;
using AeroliteSharpEngine.Shapes;
using Flat.Graphics;
using Microsoft.Xna.Framework;

namespace AeroliteEngine2DTestbed.Helpers;

public static class DrawingHelpers
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

            case AeroBox box:
                shapes.DrawBoxFill(renderPos, box.Width, box.Height, body.Angle, color);
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