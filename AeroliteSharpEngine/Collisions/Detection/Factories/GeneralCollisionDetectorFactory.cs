using AeroliteSharpEngine.Collisions.Detection.Interfaces;
using AeroliteSharpEngine.Collisions.Detection.NarrowPhase;

namespace AeroliteSharpEngine.Collisions.Detection.Factories;

public class GeneralCollisionDetectorFactory : ICollisionDetectorFactory
{
    public INarrowPhase CreateDetector(CollisionAlgorithm algorithm)
    {
        return new GeneralShapeCollisionDetector();
    }
}