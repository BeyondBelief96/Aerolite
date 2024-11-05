using AeroliteSharpEngine.AeroMath;
using AeroliteSharpEngine.Collisions.Detection.CollisionPrimitives;
using AeroliteSharpEngine.Collisions.Resolution.Interfaces;
using AeroliteSharpEngine.Core.Interfaces;

namespace AeroliteSharpEngine.Collisions.Resolution.Resolvers;

public class ImpulseMethodCollisionResolver : ICollisionResolver
{
    private readonly ProjectionCollisionResolver  _projectionResolver = new();
    public void ResolveCollisions(IReadOnlyList<CollisionManifold> collisionManifolds)
    {
        _projectionResolver.ResolveCollisions(collisionManifolds);
        foreach (var collisionManifold in collisionManifolds)
        {
            DetermineLinearImpulse(collisionManifold);
        }
    }

    private static void DetermineLinearImpulse(CollisionManifold manifold)
    {
        var objectA = manifold.ObjectA;
        var objectB = manifold.ObjectB;
        
        var e = Math.Min(manifold.ObjectA.Restitution, manifold.ObjectB.Restitution);
        var vRel = objectA.Velocity - objectB.Velocity;
        var vRelNormal = vRel.Dot(manifold.Normal);
        
        var impulseDirection = manifold.Normal;
        var impulseMagnitude = (-(1 + e) * vRelNormal) / (objectA.InverseMass + objectB.InverseMass);
        var jn = impulseDirection * impulseMagnitude;
        
        objectA.ApplyImpulse(jn);
        objectB.ApplyImpulse(-jn);
    }
}