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
    public static (bool hasCollision, AeroVec2 normal, ContactPoint contact) TestPolygonPolygon(
        AeroPolygon polygonA,
        AeroPolygon polygonB)
    {
        var contactPoint = new ContactPoint();
        AeroVec2 contactNormal;
        float abSeparation = polygonA.FindMinimumSeparation(polygonB, out var aAxis, out var aPoint);
        if (abSeparation >= 0)
        {
            return (false, default, default);
        }

        float baSeparation = polygonB.FindMinimumSeparation(polygonA, out var bAxis, out var bPoint);
        if (baSeparation >= 0)
        {
            return (false, default, default);
        }

        if (abSeparation > baSeparation)
        {
            contactPoint.Depth = -abSeparation;
            contactNormal = aAxis;
            contactPoint.StartPoint = aPoint;
            contactPoint.EndPoint = contactPoint.StartPoint + contactNormal * contactPoint.Depth;
        }
        else
        {
            contactPoint.Depth = -baSeparation;
            contactNormal = -bAxis;
            contactPoint.StartPoint = bPoint;
            contactPoint.EndPoint = contactPoint.StartPoint + contactNormal * contactPoint.Depth;
        }

        return (true, contactNormal, contactPoint);
    }
}