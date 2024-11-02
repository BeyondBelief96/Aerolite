namespace AeroliteSharpEngine.AeroMath;

/// <summary>
/// Basic mathematical primitive representing a 2D plane with a point and a normal.
/// </summary>
public struct AeroPlane
{
    /// <summary>
    /// A point on the plane.
    /// </summary>
    public AeroVec2 Point { get; set; }
    
    /// <summary>
    /// A vector representing the normal vector to the plane.
    /// </summary>
    public AeroVec2 Normal { get; set; }
}