using AeroliteSharpEngine.AeroMath;
using AeroliteSharpEngine.Collisions.Detection.Interfaces;
using AeroliteSharpEngine.Shapes;

namespace AeroliteSharpEngine.Collisions.Detection.BoundingAreas;

/// <summary>
/// Represents an axis-aligned bounding box using center-radius representation
/// </summary>
// ReSharper disable once InconsistentNaming
public struct AABB2D : IBoundingArea
{
    public AeroVec2 Center { get; private set; }
    public AeroVec2 HalfExtents { get; private set; }

    private AABB2D(AeroVec2 center, AeroVec2 halfExtents)
    {
        Center = center;
        HalfExtents = halfExtents;
    }

    public void Realign(float angle, AeroVec2 position)
    {
        // We need to recalculate half extents when the object rotates
        if (angle != 0)
        {
            var cos = MathF.Abs(MathF.Cos(angle));
            var sin = MathF.Abs(MathF.Sin(angle));
            
            // Compute new half extents that will contain the rotated box
            HalfExtents = new AeroVec2(
                HalfExtents.X * cos + HalfExtents.Y * sin,
                HalfExtents.X * sin + HalfExtents.Y * cos
            );
        }
        
        Center = position;
    }

    public static AABB2D CreateFromShape(AeroShape2D shape, AeroVec2 position)
    {
        switch (shape)
        {
            case AeroCircle circle:
                return new AABB2D(
                    position,
                    new AeroVec2(circle.Radius, circle.Radius));
            case AeroBox box:
            {
                return new AABB2D(
                    position,
                    new AeroVec2(box.Width * 0.5f, box.Height * 0.5f)
                );
            }
            case AeroPolygon polygon:
            {
                var worldVerts = polygon.WorldVertices;
                if (worldVerts.Count == 0)
                    return new AABB2D(position, AeroVec2.Zero);

                var min = new AeroVec2(float.MaxValue, float.MaxValue);
                var max = new AeroVec2(float.MinValue, float.MinValue);

                foreach (var vertex in worldVerts)
                {
                    min.X = Math.Min(min.X, vertex.X);
                    min.Y = Math.Min(min.Y, vertex.Y);
                    max.X = Math.Max(max.X, vertex.X);
                    max.Y = Math.Max(max.Y, vertex.Y);
                }

                var center = (min + max) * 0.5f;
                var halfExtents = (max - min) * 0.5f;
                return new AABB2D(center, halfExtents);
            }

            default:
                throw new ArgumentException("Unknown shape type");
        }
    }

    public bool Intersects(IBoundingArea other)
    {
        // Delegate to the static test class
        return BoundingAreaTests.Test(this, other);
    }
}