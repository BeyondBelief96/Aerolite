namespace AeroliteSharpEngine.Collisions.Detection.Interfaces;

/// <summary>
/// Factory interface for creating collision detection systems with the chosen
/// algorithm.
/// </summary>
public interface ICollisionDetectorFactory
{
    /// <summary>
    /// Creates an instance of <see cref="INarrowPhase"/> that uses the
    /// chosen collision algorithm.
    /// </summary>
    /// <param name="algorithm"></param>
    /// <returns></returns>
    INarrowPhase CreateDetector(CollisionAlgorithm algorithm);
}