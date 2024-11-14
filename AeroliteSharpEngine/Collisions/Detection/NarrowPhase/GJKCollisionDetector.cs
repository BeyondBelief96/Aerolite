using AeroliteSharpEngine.Collisions.Detection.CollisionPrimitives;
using AeroliteSharpEngine.Core.Interfaces;
using AeroliteSharpEngine.Shapes;

namespace AeroliteSharpEngine.Collisions.Detection.NarrowPhase;

public class GJKCollisionDetector : ConvexShapeCollisionDetectorBase
{
    protected override CollisionManifold TestPolygonPolygon(
        IPhysicsObject2D bodyA,
        IPhysicsObject2D bodyB,
        AeroPolygon polygonA,
        AeroPolygon polygonB)
    {
        // Implement GJK algorithm here
        throw new NotImplementedException("GJK algorithm not implemented yet");
    }

    protected override CollisionManifold TestCirclePolygon(IPhysicsObject2D circleBody, IPhysicsObject2D polygonBody, AeroCircle circle,
        AeroPolygon polygon)
    {
        throw new NotImplementedException();
    }
}