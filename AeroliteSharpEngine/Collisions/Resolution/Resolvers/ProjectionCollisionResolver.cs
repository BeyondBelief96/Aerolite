using AeroliteSharpEngine.Collisions.Detection.CollisionPrimitives;
using AeroliteSharpEngine.Collisions.Resolution.Interfaces;

namespace AeroliteSharpEngine.Collisions.Resolution.Resolvers;

public class ProjectionCollisionResolver : ICollisionResolver
{
    public void ResolveCollisions(IReadOnlyList<CollisionManifold> collisionManifolds)
    {
        foreach (var collisionManifold in collisionManifolds)
        {
            var bodyA = collisionManifold.ObjectA;
            var bodyB = collisionManifold.ObjectB;

            // Don't move if both bodies are static.
            if (bodyA.IsStatic && bodyB.IsStatic) return;
            
            float da = bodyA.InverseMass * (collisionManifold.Contact.Depth / (bodyA.InverseMass + bodyB.InverseMass));
            float db = bodyB.InverseMass * (collisionManifold.Contact.Depth / (bodyB.InverseMass + bodyA.InverseMass));
            
            bodyA.Position -= da * collisionManifold.Normal;
            bodyB.Position += db * collisionManifold.Normal;
        }
    }
}