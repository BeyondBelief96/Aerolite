using AeroliteSharpEngine.AeroMath;

namespace AeroliteSharpEngine.Collisions.Detection.CollisionPrimitives;

/// <summary>
/// Represents a single contact point between two colliding objects
/// </summary>
public struct ContactPoint
{
    /// <summary>
    /// The point of contact on object A in world space
    /// </summary>
    public AeroVec2 PointOnA;
        
    /// <summary>
    /// The point of contact on object B in world space
    /// </summary>
    public AeroVec2 PointOnB;
        
    /// <summary>
    /// The penetration depth at this contact point
    /// </summary>
    public float Depth;
}