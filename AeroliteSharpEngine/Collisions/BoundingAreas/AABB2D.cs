using AeroliteSharpEngine.AeroMath;
using AeroliteSharpEngine.Collisions.Interfaces;
using AeroliteSharpEngine.Shapes;

namespace AeroliteSharpEngine.Collisions.BoundingAreas;

/// <summary>
/// Represents an axis-aligned bounding box using center-radius representation
/// </summary>
public struct AABB2D : IBoundingArea
{
    private AeroVec2 _center;    // Center point
    private readonly AeroVec2 _halfExtents; // Half-widths along each axis (radius)

    public AeroVec2 Center => _center;

    private AABB2D(AeroVec2 center, AeroVec2 halfExtents)
    {
        _center = center;
        _halfExtents = halfExtents;
    }

    public bool Intersects(IBoundingArea other)
    {
        return other switch
        {
            AABB2D aabb => IntersectsAABB(aabb),
            BoundingCircle circle => IntersectsCircle(circle),
            _ => throw new ArgumentException("Unknown bounding volume type")
        };
    }

    private bool IntersectsAABB(AABB2D other)
    {
        return !(Math.Abs(_center.X - other._center.X) > (_halfExtents.X + other._halfExtents.X) || 
                 Math.Abs(_center.Y - other._center.Y) > (_halfExtents.Y + other._halfExtents.Y));
    }

    private bool IntersectsCircle(BoundingCircle circle)
    {
        // Find the closest point on AABB to circle center
        var closestX = Math.Max(_center.X - _halfExtents.X, 
            Math.Min(circle.Center.X, _center.X + _halfExtents.X));
        var closestY = Math.Max(_center.Y - _halfExtents.Y, 
            Math.Min(circle.Center.Y, _center.Y + _halfExtents.Y));

        // Calculate squared distance to closest point
        var dx = closestX - circle.Center.X;
        var dy = closestY - circle.Center.Y;
        var distanceSquared = dx * dx + dy * dy;

        return distanceSquared <= (circle.Radius * circle.Radius);
    }

    public void Realign(float angle, AeroVec2 position)
    {
        _center = position;
        // No need to adjust halfExtents since it's axis-aligned
    }

    public static AABB2D CreateFromShape(AeroShape2D shape, AeroVec2 position)
    {
        switch (shape)
        {
            case AeroCircle circle:
                return new AABB2D(
                    position,
                    new AeroVec2(circle.Radius, circle.Radius));

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
}