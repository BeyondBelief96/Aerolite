using AeroliteSharpEngine.AeroMath;
using AeroliteSharpEngine.Collisions.Detection.Interfaces;
using AeroliteSharpEngine.Shapes;

namespace AeroliteSharpEngine.Collisions.Detection.BoundingAreas;

/// <summary>
/// Represents a bounding circle
/// </summary>
public struct BoundingCircle : IBoundingArea
{
    public AeroVec2 Center { get; private set; }
    public float Radius { get; }

    public BoundingCircle(AeroVec2 center, float radius)
    {
        Center = center;
        Radius = radius;
    }

    public void Realign(float angle, AeroVec2 position)
    {
        // Circles only need position update, angle doesn't affect them
        Center = position;
    }

    public static BoundingCircle CreateFromShape(AeroShape2D shape, AeroVec2 position)
    {
        switch (shape)
        {
            case AeroCircle circle:
                return new BoundingCircle(position, circle.Radius);

            case AeroPolygon polygon:
            {
                var maxRadiusSquared = 0f;
                foreach (var vertex in polygon.WorldVertices)
                {
                    var distSquared = (vertex - position).MagnitudeSquared;
                    maxRadiusSquared = Math.Max(maxRadiusSquared, distSquared);
                }
                return new BoundingCircle(position, MathF.Sqrt(maxRadiusSquared));
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