using AeroliteSharpEngine.AeroMath;
using AeroliteSharpEngine.Core.Interfaces;

namespace AeroliteSharpEngine.Collisions.Detection;

/// <summary>
/// Represents the result of a collision test between two objects
/// </summary>
public struct CollisionManifold
{
    /// <summary>
    /// Boolean that represents if this manifold represents a collision or not.
    /// </summary>
    public bool HasCollision;
    
    /// <summary>
    /// Represents the collision normal, pointing from objectB to objectA
    /// </summary>
    public AeroVec2 Normal;
    
    /// <summary>
    /// The collision depth. This represents the maximum amount of penetration between the two objects.
    /// </summary>
    public float Depth;
    
    /// <summary>
    /// The inner most point of collision between the two objects.
    /// </summary>
    public AeroVec2 Point;
    
    /// <summary>
    /// The first object in the collision.
    /// </summary>
    public IPhysicsObject2D Object2DA;
    
    /// <summary>
    /// The second object in the collision.
    /// </summary>
    public IPhysicsObject2D Object2DB;
}