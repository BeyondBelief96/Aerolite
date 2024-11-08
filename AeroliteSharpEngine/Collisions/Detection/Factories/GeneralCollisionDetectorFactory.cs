using AeroliteSharpEngine.Collisions.Detection.Interfaces;

namespace AeroliteSharpEngine.Collisions.Detection.Factories;

public class GeneralCollisionDetectorFactory : ICollisionDetectorFactory
{
    public INarrowPhase CreateDetector(CollisionAlgorithm algorithm)
    {
        return new GeneralShapeCollisionDetector();
    }
}