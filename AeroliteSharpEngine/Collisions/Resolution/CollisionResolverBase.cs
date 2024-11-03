using AeroliteSharpEngine.Collisions.Detection.CollisionPrimitives;
using AeroliteSharpEngine.Collisions.Resolution.Interfaces;

namespace AeroliteSharpEngine.Collisions.Resolution;

public abstract class CollisionResolverBase : ICollisionResolver
{
    public void ResolveCollisions(IReadOnlyList<CollisionManifold> collisionManifolds, float dt)
    {
        
    }
    
    protected abstract void PreSolve(IReadOnlyList<CollisionManifold> collisionManifolds, float dt);
    protected abstract void Solve(IReadOnlyList<CollisionManifold> collisionManifolds, float dt);
    protected abstract void PostSolve(IReadOnlyList<CollisionManifold> collisionManifolds, float dt);
}