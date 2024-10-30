// First, let's define interfaces for broad and narrow phase collision detection

using AeroliteSharpEngine.Core.Interfaces;

namespace AeroliteSharpEngine.Collisions.Detection.Interfaces
{
    /// <summary>
    /// Interface for narrow phase collision detection algorithms
    /// </summary>
    public interface INarrowPhase
    {
        /// <summary>
        /// Tests for collision between two physics objects and returns detailed collision information
        /// </summary>
        CollisionManifold TestCollision(IPhysicsObject2D object2DA, IPhysicsObject2D object2DB);
    }
}