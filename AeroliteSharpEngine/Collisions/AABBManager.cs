using AeroliteSharpEngine.AeroMath;
using AeroliteSharpEngine.Core;
using AeroliteSharpEngine.Core.Interfaces;
using AeroliteSharpEngine.Shapes;

namespace AeroliteSharpEngine.Collisions
{
    /// <summary>
    /// Represents an axis-aligned bounding box.
    /// </summary>
    public readonly struct AABB
    {
        private readonly AeroVec2 _min;
        private readonly AeroVec2 _max;

        private AABB(AeroVec2 min, AeroVec2 max)
        {
            _min = min;
            _max = max;
        }

        public bool Intersects(in AABB other)
        {
            return !(_max.X <= other._min.X || _min.X >= other._max.X ||
                    _max.Y <= other._min.Y || _min.Y >= other._max.Y);
        }

        public static AABB CreateFromShape(in AeroShape2D shape, in AeroVec2 position)
        {
            if (shape is AeroCircle circle)
            {
                return new AABB(
                    new AeroVec2(position.X - circle.Radius, position.Y - circle.Radius),
                    new AeroVec2(position.X + circle.Radius, position.Y + circle.Radius));
            }

            if (shape is AeroPolygon polygon)
            {
                var worldVerts = polygon.WorldVertices;
                var min = new AeroVec2(float.MaxValue, float.MaxValue);
                var max = new AeroVec2(float.MinValue, float.MinValue);

                // Unroll loop for first vertex to set initial min/max
                if (worldVerts.Count > 0)
                {
                    var firstVertex = worldVerts[0];
                    min.X = max.X = firstVertex.X;
                    min.Y = max.Y = firstVertex.Y;

                    // Process remaining vertices
                    for (int i = 1; i < worldVerts.Count; i++)
                    {
                        var vertex = worldVerts[i];
                        min.X = Math.Min(min.X, vertex.X);
                        min.Y = Math.Min(min.Y, vertex.Y);
                        max.X = Math.Max(max.X, vertex.X);
                        max.Y = Math.Max(max.Y, vertex.Y);
                    }
                }

                return new AABB(min, max);
            }

            throw new ArgumentException("Unknown shape type");
        }

        public static AABB CreateFromParticle(in AeroParticle2D particle)
        {
            return new AABB(
                new AeroVec2(particle.Position.X - particle.Radius, particle.Position.Y - particle.Radius),
                new AeroVec2(particle.Position.X + particle.Radius, particle.Position.Y + particle.Radius)
            );
        }
    }

    /// <summary>
    /// Static class used to create AABB's and test AABB intersections.
    /// </summary>
    internal static class AABBTest
    {
        public static bool TestIntersection(IPhysicsObject a, IPhysicsObject b)
        {
            var aabbA = GetAABB(a);
            var aabbB = GetAABB(b);
            return aabbA.Intersects(aabbB);
        }

        private static AABB GetAABB(IPhysicsObject obj)
        {
            return obj switch
            {
                AeroBody2D body => AABB.CreateFromShape(body.Shape, body.Position),
                AeroParticle2D particle => AABB.CreateFromParticle(particle),
                _ => throw new ArgumentException($"Unknown physics object type: {obj.GetType()}")
            };
        }
    }
}