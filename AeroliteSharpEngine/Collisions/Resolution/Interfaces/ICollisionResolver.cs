using AeroliteSharpEngine.Collisions.Detection.CollisionPrimitives;

namespace AeroliteSharpEngine.Collisions.Resolution.Interfaces;

/// <summary>
/// Core interface for collision resolution. Implementations of this interface apply various collision resolution solvers.
/// </summary>
public interface ICollisionResolver
{
    /// <summary>
    /// Resolves collisions based on the provided collision manifolds
    /// </summary>
    /// <param name="collisionManifolds">List of collision manifolds to resolve</param>
    /// <param name="dt">Time step delta</param>
    void ResolveCollisions(IReadOnlyList<CollisionManifold> collisionManifolds, float dt);
}