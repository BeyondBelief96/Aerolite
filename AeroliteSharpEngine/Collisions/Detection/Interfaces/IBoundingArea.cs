using AeroliteSharpEngine.AeroMath;

namespace AeroliteSharpEngine.Collisions.Detection.Interfaces;

/// <summary>
/// Base interface for all bounding areas.
/// </summary>
public interface IBoundingArea
{
    /// <summary>
    /// The geometric center of the bounding area.
    /// </summary>
    AeroVec2 Center { get; }
    
    /// <summary>
    /// Function to re-align the bounding area based on some new position/rotation of the underlying object.
    /// </summary>
    /// <param name="angle">The angle of rotation of the wrapped physics object.</param>
    /// <param name="position">The position of the wrapped physics object.</param>
    void Realign(float angle, AeroVec2 position);
    
    /// <summary>
    /// Tests intersection of this bounding area and the given bounding area.
    /// </summary>
    /// <param name="other">The bounding area which we are testing intersection with.</param>
    /// <returns>Whether these two bounding areas are intersecting.</returns>
    bool Intersects(IBoundingArea other);
}