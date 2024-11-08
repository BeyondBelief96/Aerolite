using AeroliteSharpEngine.AeroMath;
using AeroliteSharpEngine.Collisions.Detection.CollisionPrimitives;
using AeroliteSharpEngine.Core.Interfaces;
using AeroliteSharpEngine.Shapes;

namespace AeroliteSharpEngine.Collisions.Detection.NarrowPhase;

/// <summary>
/// Implementation of collision detection using the Separating Axis Theorem (SAT)
/// for convex shapes.
/// </summary>
public class SATCollisionDetector : ConvexShapeCollisionDetectorBase
{
    protected override CollisionManifold TestPolygonPolygon(
        IPhysicsObject2D bodyA,
        IPhysicsObject2D bodyB,
        AeroPolygon polygonA,
        AeroPolygon polygonB)
    {
        var manifold = new CollisionManifold
        {
            ObjectA = bodyA,
            ObjectB = bodyB,
            HasCollision = false,
        };

        var (hasCollision, normal, contactPoint) = TestPolygonPolygon(
            polygonA, polygonB);

        if (!hasCollision) return manifold;

        manifold.HasCollision = hasCollision;
        manifold.Normal = normal;
        manifold.Contact = contactPoint;

        return manifold;
    }
    /// <summary>
    /// Tests for collisions and computes contact information between two polygons.
    /// </summary>
    /// <param name="polygonA">The vertices of polygonA</param>
    /// <param name="polygonB">The vertices of polygonB</param>
    /// <returns></returns>
    private static (bool hasCollision, AeroVec2 normal, ContactPoint contact) TestPolygonPolygon(
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