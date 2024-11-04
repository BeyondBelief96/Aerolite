using System;
using System.Collections.Generic;
using AeroliteSharpEngine.AeroMath;
using Microsoft.Xna.Framework;

namespace AeroliteEngine2DTestbed.Helpers;

public static class RandomUtils
{
    private static readonly Random Random = new Random();

    public static float Range(float min, float max)
    {
        return (float)(Random.NextDouble() * (max - min) + min);
    }

    public static Vector2 RandomDirection()
    {
        float angle = Range(0, MathF.PI * 2);
        return new Vector2(MathF.Cos(angle), MathF.Sin(angle));
    }

    public static Vector2 RandomPointInCircle(float radius)
    {
        float angle = Range(0, MathF.PI * 2);
        float r = MathF.Sqrt(Range(0, 1)) * radius;  // Square root for uniform distribution
        return new Vector2(
            r * MathF.Cos(angle),
            r * MathF.Sin(angle)
        );
    }

    public static AeroVec2 RandomVelocity(float maxSpeed)
    {
        float speed = Range(0, maxSpeed);
        float angle = Range(0, MathF.PI * 2);
        return new AeroVec2(
            speed * MathF.Cos(angle),
            speed * MathF.Sin(angle)
        );
    }

    public static float RandomFloat(float min, float max)
    {
        return (float)(Random.NextDouble() * (max - min) + min);
    }

    public static int[] TriangulatePolygon(int vertexCount)
    {
        // Simple fan triangulation - works for convex polygons
        var triangles = new List<int>();

        // For n vertices, we need (n-2) triangles
        for (int i = 1; i < vertexCount - 1; i++)
        {
            triangles.Add(0);      // Center vertex
            triangles.Add(i);      // Current vertex
            triangles.Add(i + 1);  // Next vertex
        }

        return triangles.ToArray();
    }
}