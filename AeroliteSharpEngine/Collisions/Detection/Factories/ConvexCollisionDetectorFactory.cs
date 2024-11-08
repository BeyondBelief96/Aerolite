using AeroliteSharpEngine.Collisions.Detection.Interfaces;
using AeroliteSharpEngine.Collisions.Detection.NarrowPhase;

namespace AeroliteSharpEngine.Collisions.Detection.Factories;

public class ConvexCollisionDetectorFactory : ICollisionDetectorFactory
{
    public INarrowPhase CreateDetector(CollisionAlgorithm algorithm)
    {
        return algorithm switch
        {
            CollisionAlgorithm.SAT => new SATCollisionDetector(),
            CollisionAlgorithm.GJK => new GJKCollisionDetector(),
            _ => throw new ArgumentException($"Unsupported algorithm: {algorithm}")
        };
    }
}
