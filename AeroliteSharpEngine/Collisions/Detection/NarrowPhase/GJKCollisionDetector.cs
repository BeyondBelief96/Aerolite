using AeroliteSharpEngine.Collisions.Detection.CollisionPrimitives;
using AeroliteSharpEngine.Core.Interfaces;
using AeroliteSharpEngine.Shapes;

namespace AeroliteSharpEngine.Collisions.Detection.NarrowPhase;

public class GJKCollisionDetector : ConvexShapeCollisionDetector
{
    protected override void TestPolygonPolygon(
        CollisionManifold manifold,
        IPhysicsObject2D bodyA,
        IPhysicsObject2D bodyB,
        AeroPolygon polygonA,
        AeroPolygon polygonB)
    {
        // Implement GJK algorithm here
        throw new NotImplementedException("GJK algorithm not implemented yet");
    }
}