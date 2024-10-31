using AeroliteSharpEngine.AeroMath;
using AeroliteSharpEngine.Collisions.Detection.Interfaces;
using AeroliteSharpEngine.Core.Interfaces;
using AeroliteSharpEngine.Shapes;

namespace AeroliteSharpEngine.Collisions.Detection.NarrowPhase
{
    public class ConvexShapeCollisionDetector : INarrowPhase
    {
        public CollisionManifold TestCollision(IPhysicsObject2D? objectA, IPhysicsObject2D? objectB)
        {
            // Quick check - if either object is null or they're the same object
            if (objectA == null || objectB == null || objectA.Equals(objectB))
            {
                return new CollisionManifold { HasCollision = false };
            }
            
            return (objectA.Shape, objectA.Shape) switch
            {
                (AeroCircle circleA, AeroCircle circleB) => 
                    TestCircleCircle(objectA, objectB, circleA, circleB),
            
                // (AeroCircle circle, AeroBox box) => 
                //     TestCircleBox(objectA, objectA, circle, box),
                //
                // (AeroBox box, AeroCircle circle) => 
                //     TestCircleBox(objectA, objectA, circle, box),
                //
                // (AeroBox boxA, AeroBox boxB) => 
                //     TestBoxBox(objectA, objectA, boxA, boxB),
                //
                // (AeroPolygon polygonA, AeroPolygon polygonB) => 
                //     TestPolygonPolygon(objectA, objectA, polygonA, polygonB),
                //
                // (AeroCircle circle, AeroPolygon polygon) => 
                //     TestCirclePolygon(objectA, objectA, circle, polygon),
                //
                // (AeroPolygon polygon, AeroCircle circle) => 
                //     TestCirclePolygon(objectA, objectA, circle, polygon),
            
                _ => new CollisionManifold { HasCollision = false }
            };
        }

        private static CollisionManifold TestCircleCircle(IPhysicsObject2D bodyA, IPhysicsObject2D bodyB, AeroCircle circleA, AeroCircle circleB)
        {
            var manifold = new CollisionManifold
            {
                Object2DA = bodyA,
                Object2DB = bodyB,
                HasCollision = false
            };

            // Vector from A to B
            var normal = bodyB.Position - bodyA.Position;
            var distanceSquared = normal.MagnitudeSquared;
            var radiusSum = circleA.Radius + circleB.Radius;

            // Quick check if circles are too far apart
            if (distanceSquared > radiusSum * radiusSum)
            {
                return manifold;
            }

            var distance = MathF.Sqrt(distanceSquared);

            // Handle case where circles are exactly on top of each other
            if (distance == 0)
            {
                manifold.HasCollision = true;
                manifold.Depth = radiusSum;
                manifold.Normal = new AeroVec2(1, 0); // Arbitrary direction
                manifold.Point = bodyA.Position;
                return manifold;
            }

            manifold.HasCollision = true;
            manifold.Depth = radiusSum - distance;
            manifold.Normal = normal.UnitVector();
            manifold.Point = bodyA.Position + (manifold.Normal * (circleA.Radius - manifold.Depth));

            return manifold;
        }
    }
}
