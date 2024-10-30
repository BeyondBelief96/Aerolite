using AeroliteSharpEngine.Collision;

namespace AeroliteSharpEngine.Collisions.Detection
{
    /// <summary>
    /// Factory for creating appropriate collision detectors based on shape requirements
    /// </summary>
    public static class CollisionDetectorFactory
    {
        public static INarrowPhase CreateNarrowPhase(CollisionSystemType type)
        {
            return type switch
            {
                CollisionSystemType.ConvexOnly => new ConvexShapeCollisionDetector(),
                CollisionSystemType.General => new GeneralShapeCollisionDetector(),
                _ => throw new ArgumentException("Unknown collision system type")
            };
        }
    }
}
