using AeroliteSharpEngine.Collisions.Detection.CollisionPrimitives;
using AeroliteSharpEngine.Collisions.Resolution.Interfaces;
using AeroliteSharpEngine.Core;
using AeroliteSharpEngine.Core.Interfaces;

namespace AeroliteSharpEngine.Collisions.Resolution.Resolvers;

/// <summary>
/// Base class that dispatches collision resolution based on object types
/// </summary>
public abstract class CollisionResolverBase : ICollisionResolver
{
    private readonly ProjectionCollisionResolver _projectionResolver = new();

    public void ResolveCollisions(IReadOnlyList<CollisionManifold> collisionManifolds)
    {
        _projectionResolver.ResolveCollisions(collisionManifolds);
        
        foreach (var manifold in collisionManifolds)
        {
            DispatchResolution(manifold);
        }
    }

    private void DispatchResolution(CollisionManifold manifold)
    {
        if (manifold.ObjectA is IBody2D bodyA && manifold.ObjectB is IBody2D bodyB)
        {
            ResolveBodyBody(manifold, bodyA, bodyB);
        }
        else if (manifold.ObjectA is IBody2D body && manifold.ObjectB is AeroParticle2D particle)
        {
            ResolveBodyParticle(manifold, body, particle);
        }
        else if (manifold.ObjectA is AeroParticle2D particle2 && manifold.ObjectB is IBody2D body2)
        {
            // Get swapped manifold from pool
            var swappedManifold = CollisionPoolService.Instance.GetManifold();
            swappedManifold.ObjectA = manifold.ObjectB;
            swappedManifold.ObjectB = manifold.ObjectA;
            swappedManifold.Normal = -manifold.Normal;
            swappedManifold.HasCollision = true;
            swappedManifold.Contact = manifold.Contact; // Reuse contact point
            
            ResolveBodyParticle(swappedManifold, body2, particle2);
            
            // Return the swapped manifold to the pool
            CollisionPoolService.Instance.ReturnManifold(swappedManifold);
        }
        else if (manifold.ObjectA is AeroParticle2D particleA && manifold.ObjectB is AeroParticle2D particleB)
        {
            ResolveParticleParticle(manifold, particleA, particleB);
        }
    }

    protected abstract void ResolveBodyBody(CollisionManifold manifold, IBody2D bodyA, IBody2D bodyB);
    protected abstract void ResolveBodyParticle(CollisionManifold manifold, IBody2D body, AeroParticle2D particle);
    protected abstract void ResolveParticleParticle(CollisionManifold manifold, AeroParticle2D particleA, AeroParticle2D particleB);
}
