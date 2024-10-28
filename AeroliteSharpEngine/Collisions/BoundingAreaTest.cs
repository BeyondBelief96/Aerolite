using AeroliteSharpEngine.Collisions.BoundingAreas;
using AeroliteSharpEngine.Collisions.Interfaces;
using AeroliteSharpEngine.Core;
using AeroliteSharpEngine.Core.Interfaces;

namespace AeroliteSharpEngine.Collisions
{
    /// <summary>
    /// Helper class for bounding volume tests
    /// </summary>
    public static class BoundingAreaTest 
    {
        public static bool TestIntersection(IPhysicsObject a, IPhysicsObject b, BoundingVolumeType type = BoundingVolumeType.AABB)
        {
            var boundingVolumeA = GetBoundingArea(a, type);
            var boundingVolumeB = GetBoundingArea(b, type);
            return boundingVolumeA.Intersects(boundingVolumeB);
        }

        private static IBoundingArea GetBoundingArea(IPhysicsObject obj, BoundingVolumeType type)
        {
            return type switch
            {
                BoundingVolumeType.AABB => obj switch
                {
                    AeroBody2D body => AABB2D.CreateFromShape(body.Shape, body.Position),
                    AeroParticle2D particle => AABB2D.CreateFromShape(new AeroCircle(particle.Radius), particle.Position),
                    _ => throw new ArgumentException($"Unknown physics object type: {obj.GetType()}")
                },
                BoundingVolumeType.Circle => obj switch
                {
                    AeroBody2D body => BoundingCircle.CreateFromShape(body.Shape, body.Position),
                    AeroParticle2D particle => BoundingCircle.CreateFromShape(new AeroCircle(particle.Radius), particle.Position),
                    _ => throw new ArgumentException($"Unknown physics object type: {obj.GetType()}")
                },
                _ => throw new ArgumentException("Unknown bounding volume type")
            };
        }
    }

    public enum BoundingVolumeType
    {
        AABB,
        Circle
    }
}