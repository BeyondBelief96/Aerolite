using AeroliteSharpEngine.Collisions.Detection.CollisionPrimitives;
using AeroliteSharpEngine.Collisions.Detection.Interfaces;
using AeroliteSharpEngine.Core.Interfaces;

namespace AeroliteSharpEngine.Collisions.Detection.NarrowPhase;

public abstract class NarrowPhaseBase : INarrowPhase
{
    public CollisionManifold TestCollision(IPhysicsObject2D? objectA, IPhysicsObject2D? objectB)
    {
        var manifold = CollisionPoolService.Instance.GetManifold();
        manifold.ObjectA = objectA;
        manifold.ObjectB = objectB;
        manifold.HasCollision = false; // Reset state
            
        TestCollisionInternal(manifold);
        return manifold;
    }

    protected abstract void TestCollisionInternal(CollisionManifold manifold);
}