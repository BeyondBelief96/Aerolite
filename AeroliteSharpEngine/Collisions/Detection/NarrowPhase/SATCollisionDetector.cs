using AeroliteSharpEngine.AeroMath;
using AeroliteSharpEngine.Collisions.Detection.CollisionPrimitives;
using AeroliteSharpEngine.Collisions.Detection.PrimitiveTests;
using AeroliteSharpEngine.Core.Interfaces;
using AeroliteSharpEngine.Shapes;

namespace AeroliteSharpEngine.Collisions.Detection.NarrowPhase;

/// <summary>
/// Implementation of collision detection using the Separating Axis Theorem (SAT)
/// for convex shapes.
/// </summary>
public class SATCollisionDetector : ConvexShapeCollisionDetector
{
    protected override void TestPolygonPolygon(
        CollisionManifold manifold,
        IPhysicsObject2D bodyA,
        IPhysicsObject2D bodyB,
        AeroPolygon polygonA,
        AeroPolygon polygonB)
    {
        float abSeparation = polygonA.FindMinimumSeparation(polygonB, out var aAxis, out var aPoint);
        if (abSeparation >= 0)
        {
            return; // No collision
        }

        float baSeparation = polygonB.FindMinimumSeparation(polygonA, out var bAxis, out var bPoint);
        if (baSeparation >= 0)
        {
            return; // No collision
        }

        // We have a collision - set up the manifold
        manifold.HasCollision = true;

        if (abSeparation > baSeparation)
        {
            manifold.Contact.Depth = -abSeparation;
            manifold.Normal = aAxis;
            manifold.Contact.StartPoint = aPoint;
            manifold.Contact.EndPoint = aPoint + manifold.Normal * manifold.Contact.Depth;
        }
        else
        {
            manifold.Contact.Depth = -baSeparation;
            manifold.Normal = -bAxis;
            manifold.Contact.StartPoint = bPoint;
            manifold.Contact.EndPoint = bPoint + manifold.Normal * manifold.Contact.Depth;
        }
    }

    /// <summary>
    /// Tests for collisions and computes contact information between two polygons.
    /// </summary>
    /// <param name="polygonA">The vertices of polygonA</param>
    /// <param name="polygonB">The vertices of polygonB</param>
    /// <returns></returns>
    private static (bool hasCollision, AeroVec2 normal, ContactPoint? contact) TestPolygonPolygon(
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