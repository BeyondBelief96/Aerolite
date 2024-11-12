using AeroliteSharpEngine.Collisions.Detection.CollisionPrimitives;

namespace AeroliteSharpEngine.Collisions.Resolution.Interfaces;

/// <summary>
/// Core interface for collision resolution.
/// </summary>
public interface ICollisionResolver
{
    /// <summary>
    /// Resolves the list of collisions given the information from the collision manifolds.
    /// </summary>
    /// <param name="collisionManifolds">List of collision manifolds to resolve</param>
    void ResolveCollisions(IReadOnlyList<CollisionManifold> collisionManifolds);
}