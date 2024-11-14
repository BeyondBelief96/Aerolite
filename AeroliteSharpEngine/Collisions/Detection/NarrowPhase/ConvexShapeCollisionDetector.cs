using AeroliteSharpEngine.AeroMath;
using AeroliteSharpEngine.Collisions.Detection.CollisionPrimitives;
using AeroliteSharpEngine.Collisions.Detection.PrimitiveTests;
using AeroliteSharpEngine.Core.Interfaces;
using AeroliteSharpEngine.Shapes;

namespace AeroliteSharpEngine.Collisions.Detection.NarrowPhase;

public abstract class ConvexShapeCollisionDetector : NarrowPhaseBase
{
    protected override void TestCollisionInternal(CollisionManifold manifold)
    {
        if (manifold.ObjectA == null || manifold.ObjectB == null || 
            manifold.ObjectA.Equals(manifold.ObjectB))
        {
            return; // manifold.HasCollision is already false
        }

        switch (manifold.ObjectA.Shape, manifold.ObjectB.Shape)
        {
            case (AeroCircle circleA, AeroCircle circleB):
                TestCircleCircle(manifold, manifold.ObjectA, manifold.ObjectB, circleA, circleB);
                break;
            case (AeroPolygon polygonA, AeroPolygon polygonB):
                TestPolygonPolygon(manifold, manifold.ObjectA, manifold.ObjectB, polygonA, polygonB);
                break;
            case (AeroCircle circle, AeroPolygon polygon):
                TestCirclePolygon(manifold, manifold.ObjectA, manifold.ObjectB, circle, polygon);
                break;
            case (AeroPolygon polygon, AeroCircle circle):
                // Create a swapped manifold for consistent handling
                var swappedManifold = CollisionPoolService.Instance.GetManifold();
                swappedManifold.ObjectA = manifold.ObjectB;
                swappedManifold.ObjectB = manifold.ObjectA;
                TestCirclePolygon(swappedManifold, manifold.ObjectB, manifold.ObjectA, circle, polygon);
                
                // If there was a collision, copy the results back with adjusted normal
                if (swappedManifold.HasCollision)
                {
                    manifold.HasCollision = true;
                    manifold.Normal = -swappedManifold.Normal;
                    manifold.Contact = swappedManifold.Contact;
                }
                
                CollisionPoolService.Instance.ReturnManifold(swappedManifold);
                break;
        }
    }
    
    protected abstract void TestPolygonPolygon(
        CollisionManifold manifold,
        IPhysicsObject2D bodyA,
        IPhysicsObject2D bodyB,
        AeroPolygon polygonA,
        AeroPolygon polygonB);

    protected virtual void TestCircleCircle(
        CollisionManifold manifold,
        IPhysicsObject2D bodyA,
        IPhysicsObject2D bodyB,
        AeroCircle circleA,
        AeroCircle circleB)
    {
        var ab = bodyB.Position - bodyA.Position;
        var distanceSquared = ab.MagnitudeSquared;
        var radiusSum = circleA.Radius + circleB.Radius;

        if (distanceSquared > radiusSum * radiusSum)
        {
            return;
        }

        var distance = ab.Magnitude;
        if (AeroMathExtensions.IsNearlyZero(distance))
        {
            manifold.HasCollision = true;
            manifold.Normal = new AeroVec2(-1, 0);
            manifold.Contact.StartPoint = bodyB.Position - new AeroVec2(circleB.Radius, 0);
            manifold.Contact.EndPoint = bodyA.Position + new AeroVec2(circleA.Radius, 0);
            manifold.Contact.Depth = radiusSum;
            return;
        }

        manifold.HasCollision = true;
        manifold.Normal = ab.UnitVector();
        manifold.Contact.StartPoint = bodyB.Position - manifold.Normal * circleB.Radius;
        manifold.Contact.EndPoint = bodyA.Position + manifold.Normal * circleA.Radius;
        manifold.Contact.Depth = (manifold.Contact.EndPoint - manifold.Contact.StartPoint).Magnitude;
    }

    protected virtual void TestCirclePolygon(
        CollisionManifold manifold,
        IPhysicsObject2D circleBody,
        IPhysicsObject2D polygonBody,
        AeroCircle circle,
        AeroPolygon polygon)
    {
        var closestPoint = CollisionUtilities.FindClosestEdgePoint(polygon.WorldVertices, circleBody.Position);
        var normal = closestPoint - circleBody.Position;
        float distanceSquared = normal.MagnitudeSquared;

        if (distanceSquared > circle.Radius * circle.Radius)
        {
            return;
        }

        float distance = MathF.Sqrt(distanceSquared);
        if (!AeroMathExtensions.IsNearlyZero(distance))
        {
            normal.Normalize();
        }
        else
        {
            normal = polygon.GetEdgeNormal(0);
        }

        manifold.HasCollision = true;
        manifold.Normal = normal;
        manifold.Contact.StartPoint = circleBody.Position + (normal * circle.Radius);
        manifold.Contact.EndPoint = closestPoint;
        manifold.Contact.Depth = circle.Radius - distance;
    }
}