namespace AeroliteSharpEngine.Collisions.Detection;

/// <summary>
/// Specifies the type of bounding area for the collision detection system to use.
/// </summary>
public enum BoundingAreaType
{
    AABB,
    BoundingCircle
}

public enum CollisionSystemType
{
    /// <summary>
    /// Optimizes the collision detection for convex shapes only. Uses faster algorithms taking advantage
    /// of convex geometry.
    /// </summary>
    ConvexOnly,
        
    /// <summary>
    /// Handles both convex and concave shapes. Less performant due to having to handle the concave case.
    /// </summary>
    General, 
}