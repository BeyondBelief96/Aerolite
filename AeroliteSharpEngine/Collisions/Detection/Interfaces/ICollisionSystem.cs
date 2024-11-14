using AeroliteSharpEngine.Collisions.Detection.CollisionPrimitives;
using AeroliteSharpEngine.Core.Interfaces;

namespace AeroliteSharpEngine.Collisions.Detection.Interfaces;

/// <summary>
/// Abstraction for the engine's collision system.
/// </summary>
public interface ICollisionSystem
{
    /// <summary>
    /// Configuration object for configuring various parameters of the collision system of the engine.
    /// </summary>
    CollisionSystemConfiguration Configuration { get; }
    
    /// <summary>
    /// Collection of collision manifolds detected for the current frame.
    /// </summary>
    IEnumerable<CollisionManifold> Collisions { get; }
    
    /// <summary>
    /// Collection of physic object pairs that have been marked as potentially colliding (passing broad phase).
    /// </summary>
    IEnumerable<(IPhysicsObject2D, IPhysicsObject2D)> PotentialPairs { get; }
    
    /// <summary>
    /// Clears all caches and collisions from the collision detection system.
    /// </summary>
    void Clear();
    
    /// <summary>
    /// Performs collision detection and resolution on the collection of <see cref="IPhysicsObject2D"/>
    /// </summary>
    /// <param name="objects">The collection of simulation objects to perform collision checks and resolutions on.</param>
    /// <returns>The collision manifolds which contain collision information.</returns>
    void HandleCollisions(IReadOnlyList<IPhysicsObject2D> objects);
}