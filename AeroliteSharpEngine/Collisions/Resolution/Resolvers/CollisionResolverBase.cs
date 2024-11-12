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
            // Swap objects and normal for consistent handling
            var swappedManifold = new CollisionManifold(manifold.Contact, manifold.ObjectB, manifold.ObjectA)
            {
                Normal = -manifold.Normal,
                HasCollision = true
            };
            ResolveBodyParticle(swappedManifold, body2, particle2);
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
