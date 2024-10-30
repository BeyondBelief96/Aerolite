// Then fix the AeroTriangle class

using AeroliteSharpEngine.AeroMath;
using AeroliteSharpEngine.Shapes.Interfaces;

namespace AeroliteSharpEngine.Shapes;

public class AeroTriangle : AeroPolygon, IConvexShape
{
    /// <summary>
    /// Creates an equilateral triangle centered at origin with given side length
    /// </summary>
    public AeroTriangle(float sideLength)
        : base(GenerateEquilateralTriangle(sideLength))
    {
    }

    /// <summary>
    /// Creates a triangle with given base width and height, centered at origin
    /// </summary>
    public AeroTriangle(float baseWidth, float height)
        : base(GenerateIsoscelesTriangle(baseWidth, height))
    {
    }

    /// <summary>
    /// Creates a triangle from three explicit vertices
    /// </summary>
    private AeroTriangle(AeroVec2 v1, AeroVec2 v2, AeroVec2 v3)
        : base(new[] { v1, v2, v3 })
    {
        if (LocalVertices.Count != 3)
        {
            throw new ArgumentException("Triangle must have exactly 3 vertices");
        }
        UpdateCachedProperties();
    }

    /// <summary>
    /// Creates a right triangle with given width and height, centered appropriately
    /// </summary>
    public static AeroTriangle CreateRightTriangle(float width, float height)
    {
        // Center the right triangle so its centroid is at origin
        float cx = width / 3.0f;  // Centroid is at 1/3 of the width
        float cy = height / 3.0f; // Centroid is at 1/3 of the height

        return new AeroTriangle(
            new AeroVec2(-cx, -cy),           // Bottom left
            new AeroVec2(width - cx, -cy),    // Bottom right
            new AeroVec2(-cx, height - cy)    // Top
        );
    }

    private static AeroVec2[] GenerateEquilateralTriangle(float sideLength)
    {
        float height = sideLength * MathF.Sqrt(3) / 2;
        float halfWidth = sideLength / 2;

        return new[]
        {
            new AeroVec2(0, height * (2.0f/3.0f)),         // Top vertex
            new AeroVec2(-halfWidth, -height/3.0f),        // Bottom left
            new AeroVec2(halfWidth, -height/3.0f)          // Bottom right
        };
    }

    private static AeroVec2[] GenerateIsoscelesTriangle(float baseWidth, float height)
    {
        float halfWidth = baseWidth / 2;
        float heightOffset = height / 3.0f;  // Offset to center at origin

        return new[]
        {
            new AeroVec2(0, height - heightOffset),        // Top vertex
            new AeroVec2(-halfWidth, -heightOffset),       // Bottom left
            new AeroVec2(halfWidth, -heightOffset)         // Bottom right
        };
    }

    protected sealed override void UpdateCachedProperties()
    {
        var v1 = LocalVertices[0];
        var v2 = LocalVertices[1];
        var v3 = LocalVertices[2];

        // Calculate area using cross product
        CachedArea = Math.Abs((v2 - v1).Cross(v3 - v1)) / 2.0f;

        // Calculate centroid (average of vertices)
        CachedCentroid = (v1 + v2 + v3) / 3.0f;

        // Calculate moment of inertia
        float a = (v2 - v1).Magnitude;
        float b = (v3 - v2).Magnitude;
        float c = (v1 - v3).Magnitude;

        // I = (a² + b² + c²) / 36 for a triangle about its centroid
        CachedMomentOfInertia = (a * a + b * b + c * c) / 36.0f;
        NeedsUpdate = false;
    }

    public override ShapeType GetShapeType()
    {
        return ShapeType.Triangle;
    }
}