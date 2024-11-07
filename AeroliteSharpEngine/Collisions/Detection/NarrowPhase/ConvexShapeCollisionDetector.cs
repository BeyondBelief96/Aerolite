using AeroliteSharpEngine.AeroMath;
using AeroliteSharpEngine.Collisions.Detection.CollisionPrimitives;
using AeroliteSharpEngine.Collisions.Detection.Interfaces;
using AeroliteSharpEngine.Core.Interfaces;
using AeroliteSharpEngine.Shapes;

namespace AeroliteSharpEngine.Collisions.Detection.NarrowPhase;

public class ConvexShapeCollisionDetector : INarrowPhase
{
    public CollisionManifold TestCollision(IPhysicsObject2D? objectA, IPhysicsObject2D? objectB)
    {
        if (objectA == null || objectB == null || objectA.Equals(objectB))
        {
            return new CollisionManifold { HasCollision = false };
        }
        
        return (objectA.Shape, objectB.Shape) switch
        {
            (AeroCircle circleA, AeroCircle circleB) => 
                TestCircleCircle(objectA, objectB, circleA, circleB),
            (AeroPolygon polygonA, AeroPolygon polygonB) => 
                TestPolygonPolygon(objectA, objectB, polygonA, polygonB),
            (AeroCircle circle, AeroPolygon polygon) => 
                TestCirclePolygon(objectA, objectB, circle, polygon),
            (AeroPolygon polygon, AeroCircle circle) => 
                TestCirclePolygon(objectB, objectA, circle, polygon),
            _ => new CollisionManifold { HasCollision = false }
        };
    }

    private static CollisionManifold TestCircleCircle(
        IPhysicsObject2D bodyA, 
        IPhysicsObject2D bodyB, 
        AeroCircle circleA, 
        AeroCircle circleB)
    {
        var manifold = new CollisionManifold
        {
            ObjectA = bodyA,
            ObjectB = bodyB,
            HasCollision = false
        };

        // Vector from center of A to center of B
        var ab = bodyB.Position - bodyA.Position;
        var distanceSquared = ab.MagnitudeSquared;
        var radiusSum = circleA.Radius + circleB.Radius;
        
        // If true, then no collision is detected, early return.
        if (distanceSquared > radiusSum * radiusSum)
        {
            return manifold;
        }

        var distance = ab.Magnitude;
        if (AeroMathExtensions.IsNearlyZero(distance))
        {
            manifold.HasCollision = true;
            manifold.Normal = new AeroVec2(-1, 0); // Arbitrary direction but pointing toward A
            manifold.Contact = new ContactPoint
            {
                StartPoint = bodyB.Position - new AeroVec2(circleB.Radius, 0),
                EndPoint = bodyA.Position + new AeroVec2(circleA.Radius, 0),
                Depth = radiusSum
            };
            return manifold;
        }
        
        // Compute contact information
        manifold.HasCollision = true;
        manifold.Normal = ab.UnitVector();
        manifold.Contact = new ContactPoint()
        {
            StartPoint = bodyB.Position - manifold.Normal * circleB.Radius,
            EndPoint = bodyA.Position + manifold.Normal * circleA.Radius,
        };
        
        manifold.Contact.Depth = (manifold.Contact.EndPoint - manifold.Contact.StartPoint).Magnitude;

        return manifold;
    }

    private static CollisionManifold TestPolygonPolygon(
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
        
        var (hasCollision, normal, depth, contactPoint) = SeparatingAxisCollisionDetector.TestPolygonPolygon(
            polygonA, polygonB);
        
        if(!hasCollision) return manifold;

        manifold.HasCollision = hasCollision;
        manifold.Normal = normal;

        return manifold;
    }

    private static CollisionManifold TestCirclePolygon(
        IPhysicsObject2D circleBody, 
        IPhysicsObject2D polygonBody,
        AeroCircle circle,
        AeroPolygon polygon)
    {
        var manifold = new CollisionManifold
        {
            ObjectA = circleBody,
            ObjectB = polygonBody,
            HasCollision = false,
        };

        // TODO: Implement circle-polygon collision test
        // This should:
        // 1. Find closest point on polygon to circle center
        // 2. Check for intersection
        // 3. Create contact point at intersection
            
        return manifold;
    }
}