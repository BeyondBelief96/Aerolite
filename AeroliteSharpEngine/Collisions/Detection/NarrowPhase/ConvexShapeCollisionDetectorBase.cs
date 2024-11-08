using AeroliteSharpEngine.AeroMath;
using AeroliteSharpEngine.Collisions.Detection.CollisionPrimitives;
using AeroliteSharpEngine.Collisions.Detection.Interfaces;
using AeroliteSharpEngine.Core.Interfaces;
using AeroliteSharpEngine.Shapes;

namespace AeroliteSharpEngine.Collisions.Detection.NarrowPhase;

public abstract class ConvexShapeCollisionDetectorBase : INarrowPhase
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
    
    protected abstract CollisionManifold TestPolygonPolygon(
        IPhysicsObject2D bodyA,
        IPhysicsObject2D bodyB,
        AeroPolygon polygonA,
        AeroPolygon polygonB);
    
    protected virtual CollisionManifold TestCircleCircle(
        IPhysicsObject2D bodyA,
        IPhysicsObject2D bodyB,
        AeroCircle circleA,
        AeroCircle circleB)
    {
        // Implementation from your existing code
        var manifold = new CollisionManifold
        {
            ObjectA = bodyA,
            ObjectB = bodyB,
            HasCollision = false
        };

        var ab = bodyB.Position - bodyA.Position;
        var distanceSquared = ab.MagnitudeSquared;
        var radiusSum = circleA.Radius + circleB.Radius;

        if (distanceSquared > radiusSum * radiusSum)
        {
            return manifold;
        }

        var distance = ab.Magnitude;
        if (AeroMathExtensions.IsNearlyZero(distance))
        {
            manifold.HasCollision = true;
            manifold.Normal = new AeroVec2(-1, 0);
            manifold.Contact = new ContactPoint
            {
                StartPoint = bodyB.Position - new AeroVec2(circleB.Radius, 0),
                EndPoint = bodyA.Position + new AeroVec2(circleA.Radius, 0),
                Depth = radiusSum
            };
            return manifold;
        }

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

    protected virtual CollisionManifold TestCirclePolygon(
        IPhysicsObject2D circleBody,
        IPhysicsObject2D polygonBody,
        AeroCircle circle,
        AeroPolygon polygon)
    {
        // Implementation of circle-polygon collision
        return new CollisionManifold { HasCollision = false };
    }
}