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
            HasCollision = false,
            ContactPoints = new List<ContactPoint>()
        };

        var normal = bodyB.Position - bodyA.Position;
        var distanceSquared = normal.MagnitudeSquared;
        var radiusSum = circleA.Radius + circleB.Radius;

        if (distanceSquared > radiusSum * radiusSum)
        {
            return manifold;
        }

        var distance = MathF.Sqrt(distanceSquared);

        // Handle case where circles are exactly on top of each other
        if (distance == 0)
        {
            manifold.HasCollision = true;
            manifold.Normal = new AeroVec2(1, 0); // Arbitrary direction
            manifold.ContactPoints.Add(new ContactPoint
            {
                PointOnA = bodyA.Position + new AeroVec2(circleA.Radius, 0),
                PointOnB = bodyB.Position + new AeroVec2(circleB.Radius, 0),
                Depth = radiusSum
            });
            return manifold;
        }

        manifold.HasCollision = true;
        manifold.Normal = normal / distance; // Normalized
        var depth = radiusSum - distance;

        // For circles, there's always exactly one contact point
        var contactOnA = bodyA.Position + manifold.Normal * circleA.Radius;
        var contactOnB = bodyB.Position - manifold.Normal * circleB.Radius;

        manifold.ContactPoints.Add(new ContactPoint
        {
            PointOnA = contactOnA,
            PointOnB = contactOnB,
            Depth = depth
        });

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
            ContactPoints = []
        };
        
        var (hasCollision, normal, depth, contactPoint) = SeparatingAxisCollisionDetector.TestPolygonPolygon(
            polygonA.WorldVertices, polygonB.WorldVertices);
        
        if(!hasCollision) return manifold;

        manifold.HasCollision = true;
        manifold.Normal = normal;
        manifold.ContactPoints.Add(contactPoint);

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
            ContactPoints = []
        };

        // TODO: Implement circle-polygon collision test
        // This should:
        // 1. Find closest point on polygon to circle center
        // 2. Check for intersection
        // 3. Create contact point at intersection
            
        return manifold;
    }
}