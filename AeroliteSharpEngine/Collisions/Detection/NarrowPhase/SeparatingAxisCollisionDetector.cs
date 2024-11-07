using AeroliteSharpEngine.AeroMath;
using AeroliteSharpEngine.Collisions.Detection.CollisionPrimitives;
using AeroliteSharpEngine.Shapes;

namespace AeroliteSharpEngine.Collisions.Detection.NarrowPhase;

/// <summary>
/// Implementation of collision detection using the Separating Axis Theorem (SAT)
/// for convex shapes.
/// </summary>
public static class SeparatingAxisCollisionDetector
{
    /// <summary>
    /// Tests for collisions and computes contact information between two polygons.
    /// </summary>
    /// <param name="polygonA">The vertices of polygonA</param>
    /// <param name="polygonB">The vertices of polygonB</param>
    /// <returns></returns>
    public static (bool hasCollision, AeroVec2 normal, float depth, ContactPoint contact) TestPolygonPolygon(
        AeroPolygon polygonA,
        AeroPolygon polygonB)
    {
        if (polygonA.FindMinimumSeparation(polygonB) >= 0)
        {
            return (false, default, default, default);
        }

        if (polygonB.FindMinimumSeparation(polygonA) >= 0)
        {
            return (false, default, default, default);
        }
        
        return (true, default, default, default);
    }
}