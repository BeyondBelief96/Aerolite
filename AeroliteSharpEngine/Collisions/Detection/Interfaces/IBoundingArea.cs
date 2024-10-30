using AeroliteSharpEngine.AeroMath;

namespace AeroliteSharpEngine.Collisions.Detection.Interfaces;

/// <summary>
/// Base interface for all bounding areas
/// </summary>
public interface IBoundingArea
{
    /// <summary>
    /// The geometric center of the bounding area.
    /// </summary>
    AeroVec2 Center { get; }
    
    /// <summary>
    /// Function to re-align the bounding area based on some new position/rotation.
    /// </summary>
    /// <param name="angle"></param>
    /// <param name="position"></param>
    void Realign(float angle, AeroVec2 position);
    
    /// <summary>
    /// Intersection test of this bounding area and the given bounding area.
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    bool Intersects(IBoundingArea other);
}