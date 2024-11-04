using AeroliteSharpEngine.AeroMath;
using AeroliteSharpEngine.Collisions.Detection.CollisionPrimitives;
using AeroliteSharpEngine.Collisions.Resolution.Interfaces;
using AeroliteSharpEngine.Core.Interfaces;

namespace AeroliteSharpEngine.Collisions.Resolution.Resolvers;

public class ImpulseMethodCollisionResolver : ICollisionResolver
{
    public void ResolveCollisions(IReadOnlyList<CollisionManifold> collisionManifolds)
    {
        foreach (var collisionManifold in collisionManifolds)
        {
            DetermineLinearImpulse(collisionManifold);
        }
    }

    private void DetermineLinearImpulse(CollisionManifold manifold)
    {
        var objectA = manifold.ObjectA;
        var objectB = manifold.ObjectB;
        
        var e = Math.Min(manifold.ObjectA.Restitution, manifold.ObjectB.Restitution);
        var vRel = objectA.Velocity - objectB.Velocity;
        
        var impulseMagnitude = -(1 + e) * AeroVec2.Dot(vRel, manifold.Normal) / (objectA.InverseMass + objectB.InverseMass);
        var jn = manifold.Normal * impulseMagnitude;
        
        objectA.ApplyImpulse(jn);
        objectB.ApplyImpulse(-jn);
    }
}