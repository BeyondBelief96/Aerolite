using AeroliteSharpEngine.AeroMath;
using AeroliteSharpEngine.Collisions.Interfaces;
using AeroliteSharpEngine.Shapes;

namespace AeroliteSharpEngine.Collisions.BoundingAreas;

/// <summary>
    /// Represents a bounding circle
    /// </summary>
    public struct BoundingCircle : IBoundingArea
    {
        public AeroVec2 Center { get; private set; }

        public float Radius { get; }

        private BoundingCircle(AeroVec2 center, float radius)
        {
            Center = center;
            Radius = radius;
        }

        public bool Intersects(IBoundingArea other)
        {
            return other switch
            {
                AABB2D aabb => aabb.Intersects(this), // Reuse AABB's implementation
                BoundingCircle circle => IntersectsCircle(circle),
                _ => throw new ArgumentException("Unknown bounding volume type")
            };
        }

        private bool IntersectsCircle(BoundingCircle other)
        {
            var distanceSquared = (Center - other.Center).MagnitudeSquared;
            var radiusSum = Radius + other.Radius;
            return distanceSquared <= (radiusSum * radiusSum);
        }

        public void Realign(float angle, AeroVec2 position)
        {
            Center = position;
            // Radius doesn't change since circles are rotation invariant
        }

        public static BoundingCircle CreateFromShape(AeroShape2D shape, AeroVec2 position)
        {
            switch (shape)
            {
                case AeroCircle circle:
                    return new BoundingCircle(position, circle.Radius);

                case AeroPolygon polygon:
                {
                    // Find the furthest vertex from center to determine radius
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
    }