namespace AeroliteSharpEngine.Collisions.Detection;

/// <summary>
/// Specifies the type of bounding area for the collision detection system to use.
/// </summary>
public enum BoundingAreaType
{
    AABB,
    BoundingCircle
}

/// <summary>
/// Enum specifying the type of shapes for the collision system to expect for detection.
/// </summary>
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

/// <summary>
/// Enumeration used to specify the collision detection algorithm to use.
/// </summary>
public enum CollisionAlgorithm
{
    /// <summary>
    /// Separating axis theorem.
    /// </summary>
    SAT,
    
    /// <summary>
    /// Gilbert–Johnson–Keerthi
    /// </summary>
    GJK,
}